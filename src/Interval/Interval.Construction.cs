namespace Math.Interval
{
    using System.Collections.Generic;
    using static Math.Interval.Topology;

    public partial struct Interval
    {
        public static readonly IntervalHelpers<double, Interval>.Create Create 
            = (in (double Value, bool Inclusive) minimum, in (double Value, bool Inclusive) maximum)
            => new Interval(minimum, maximum);

        public readonly static Interval EmptyDefault = EmptyAt(default);

        public static Interval Degenerate(double value)
        {
            return new Interval()
            {
                Maximum = value,
                Minimum = value,
                // Topology must be inclusive otherwise it represents an empty set.
                // i.e. with default topology [,) the result is x <= x < x - which is always false, i.e. an empty set
                Topology = Inclusive,
            };
        }

        public static Interval EmptyAt(double value)
        {
            return new Interval()
            {
                Maximum = value,
                Minimum = value,
                // anything other than Inclusive is Empty (as per IsEmpty below)
                Topology = Exclusive,
            };
        }

        public static Interval FromTolerance(double value, double tolerance)
        {
            return new Interval()
            {
                Minimum = value - tolerance,
                Maximum = value + tolerance,
                Topology = Inclusive
            };
        }

        public static Interval Approximation(double value)
        {
            var fivePercent = value * 0.05;
            return new Interval()
            {
                Minimum = value - fivePercent,
                Maximum = value + fivePercent,
                Topology = Inclusive
            };
        }

        public static Interval SameOrderOfMagnitudeAs(double value)
        {
            return new Interval()
            {
                Minimum = System.Math.Pow(value, 0.1),
                Maximum = System.Math.Pow(value, 10),
                Topology = Inclusive
            };
        }

        /// <summary>
        /// The closure of I is the smallest closed interval that contains I;
        /// which is also the set I augmented with its finite endpoints.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>An interval that covers the range of values in the input vector</returns>
        public static Interval ClosureOf(IEnumerable<double> vector)
        {
            double min = double.PositiveInfinity,
                max = double.NegativeInfinity;

            foreach (var scalar in vector)
            {
                if (scalar < min)
                {
                    min = scalar;
                }
                else if (scalar > max)
                {
                    max = scalar;
                }
            }

            return new Interval()
            {
                Maximum = max,
                Minimum = min,
                Topology = Closed,
            };
        }

    }
}