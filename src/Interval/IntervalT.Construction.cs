
namespace Math.Interval
{
    using static Math.Interval.Topology;
    using static Math.Interval.Comparison;
    using static Math.Interval.IntersectionDetails;
    using System.Collections.Generic;

    public partial struct Interval<T>
    {
        public static readonly IntervalHelpers<T, Interval<T>>.Create Create 
           = (in (T Value, bool Inclusive) minimum, in (T Value, bool Inclusive) maximum)
           => new Interval<T>(minimum, maximum);


        public readonly static Interval<T> EmptyDefault = EmptyAt(default);

        public static Interval<T> Degenerate(T value)
        {
            return new Interval<T>()
            {
                Maximum = value,
                Minimum = value,
                // Topology must be inclusive otherwise it represents an empty set.
                // i.e. with default topology [,) the result is x <= x < x - which is always false, i.e. an empty set
                Topology = Inclusive,
            };
        }

        public static Interval<T> EmptyAt(T value)
        {
            return new Interval<T>()
            {
                Maximum = value,
                Minimum = value,
                // anything other than Inclusive is Empty (as per IsEmpty below)
                Topology = Exclusive,
            };
        }

        /// <summary>
        /// The closure of I is the smallest closed interval that contains I;
        /// which is also the set I augmented with its finite endpoints.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>An interval that covers the range of values in the input vector</returns>
        public static Interval<T> ClosureOf(IEnumerable<T> vector, T floorT, T ceilingT)
        {
            T min = ceilingT,
              max = floorT;

            foreach (var scalar in vector)
            {
                if (scalar.CompareTo(min) is Less)
                {
                    min = scalar;
                }
                else if (scalar.CompareTo(max) is Greater)
                {
                    max = scalar;
                }
            }

            return new Interval<T>()
            {
                Maximum = max,
                Minimum = min,
                Topology = Closed,
            };
        }
    }
}