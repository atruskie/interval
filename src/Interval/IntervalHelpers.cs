
using System;
using System.Globalization;
//using static Math.Topology;
using static Math.Interval.Comparison;
using static Math.Interval.Containment;
using static Math.Interval.IntersectionDetails;


namespace Math.Interval
{
    public  static partial class IntervalHelpers<T, U>
        where T : struct, IComparable<T>, IEquatable<T>, IFormattable
                where U : IInterval<T>
    {
        public delegate U Create(in (T Value, bool Inclusive) minimum, in (T Value, bool Inclusive) maximum);

        public static readonly Func<Create, U> Empty = create => create((default, false), (default, false));
        public static readonly Func<T, Create, U> Degenerate = (value, create) => create((value, true), (value, true));


        public static IInterval<T> Union(in IInterval<T> a, in IInterval<T> b, Create create)
        {
            if (GetIntersection(a, b).IsIntersecting)
            {
                return create(a.Min(b), a.Max(b));
            }

            return Empty(create);
        }

        public static IInterval<T> Intersection(in IInterval<T> a, in IInterval<T> b, Create create)
        {
            var intersection = GetIntersection(a, b);
            var (a1, a2) = a.Deconstruct();
            var (b1, b2) = b.Deconstruct();

            return intersection switch
            {
                Disjoint => Empty(create),
                Intersecting and { Closure: Equal } => a,
                { IsSubset: true } => a,
                // if A is a superset of B then we're 'cutting a hole' into A
                { IsSuperset: true } => b,
                Intersecting and { Closure: Closure.AOverlapsLowerB } => create(a2, b1),
                Intersecting and { Closure: Closure.AOverlapsUpperB } => create(a1, b2),
                _ => throw new NotImplementedException($"Unsupported symmetric difference operation between {a} and {b}")
            };
        }

        public static SplitResult<U> Difference(in IInterval<T> a, in IInterval<T> b, Create create)
        {
            var intersection = GetIntersection(a, b);
            var (a1, a2) = a.Deconstruct();
            var (b1, b2) = b.Deconstruct();

            // Difference = A - B
            // where A could overlap or be placed anywhere over B
            return intersection switch
            {
                Intersecting and { Closure: Equal }
                // if A is a subset of B, then subtracting B leaves nothing but an empty set
                or { IsSubset: true } => new MonoSplitResult<U>(
                    Empty(create)
                ),
                // if A is a superset of B then we're 'cutting a hole' into A
                { IsSuperset: true } => new BiSplitResult<U>(
                    create(a1, (b1.Value, Exclusive)),
                    create((b2.Value, Exclusive), a2)
                ),
                Intersecting and { Closure: Closure.AOverlapsLowerB } => new MonoSplitResult<U>(
                    create(a1, (b1.Value, Exclusive))
                ),
                Intersecting and { Closure: Closure.AOverlapsUpperB } => new MonoSplitResult<U>(
                    create((b2.Value, Exclusive), a2)
                ),
                Disjoint => new MonoSplitResult<U>((U)a),
                _ => throw new NotImplementedException($"Unsupported difference operation between {a} and {b}")
            };
        }


        public static (IInterval<T> Lower, IInterval<T> Upper) SymmetricDifference(in IInterval<T> a, in IInterval<T> b, Create create)
        {
            var intersection = GetIntersection(a, b);
            var (a1, a2) = a.Deconstruct();
            var (b1, b2) = b.Deconstruct();


            // Symmetric Difference = A XOR B
            // where A could overlap or be placed anywhere over B
            return intersection switch
            {
                Intersecting and { Closure: Equal } => (Empty(create), Empty(create)),
                { IsSubset: true } => (
                    create(b1, (a1.Value, Exclusive)),
                    create((a2.Value, Exclusive), b2)
                ),
                // if A is a superset of B then we're 'cutting a hole' into A
                { IsSuperset: true } => (
                    create(a1, (b1.Value, Exclusive)),
                    create((b2.Value, Exclusive), a2)
                ),
                Intersecting and { Closure: Closure.AOverlapsLowerB } => (
                    create(a1, (b1.Value, Exclusive)),
                    create((a2.Value, Exclusive), b2)
                ),
                Intersecting and { Closure: Closure.AOverlapsUpperB } => (
                    create(b1, (a.Minimum, Exclusive)),
                    create((a2.Value, Exclusive), b2)
                ),
                Disjoint => (a, b),
                _ => throw new NotImplementedException($"Unsupported symmetric difference operation between {a} and {b}")
            };
        }

        public static bool Contains(in IInterval<T> a, T value)
        {
            return (a.Minimum.CompareTo(value), a.Maximum.CompareTo(value)) switch
            {
                (Greater, Less) => true,
                (Less, _) or (_, Greater) => false,
                (Equal, _) when a.Topology.IsMinimumInclusive() => true,
                (_, Equal) when a.Topology.IsMaximumInclusive() => true,
                _ => false,
            };
        }

        public static PartitionResult<IInterval<T>> Partition(in IInterval<T> a, T point, Create create)
        {
            if (a.Contains(point))
            {
                return new ValidPartition<IInterval<T>>(
                    new Interval<T>(a.Minimum, point, a.Topology.OpenMaximum()),
                    Degenerate(point, create),
                    new Interval<T>(point, a.Maximum, a.Topology.OpenMinimum())
                );
            }

            return new DisjointPartition<IInterval<T>>(Empty(create));
        }



    }


}