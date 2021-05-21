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


        public static double UnitNormalize(this Interval i, double value, bool clamp = true)
        {
            var v = (value - i.Minimum) / i.Range;
            return clamp ? Math.Clamp(v, Interval.Unit.Minimum, Interval.Unit.Maximum) : v;
        }

        public static Interval Scale(this Interval i, double value)
        {
            var middle = i.Minimum + i.Radius;
            var scaled = i.Range * value * 0.5;
            return (middle - scaled, middle + scaled, i.Topology);
        }

        public static Interval Extend(this Interval i, double value) => (i.Minimum - value, i.Maximum + value, i.Topology);
        public static Interval Shift(this Interval i, double value) => (i.Minimum + value, i.Maximum + value, i.Topology);


    }
}