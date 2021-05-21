using System;
using System.Buffers.Text;
using System.Text;

using static Math.Interval.Topology;
using static Math.Interval.Symbols;
using Util;

namespace Math.Interval
{
    public class Parser<T, U>
        where T : struct, IComparable<T>, IEquatable<T>, IFormattable
        where U : IInterval<T>
    {
        private readonly Func<T, T, U> createTolerance;
        private readonly Func<T, U> createApproximation;
        private readonly Func<T, U> createSameOrderOfMagnitude;

        public T Epsilon { get; }
        public IntervalHelpers<T, U>.Create Create { get; }
        public T Min { get; }
        public T Max { get; }
        public EndpointParser EndPointParser { get; }

        public delegate bool EndpointParser(in ReadOnlySpan<byte> bytes, out T value, out int consumed);

        internal Parser(T epsilon, IntervalHelpers<T,U>.Create create, T min, T max, EndpointParser endPointParser, 
            Func<T, T, U> createTolerance = null, Func<T,U> createApproximation = null, Func<T,U> createSameOrderOfMagnitude = null) {
            Epsilon = epsilon;
            Create = create;
            Min = min;
            Max = max;
            EndPointParser = endPointParser;
            this.createTolerance = createTolerance;
            this.createApproximation = createApproximation;
            this.createSameOrderOfMagnitude = createSameOrderOfMagnitude;
        }


        
        public U Parse(ReadOnlySpan<byte> @string)
        {
            if (TryParse(@string, out var value, out var error))
            {
                return value;
            }

            throw new ArgumentException(error);
        }

        public bool TryParse(ReadOnlySpan<byte> @string, out U value, out string error)
        {
            // see docs/interval.ebnf for valid formats we can parse here
            error = null;
            if (@string.SequenceEqual(emptySymbol))
            {
                value = IntervalHelpers<T,U>.Empty(Create);
                return true;
            }
            else if (ParseNumber(@string, out var number, out var next))
            {
                // number or tolerance
                if (next.IsEmpty)
                {
                    // number
                    value = IntervalHelpers<T,U>.Degenerate(number, Create);
                    return true;
                }

                // tolerance?
                if (ParseTolerance(number, next, out value, ref error))
                {
                    return true;
                }
            }
            else if (ParseInterval(@string, out value, ref error))
            {
                // bounded 'traditional' interval
                return true;

            }
            else if (ParseApproximation(@string, out value, ref error))
            {
                // approximation
                return true;
            }
            else if (ParseInequality(@string, out value, ref error))
            {
                // inequality
                return true;
            }
            else
            {
                error ??= "Unknown interval format.";
            }

            var input = Encoding.UTF8.GetString(@string);
            error = $"Failed to parse `{input}` as an interval. {error}";
            return false;
        }

        private bool ParseNumber(in ReadOnlySpan<byte> span, out T value, out ReadOnlySpan<byte> next)
        {
            if (span.TryConsume(epsilonSymbol, out next))
            {
                value = Epsilon;
                return true;
            }


            if (EndPointParser(span, out value, out var consumed))
            {
                next = span[consumed..];
                return true;
            }

            return false;
        }

        private bool EnsureTerminal(in ReadOnlySpan<byte> span, ref string error)
        {
            if (span.IsEmpty)
            {
                return true;
            }

            error = $"Characters left over: {span.ToString()}";
            return false;
        }

        private bool EnsureNumber(in ReadOnlySpan<byte> span, out T number, out ReadOnlySpan<byte> next, ref string error)
        {
            if (ParseNumber(span, out number, out next))
            {
                return true;
            }

            number = default;
            error = $" `{span.ToString()}` is not a valid number";
            return false;
        }

        private bool ParseTolerance(T first, ReadOnlySpan<byte> span, out U value, ref string error)
        {
            if (!span.TryConsume(toleranceSymbol, out ReadOnlySpan<byte> next) && !span.TryConsume(toleranceSymbolAlt, out next))
            {
                value = default;
                error = @"`±` or `+-` not found after number";
                return false;
            }

            if (EnsureNumber(next, out var tolerance, out next, ref error) && EnsureTerminal(next, ref error))
            {
                if (createTolerance is null) {
                    error += $"Cannot process a tolerance for the type {typeof(T).Name}";
                    value = default;
                    return false;
                }

                value = createTolerance(first, tolerance);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        private bool ParseApproximation(ReadOnlySpan<byte> span, out U value, ref string error)
        {
            Func<T, U> factory;

            if (span.TryConsume(tildeSymbol, out var next))
            {
                factory = createSameOrderOfMagnitude;

            }
            else if (span.TryConsume(approximateSymbol, out next))
            {
                factory = createApproximation;
            }
            else
            {
                value = default;
                error = null;
                return false;
            }

            if (EnsureNumber(next, out var number, out next, ref error) && EnsureTerminal(next, ref error))
            {
                if (factory is null) {
                    error += $"Cannot process a approximation or same order of magnitude for the type {typeof(T).Name}";
                    value = default;
                    return false;
                }

                value = factory(number);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        private bool ParseInequality(ReadOnlySpan<byte> span, out U value, ref string error)
        {
            Topology topology;
            bool lowAnchor;
            if (span.TryConsume(greaterThan, out ReadOnlySpan<byte> next))
            {
                topology = MinimumExclusiveMaximumInclusive;
                lowAnchor = true;

            }
            else if (span.TryConsume(lessThan, out next))
            {
                topology = MinimumExclusiveMaximumInclusive;
                lowAnchor = false;
            }
            else if (span.TryConsume(lessEqualSymbol, out next) || span.TryConsume(lessEqualSymbolAlt, out next))
            {
                topology = Inclusive;
                next = span[lessEqualSymbol.Length..];
                lowAnchor = false;
            }
            else if (span.TryConsume(greaterEqualSymbol, out next) || span.TryConsume(greaterEqualSymbolAlt, out next))
            {
                topology = Inclusive;
                lowAnchor = true;
            }
            else
            {
                value = default;
                error = null;
                return false;
            }

            if (EnsureNumber(next, out var anchor, out next, ref error) && EnsureTerminal(next, ref error))
            {
                value = Create(
                    (lowAnchor ? anchor : Min, topology.IsMinimumInclusive()),
                    (lowAnchor ? Max : anchor, topology.IsMaximumInclusive())
                );
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        private bool ParseInterval(ReadOnlySpan<byte> span, out U value, ref string error)
        {
            if (span[0] is not leftSquare or leftParen)
            {
                value = default;
                return false;
            }

            value = default;
            Topology? topology = (span[0], span[^1]) switch
            {
                (leftSquare, rightSquare) => Closed,
                (leftSquare, rightParen) => LeftClosedRightOpen,
                (leftParen, rightSquare) => LeftOpenRightClosed,
                (leftParen, rightParen) => Open,
                _ => null
            };

            if (!topology.HasValue)
            {
                error = null;
                return false;
            }

            var next = span[1..^1];

            if (!ParseNumber(next, out var minimum, out next))
            {
                error = $"Count not parse interval minimum in {span.ToString()}";
                return false;
            }

            if (!span.TryConsume(comma, out next))
            {
                error = $"Missing `,` (the comma) in the interval {span.ToString()}";
                return false;
            }

            while (char.IsWhiteSpace((char)next[0]))
            {
                next = next[1..];
            }

            if (!ParseNumber(next, out var maximum, out next))
            {
                error = $"Count not parse interval maximum in {span.ToString()}";
                return false;
            }

            if (EnsureTerminal(next, ref error))
            {
                value = Create(
                    (minimum, topology.Value.IsMinimumInclusive()),
                    (maximum, topology.Value.IsMaximumInclusive())
                );

                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}