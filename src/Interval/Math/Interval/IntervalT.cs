
using System;
using static Math.Interval.Topology;
using static Math.Interval.Comparison;
using static Math.Interval.IntersectionDetails;
using System.Globalization;

namespace Math.Interval
{

    /// <summary>
    /// A generic interval implementation based on IComparable<T>.
    /// </summary>
    /// <remarks>
    /// Does not support unbounded intervals. See <see cref="Inteval"/> for
    /// unbounded support (which possible due to an explicit IInterval<double> implementation).
    /// </remarks>
    public readonly partial struct Interval<T> : IInterval<T>
        where T : struct, IComparable<T>, IEquatable<T>, IFormattable
    {
        public Interval(T minimum, T maximum) : this()
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public Interval(T minimum, T maximum, Topology topology) : this()
        {
            Minimum = minimum;
            Maximum = maximum;
            Topology = topology;
        }

        public Interval((T Value, bool Inclusive) minimum, (T Value, bool Inclusive) maximum)
        {
            Minimum = minimum.Value;
            Maximum = maximum.Value;
            Topology = TopologyExtensions.Create(minimum.Inclusive, maximum.Inclusive);
        }


        public T Minimum { get; init; }
        public T Maximum { get; init; }
        public Topology Topology { get; init; }

        public bool IsMinimumInclusive => Topology.IsMinimumInclusive();
        public bool IsMaximumInclusive => Topology.IsMaximumInclusive();
        public bool IsDegenerate => Minimum.Equals(Maximum) && Topology == Inclusive;
        public bool IsEmpty => Minimum.Equals(Maximum) && Topology != Inclusive;
        public bool IsProper => !IsDegenerate && !IsEmpty;
        public IInterval<T> Interior()
            => new Interval<T>() { Minimum = Minimum, Maximum = Maximum, Topology = Exclusive };

        public int CompareMinimums(IInterval<T> other) => Minimum.CompareTo(other.Minimum) switch
        {
            Equal => Topology.IsMinimumCompatible(other.Topology) ? Equal : Less,
            int i => i,
        };

        public int CompareMaximums(IInterval<T> other) => Maximum.CompareTo(other.Maximum) switch
        {
            Equal => Topology.IsMaximumCompatible(other.Topology) ? Equal : Less,
            int i => i,
        };

        public override int GetHashCode()
        {
            return HashCode.Combine(Minimum, Maximum, Topology);
        }

        public override bool Equals(object obj)
        {
            if (obj is Interval<T> i)
            {
                return Equals(i);
            }

            return false;
        }

        public bool Equals(IInterval<T> other)
        {
            return Minimum.Equals(other.Minimum) &&
                Maximum.Equals(other.Maximum) &&
                Topology == other.Topology;
        }

        public int CompareTo(IInterval<T> other)
        {
            if (other is null)
            {
                return Less;
            }

            int comparison = CompareMinimums(other);
            return comparison == Equal ? CompareMaximums(other) : comparison;
        }
        public int CompareTo(Interval<T> other)
        {
            int comparison = CompareMinimums(other);
            return comparison == Equal ? CompareMaximums(other) : comparison;
        }

        public static bool operator ==(Interval<T> left, Interval<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Interval<T> left, Interval<T> right)
        {
            return !(left == right);
        }

        public static bool operator <(Interval<T> left, Interval<T> right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Interval<T> left, Interval<T> right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Interval<T> left, Interval<T> right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Interval<T> left, Interval<T> right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static implicit operator Interval<T>(((T Value, bool Inclusive) Minimum, (T Value, bool Inclusive) Maximum) value)
        {
            return new Interval<T>(value.Minimum, value.Maximum);
        }

        public static implicit operator Interval<T>((T Minimum, T Maximum) value)
        {
            return new Interval<T>(value.Minimum, value.Maximum);
        }

        public static implicit operator Interval<T>((T Minimum, T Maximum, Topology Topology) value)
        {
            return new Interval<T>(value.Minimum, value.Maximum, value.Topology);
        }



        public bool Contains(T value) => IntervalHelpers<T, Interval<T>>.Contains(this, value);

        public bool IsProperSubset(IInterval<T> other)
            => GetIntersection(this, other).IsProperSubset;

        public bool IsProperSuperset(IInterval<T> other)
            => GetIntersection(this, other).IsProperSuperset;

        public bool IsSubset(IInterval<T> other)
            => GetIntersection(this, other).IsSubset;

        public bool IsSuperset(IInterval<T> other)
            => GetIntersection(this, other).IsSuperset;

        public bool IntersectsWith(IInterval<T> other)
            => GetIntersection(this, other).IsIntersecting;

        public Interval<T> Union(Interval<T> other)
            => (Interval<T>)IntervalHelpers<T, Interval<T>>.Union(this, other, Create);

        public SplitResult<Interval<T>> Difference(Interval<T> other)
            => IntervalHelpers<T, Interval<T>>.Difference(this, other, Create);

        public (Interval<T> Lower, Interval<T> Upper) SymmetricDifference(Interval<T> other)
             => ((Interval<T> Lower, Interval<T> Upper))IntervalHelpers<T, Interval<T>>.SymmetricDifference(this, other, Create);

        public Interval<T> Intersection(Interval<T> other)
            => (Interval<T>)IntervalHelpers<T, Interval<T>>.Intersection(this, other, Create);

        public PartitionResult<IInterval<T>> Partition(T point)
            => IntervalHelpers<T, Interval<T>>.Partition(this, point, Create);

        public void Deconstruct(out (T Value, bool Inclusive) minimum, out (T Value, bool Inclusive) maximum)
        {
            minimum = (Minimum, IsMinimumInclusive);
            maximum = (Maximum, IsMaximumInclusive);
        }

        public void Deconstruct(out T minimum, out T maximum, out Topology topology)
        {
            minimum = Minimum;
            maximum = Maximum;
            topology = Topology;
        }

        public override string ToString()
        {
            return Formatter<T, Interval<T>>.Format(this, IntervalFormattingOptions.Default, null, CultureInfo.CurrentCulture);
        }

        public string ToString(IntervalFormattingOptions options, string endpointFormatString, IFormatProvider formatProvider)
        {
            return Formatter<T, Interval<T>>.Format(this, options, endpointFormatString, formatProvider);
        }
    }
}