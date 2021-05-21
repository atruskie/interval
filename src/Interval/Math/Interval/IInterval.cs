
using System;
using static Math.Interval.Topology;
using static Math.Interval.Comparison;
using static Math.Interval.IntersectionDetails;

namespace Math.Interval
{
    public interface IInterval<T> : IEquatable<IInterval<T>>, IComparable<IInterval<T>>//, ISet<Interval<T>>
        where T : struct, IComparable<T>, IEquatable<T>, IFormattable
    {
        T Minimum { get; init; }
        T Maximum { get; init; }
        Topology Topology { get; init; }

        bool IsMinimumInclusive { get; }
        bool IsMaximumInclusive { get; }
        bool IsDegenerate { get; }
        bool IsEmpty { get; }
        bool IsProper { get; }

        /// <summary>
        /// Gets the largest open interval (excluding endpoints) within the bounds of the current interval.
        /// </summary>
        /// <returns>A new interior interval</returns>
        IInterval<T> Interior();

        int CompareMinimums(IInterval<T> other);

        int CompareMaximums(IInterval<T> other);




        bool Contains(T value);

        bool IsProperSubset(IInterval<T> other);
        bool IsProperSuperset(IInterval<T> other);
        bool IsSubset(IInterval<T> other);
        bool IsSuperset(IInterval<T> other);
        bool IntersectsWith(IInterval<T> other);



        IntersectionDetails GetIntersectionDetails(IInterval<T> other) => GetIntersection(this, other);



        Interval<T> Union(Interval<T> other);
        SplitResult<Interval<T>> Difference(Interval<T> other);
        BiSplitResult<T> SymmetricDifference(Interval<T> other);
        Interval<T> Intersection(Interval<T> other);


        PartitionResult<IInterval<T>> Partition(T point);



        void Deconstruct(out (T Value, bool Inclusive) minimum, out (T Value, bool Inclusive) maximum);
        void Deconstruct(out T minimum, out T maximum, out Topology topology);

        string ToString();
        string ToString(IntervalFormattingOptions intervalFormat, string endpointFormat, IFormatProvider provider);
    }

    public interface IUnboundedInterval<T> : IInterval<T>
        where T : struct, IComparable<T>, IEquatable<T>, IFormattable
    {
        bool IsLeftBounded { get; }
        bool IsRightBounded { get; }
        bool IsBounded { get; }

        BiSplitResult<T> Complement();
    }

    public abstract record SplitResult<T>();
    public record BiSplitResult<T>(T Lower, T Upper) : SplitResult<T>();
    public record MonoSplitResult<T>(T Result) : SplitResult<T>();

    public abstract record PartitionResult<T>();
    public record DisjointPartition<T>(T Disjoint) : PartitionResult<T>();
    public record ValidPartition<T>(T Lower, T Point, T Upper) : PartitionResult<T>();


}