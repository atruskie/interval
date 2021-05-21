
using System;
using static Math.Interval.Position;
using static Math.Interval.Comparison;

namespace Math.Interval
{
    internal static class Comparison
    {
        public const int Equal = 0;
        public const int Greater = 1;
        public const int Less = -1;
    }

    internal static class Containment {
        public const bool Exclusive = false;
        public const bool Inclusive = true;
    }

    public enum Position : short
    {
        Below = -1,
        Above = 1
    }

    public enum Closure : byte
    {
        // sorted in order of desired comparable order
        ProperSuperset,
        Superset,
        AOverlapsLowerB,
        Equal,
        Subset,
        ProperSubset,
        AOverlapsUpperB
    }


    /// <summary>
    /// Describes the various conditions that can occur with two possibly overlapping intervals.
    /// </summary>
    public abstract record IntersectionDetails
    {
        public static readonly IntersectionDetails Equivalent = new Intersecting(Closure.Equal);
        public static readonly IntersectionDetails FullyBelow = new Disjoint(Below);
        public static readonly IntersectionDetails FullyAbove = new Disjoint(Above);
        public static readonly IntersectionDetails Subset = new Intersecting(Closure.Subset);
        public static readonly IntersectionDetails ProperSubset = new Intersecting(Closure.ProperSubset);
        public static readonly IntersectionDetails Superset = new Intersecting(Closure.Superset);
        public static readonly IntersectionDetails ProperSuperset = new Intersecting(Closure.ProperSuperset);
        public static readonly IntersectionDetails AOverlapsLowerB = new Intersecting(Closure.AOverlapsLowerB);
        public static readonly IntersectionDetails AOverlapsUpperB = new Intersecting(Closure.AOverlapsUpperB);

        public static IntersectionDetails GetIntersection<T>(IInterval<T> a, IInterval<T> b)
        where T : struct, IComparable<T>, IEquatable<T>, IFormattable
        {
            var (ta, tb) = (a.Topology, b.Topology);
            int a1b1 = a.Minimum.CompareTo(b.Minimum),
                a1b2 = a.Minimum.CompareTo(b.Maximum),
                a2b1 = a.Maximum.CompareTo(b.Minimum),
                a2b2 = a.Maximum.CompareTo(b.Maximum);

            if (a2b1 is Less)
            {
                // A  B         non-overlapping
                return FullyBelow;
            }

            if (a2b1 is Greater)
            {
                // A1--B1==A2--B2   A and B overlap
                return AOverlapsLowerB;
            }

            if (a1b2 is Less)
            {
                // B1--A1==B2--A2   B and A overlap
                return AOverlapsUpperB;
            }

            if (a1b2 is Greater)
            {
                // B  A         non-overlapping
                return FullyAbove;
            }

            if (a1b1 is Less && a2b2 is Greater)
            {
                // A1--B--A2      A wholly contains B
                return ProperSuperset;
            }

            if (a1b1 is Greater && a2b2 is Less)
            {
                // B1--A--B2      B wholly contains A
                return ProperSubset;
            }

            // now deal with endpoints that touch
            // AB         touching, possibly overlapping
            if (a2b1 is Equal)
            {
                return ta.IsAsymmetricallyCompatible(tb) ? AOverlapsLowerB : FullyBelow;
            }

            // BA         touching, possibly overlapping
            if (a1b2 is Equal)
            {
                return tb.IsAsymmetricallyCompatible(ta) ? AOverlapsUpperB : FullyAbove;
            }

            // A1B1--B2--A2 low touching, superset?
            if (a1b1 is Equal && ta.IsMinimumEqual(tb) && a2b2 is Greater)
            {
                return Superset;
            }

            // A1--B1--B2A2 high touching, superset?
            if (a2b2 is Equal && ta.IsMaximumEqual(tb) && a1b1 is Less)
            {
                return Superset;
            }

            // A1B1--A2B2  endpoints aligned, possibly overlapping?
            if (a1b1 is Equal && a2b2 is Equal)
            {
                return (ta.IsMinimumCompatible(tb), ta.IsMaximumCompatible(tb)) switch
                {
                    (true, true) => Equivalent,
                    (true, false) => Subset,
                    (false, true) => Subset,
                    (false, false) => ProperSubset,
                };
            }

            // no other cases should be possible
            throw new InvalidOperationException($"An unexpected interval intersection case was found: A:{a}, B:{b}");
        }


        public bool IsFullyBelow => this is Disjoint d && d.Position == Below;
        public bool IsFullyAbove => this is Disjoint d && d.Position == Above;
        public bool IsSubset => this is Intersecting i && i.Closure is Closure.Subset or Closure.ProperSubset;
        public bool IsProperSubset => this is Intersecting i && i.Closure == Closure.ProperSubset;
        public bool IsSuperset => this is Intersecting i && i.Closure is Closure.Superset or Closure.ProperSuperset;
        public bool IsProperSuperset => this is Intersecting i && i.Closure == Closure.ProperSuperset;
        public bool IsIntersecting => this is Intersecting;
        public bool IsDisjoint => this is Disjoint;
    };

    public record Intersecting : IntersectionDetails
    {
        public Closure Closure { get; }
        internal Intersecting(Closure closure) : base()
        {
            Closure = closure;
        }
    }

    public record Disjoint : IntersectionDetails
    {
        public Position Position { get; }
        internal Disjoint(Position position) : base()
        {
            Position = position;
        }
    }
}