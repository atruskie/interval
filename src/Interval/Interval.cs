
using System;
using System.Globalization;

using static Math.Interval.Topology;
using static Math.Interval.Comparison;
using static Math.Interval.IntersectionDetails;

namespace Math.Interval
{
    public readonly partial struct Interval : IUnboundedInterval<double>
    {
        public static readonly Interval Unit = new(0, 1);

        public Interval(double minimum, double maximum) : this()
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public Interval(double minimum, double maximum, Topology topology) : this()
        {
            Minimum = minimum;
            Maximum = maximum;
            Topology = topology;
        }

        public Interval(in (double Value, bool Inclusive) minimum, in (double Value, bool Inclusive) maximum)
        {
            Minimum = minimum.Value;
            Maximum = maximum.Value;
            Topology = TopologyExtensions.Create(minimum.Inclusive, maximum.Inclusive);
        }

        public double Minimum { get; init; }
        public double Maximum { get; init; }
        public Topology Topology { get; init; }

        /// <summary>
        /// Gets the original value of a interval that specified as a target value and tolerance tuple.
        /// In cases where a interval was defined as minimum and maximum it instead gets the 
        /// geometric mean of the interval. 
        /// In the case of a relation (or an interval with an infinite bound) it returns whichever bound is not infinite.
        /// </summary>
        /// <remarks>
        /// This property is designed to be give a useful real number that allows the interval to placed in a domain.
        /// If you need the strict mathematical definition of the midpoint then see <c>Center</c>.
        /// <value></value>
        public double Anchor => this switch {
            { IsBounded: true } => Center,
            { IsLeftBounded: true } => Minimum,
            { IsRightBounded: true } => Maximum,
            _ => throw new InvalidOperationException("Unsupported middle value")
        };

        /// <summary>
        /// Gets the midpoint of the interval.
        /// </summary>
        /// <remarks>
        /// Differs from <c>Anchor</c> in that is the mathematical definition only.
        /// If either endpoint is Infinite, the result will be infinite.
        /// </remarks>
        /// <returns>The midpoint.</returns>
        public double Center => Maths.Center(Minimum, Maximum);


        public bool IsMinimumInclusive => Topology.IsMinimumInclusive();
        public bool IsMaximumInclusive => Topology.IsMaximumInclusive();
        public bool IsDegenerate => Minimum.Equals(Maximum) && Topology == Inclusive;
        public bool IsEmpty => Minimum.Equals(Maximum) && Topology != Inclusive;
        public bool IsProper => !IsDegenerate && !IsEmpty;

        public IInterval<double> Interior()
            => new Interval() { Minimum = Minimum, Maximum = Maximum, Topology = Exclusive };

        public double Range => Maximum - Minimum;

        public double Radius => System.Math.Abs(Minimum - Maximum) / 2;

        public bool IsLeftBounded => Minimum != double.NegativeInfinity;
        public bool IsRightBounded => Maximum != double.PositiveInfinity;
        public bool IsBounded => IsLeftBounded && IsRightBounded;


        public int CompareMinimums(IInterval<double> other) => Minimum.CompareTo(other.Minimum) switch
        {
            Equal => Topology.IsMinimumCompatible(other.Topology) ? Equal : Less,
            int i => i,
        };

        public int CompareMaximums(IInterval<double> other) => Maximum.CompareTo(other.Maximum) switch
        {
            Equal => Topology.IsMaximumCompatible(other.Topology) ? Equal : Less,
            int i => i,
        };
        public int CompareTo(Interval? other)
        {
            if (other is null)
            {
                return Less;
            }

            return CompareTo(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Interval i)
            {
                return Equals(i);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Minimum, Maximum, Topology);
        }

        public bool Equals(IInterval<double> other)
        {
            return
                Minimum.Equals(other.Minimum) &&
                Maximum.Equals(other.Maximum) &&
                Topology == other.Topology;
        }


        public bool Equals(Interval other)
        {
            return
                Minimum.Equals(other.Minimum) &&
                Maximum.Equals(other.Maximum) &&
                Topology == other.Topology;
        }

        public int CompareTo(IInterval<double> other)
        {
            int comparison = CompareMinimums(other);
            return comparison == Equal ? CompareMaximums(other) : comparison;
        }
        public int CompareTo(Interval other)
        {
            int comparison = CompareMinimums(other);
            return comparison == Equal ? CompareMaximums(other) : comparison;
        }

        public static bool operator ==(Interval left, Interval right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Interval left, Interval right)
        {
            return !(left == right);
        }

        public static bool operator <(Interval left, Interval right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Interval left, Interval right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Interval left, Interval right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Interval left, Interval right)
        {
            return left.CompareTo(right) >= 0;
        }


        public static implicit operator Interval(((double Value, bool Inclusive) Minimum, (double Value, bool Inclusive) Maximum) value)
        {
            return new Interval(value.Minimum, value.Maximum);
        }

        public static implicit operator Interval((double Minimum, double Maximum) value)
        {
            return new Interval(value.Minimum, value.Maximum);
        }

        public static implicit operator Interval((double Minimum, double Maximum, Topology Topology) value)
        {
            return new Interval(value.Minimum, value.Maximum, value.Topology);
        }


        public bool Contains(double value) => IntervalHelpers<double, Interval>.Contains(this, value);

        public bool IsProperSubset(IInterval<double> other)
            => GetIntersection(this, other).IsProperSubset;

        public bool IsProperSuperset(IInterval<double> other)
            => GetIntersection(this, other).IsProperSuperset;

        public bool IsSubset(IInterval<double> other)
            => GetIntersection(this, other).IsSubset;

        public bool IsSuperset(IInterval<double> other)
            => GetIntersection(this, other).IsSuperset;

        public bool IntersectsWith(IInterval<double> other)
            => GetIntersection(this, other).IsIntersecting;

        public Interval Union(Interval other)
            => (Interval)IntervalHelpers<double, Interval>.Union(this, other, Create);

        public SplitResult<Interval> Difference(Interval other)
            => IntervalHelpers<double, Interval>.Difference(this, other, Create);


        public (Interval Lower, Interval Upper) SymmetricDifference(Interval other)
             => ((Interval Lower, Interval Upper))IntervalHelpers<double, Interval>.SymmetricDifference(this, other, Create);

        public Interval Intersection(Interval other)
            => (Interval)IntervalHelpers<double, Interval>.Intersection(this, other, Create);

        public PartitionResult<IInterval<double>> Partition(double point)
            => IntervalHelpers<double, Interval>.Partition(this, point, Create);

        public ((double Value, bool Inclusive) Minimum, (double Value, bool Inclusive) Maximum) Deconstruct()
        {
            return ((Minimum, IsMinimumInclusive), (Maximum, IsMaximumInclusive));
        }

        public override string ToString()
        {
            return Formatter<double, Interval>.Format(this);
        }

        public string ToString(IntervalFormattingOptions options, string endpointFormatString, IFormatProvider formatProvider)
        {
            return Formatter<double, Interval>.Format(this, options, endpointFormatString, formatProvider);
        }

        public double UnitNormalize(double value, bool clamp = true)
        {
            var v = (value - Minimum) / Range;
            return clamp ? System.Math.Clamp(v, 0, 1) : v;
         }
    }
}