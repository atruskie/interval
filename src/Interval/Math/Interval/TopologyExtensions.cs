namespace Math.Interval
{
    using static Topology;
    public static class TopologyExtensions
    {
        /// <summary>
        /// Tests if this topology has an inclusive lower bound.
        /// </summary>
        /// <param name="topology">The value to test.</param>
        /// <returns><c>true</c> is the lower bound is inclusive.</returns>
        public static bool IsMinimumInclusive(this Topology topology)
        {
            return topology is MinimumInclusiveMaximumExclusive or Inclusive;
        }

        /// <summary>
        /// Tests if this topology has an inclusive upper bound.
        /// </summary>
        /// <param name="topology">The value to test.</param>
        /// <returns><c>true</c> is the upper bound is inclusive.</returns>
        public static bool IsMaximumInclusive(this Topology topology)
        {
            return topology is MinimumExclusiveMaximumInclusive or Inclusive;
        }

        /// <summary>
        /// Determines if two adjacent topologies would allow their endpoints to be
        /// equal if the endpoints had the same value.
        /// </summary>
        /// <param name="lower">The topology of the interval that is lower than the other.</param>
        /// <param name="upper">The topology of the interval that is greater than the other.></param>
        /// <returns></returns>
        public static bool IsAsymmetricallyCompatible(this Topology lower, Topology upper)
        {
            return lower.IsMaximumInclusive() || upper.IsMinimumInclusive();
        }

        public static bool IsMinimumCompatible(this Topology a, Topology b)
        {
            return a.IsMinimumInclusive() || b.IsMinimumInclusive();
        }

        public static bool IsMaximumCompatible(this Topology a, Topology b)
        {
            return a.IsMaximumInclusive() || b.IsMaximumInclusive();
        }

        public static bool IsMinimumEqual(this Topology a, Topology b)
        {
            return (a.IsMinimumInclusive(), b.IsMinimumInclusive()) switch
            {
                (true, true) or (false, false) => true,
                _ => false
            };
        }
        public static bool IsMaximumEqual(this Topology a, Topology b)
        {
            return (a.IsMaximumInclusive(), b.IsMaximumInclusive()) switch
            {
                (true, true) or (false, false) => true,
                _ => false
            };
        }

        public static Topology Create(bool minimumInclusive, bool maximumInclusive) {
            return (minimumInclusive, maximumInclusive) switch {
                (true, true) => Inclusive,
                (false, false) => Exclusive,
                (true, false) =>MinimumInclusiveMaximumExclusive,
                (false, true) => MinimumExclusiveMaximumInclusive,
            };
        }

        public static (bool MinimumInclusive, bool MaximumInclusive) Deconstruct(this Topology t) {
            return (t.IsMinimumInclusive(), t.IsMaximumInclusive());
        }

        /// <summary>
        /// Combines the two topologies, taking the most inclusive bound from each.
        /// The lower bound is taken from the first argument, the upper from the second.
        /// </summary>
        /// <param name="a">The topology to extract the lower bound from.</param>
        /// <param name="b">The topology to extract the upper bound from.</param>
        /// <returns></returns>
        public static Topology Combine(this Topology a, Topology b)
        {
            return Create(a.IsMinimumInclusive(), b.IsMaximumInclusive());
        }

        /// <summary>
        /// Transforms the given topology so that it's lower bound becomes included.
        /// Leaves the upper bound unchanged.
        /// </summary>
        /// <param name="topology">The value to transform.</param>
        /// <returns>A topology with an inclusive lower bound.</returns>
        public static Topology CloseMinimum(this Topology topology)
        {
            return topology switch
            {
                MinimumInclusiveMaximumExclusive or Inclusive => topology,
                MinimumExclusiveMaximumInclusive => Inclusive,
                Exclusive => MinimumInclusiveMaximumExclusive,
                _ => throw new System.InvalidOperationException(),
            };
        }

        /// <summary>
        /// Transforms the given topology so that it's upper bound becomes included.
        /// Leaves the lower bound unchanged.
        /// </summary>
        /// <param name="topology">The value to transform.</param>
        /// <returns>A topology with an inclusive upper bound.</returns>
        public static Topology CloseMaximum(this Topology topology)
        {
            return topology switch
            {
                MinimumExclusiveMaximumInclusive or Inclusive => topology,
                MinimumInclusiveMaximumExclusive => Inclusive,
                Exclusive => MinimumExclusiveMaximumInclusive,
                _ => throw new System.InvalidOperationException(),
            };
        }

        /// <summary>
        /// Transforms the given topology so that it's lower bound becomes excluded.
        /// Leaves the upper bound unchanged.
        /// </summary>
        /// <param name="topology">The value to transform.</param>
        /// <returns>A topology with an exclusive lower bound.</returns>
        public static Topology OpenMinimum(this Topology topology)
        {
            return topology switch
            {
                MinimumExclusiveMaximumInclusive or Exclusive => topology,
                MinimumInclusiveMaximumExclusive => Exclusive,
                Inclusive => MinimumExclusiveMaximumInclusive,
                _ => throw new System.InvalidOperationException(),
            };
        }

        /// <summary>
        /// Transforms the given topology so that it's upper bound becomes excluded.
        /// Leaves the lower bound unchanged.
        /// </summary>
        /// <param name="topology">The value to transform.</param>
        /// <returns>A topology with an exclusive upper bound.</returns>
        public static Topology OpenMaximum(this Topology topology)
        {
            return topology switch
            {
                MinimumInclusiveMaximumExclusive or Exclusive => topology,
                MinimumExclusiveMaximumInclusive => Exclusive,
                Inclusive => MinimumInclusiveMaximumExclusive,
                _ => throw new System.InvalidOperationException(),
            };
        }

    }
}