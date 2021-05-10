using System;
using System.Globalization;
using System.Text;
using static Math.Interval.Symbols;

namespace Math.Interval
{
    public enum IntervalFormattingOptions
    {
        Default,
        SimplifyExpression
    }

    public static class Formatter<T, U>
        where T : struct, IComparable<T>, IEquatable<T>, IFormattable
        where U : IInterval<T>
    {
        public static string Format(
            in U interval,
            IntervalFormattingOptions intervalFormat = IntervalFormattingOptions.Default,
            string endpointFormat = "G",
            IFormatProvider provider = null)
        {
            return intervalFormat switch
            {
                IntervalFormattingOptions.Default => FormatNormal(interval),
                IntervalFormattingOptions.SimplifyExpression => interval switch
                {
                    { IsDegenerate: true } => To(interval.Minimum),
                    { IsEmpty: true } => Encoding.UTF8.GetString(emptySymbol),
                    IUnboundedInterval<T> infinite => infinite switch
                    {
                        { IsBounded: true } => FormatNormal(interval),
                        { IsRightBounded: false }
                            => (infinite.IsMinimumInclusive ? Encoding.UTF8.GetString(greaterEqualSymbol) : greaterThan) + To(infinite.Minimum),
                        { IsLeftBounded: false }
                            => (infinite.IsMaximumInclusive ? Encoding.UTF8.GetString(lessEqualSymbol) : lessThan) + To(infinite.Minimum),
                        _ => throw new InvalidOperationException("Unsupported simplified format case for unbounded interval"),
                    },
                    _ => throw new InvalidOperationException("Unsupported simplified format case")
                },
                _ => throw new InvalidOperationException("Unsupported formatting arguments")
            };

            string FormatNormal(in U interval)
            {
                var left = interval.IsMinimumInclusive ? leftSquare : leftParen;
                var right = interval.IsMaximumInclusive ? rightSquare : rightParen;
                return $"{left}{To(interval.Minimum)}{comma} {To(interval.Maximum)}{right}";
            }
            string To(T value) => value.ToString(endpointFormat, provider ?? CultureInfo.CurrentCulture);
        }

    }
}