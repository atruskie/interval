namespace Math.Interval
{
    using static Math.Interval.Topology;
    using static Math.Interval.Comparison;
    using static Math.Interval.IntersectionDetails;
    using System;

    public static class IntervalExtensions
    {
        public static bool IntersectsWith(this double value, IInterval<double> interval) => interval.Contains(value);

        public static Interval WithTolerance(this double value, double tolerance)
        {
            return Interval.FromTolerance(value, tolerance);
        }

        public static (T Value, bool Inclusive) Min<T>(this IInterval<T> a, IInterval<T> b)
            where T : struct, IComparable<T>, IEquatable<T>, IFormattable
        {
            return a.CompareMinimums(b) switch
            {
                Equal or Less => (a.Minimum, a.Topology.IsMinimumInclusive()),
                Greater => (b.Minimum, b.Topology.IsMinimumInclusive()),
                _ => throw new InvalidOperationException()
            };
        }

        public static (T Value, bool Inclusive) Max<T>(this IInterval<T> a, IInterval<T> b)
            where T : struct, IComparable<T>, IEquatable<T>, IFormattable
        {
            return a.CompareMaximums(b) switch
            {
                Equal or Greater => (b.Maximum, b.Topology.IsMaximumInclusive()),
                Less => (a.Maximum, a.Topology.IsMaximumInclusive()),
                _ => throw new InvalidOperationException()
            };
        }

        



    }
}