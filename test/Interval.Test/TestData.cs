using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    using global::Math.Interval;
    using static global::Math.Interval.Topology;
    using static ResultExtensions;
    using static Double;
 
     public static class ResultExtensions {
        public static BiSplitResult<Interval> Bi(Interval a, Interval b) {
            return new BiSplitResult<Interval>(a, b);
        }
        public static MonoSplitResult<Interval> Mono(Interval a) {
            return new MonoSplitResult<Interval>(a);
        }
        public static DisjointPartition<Interval> Partition(Interval a) {
            return new DisjointPartition<Interval>(a);
        }
        public static ValidPartition<Interval> Partition(Interval a, Interval b, Interval c) {
            return new ValidPartition<Interval>(a, b ,c);
        }

        
    }
    public record TestData<T>
        where T : struct, IComparable<T>, IEquatable<T>, IFormattable
    {

        public TestData(string name, T minimum, T maximum, Topology topology, string normalizedString, string simplifiedString, params string[] parseStrings)
        {
            Name = name;
            Minimum = minimum;
            Maximum = maximum;
            Topology = topology;
            ParseStrings = parseStrings.Append(normalizedString).ToArray();
            NormalizedString = normalizedString;
            SimplifiedString = simplifiedString;
        }


        public string Name { get; init; }
        public T Minimum { get; init; }
        public T Maximum { get; init; }
        public Topology Topology { get; init; }
        public string NormalizedString { get; init; }
        public string SimplifiedString { get; init; }
        public string[] ParseStrings { get; init; }

        public static implicit operator Interval(TestData<T> value)
        {
            if (value is TestData<double> d)
            {
                return new Interval(d.Minimum, d.Maximum, d.Topology);
            }

            throw new NotImplementedException();
        }
    }

    public class IntervalTypes : TheoryData<Type>
    {
        public IntervalTypes()
        {
            Add(typeof(Interval));
            Add(typeof(Interval<int>));
        }
    }

    public static class TestData
    {
        // arbitrary value, but the smallest we choose to allow for some hypothetical scenario
        public const double Epsilon = 0.01;

        public static readonly TestData<double> Unit = new("Unit", 0.0, 1.0, Inclusive, "[0, 1]", "[0, 1]");
        public static readonly TestData<double> Empty = new("Empty", 0.0, 0.0, Exclusive, "(0, 0)", "∅", "∅");
        public static readonly TestData<double> EmptyNonZero = new("Empty (non-zero)", 5, 5, Exclusive, "(5, 5)", "∅", "(5,5)");
        public static readonly TestData<double> EmptyNonZeroHalfOpenA = new("Empty (non-zero) half-open a", 5, 5, Exclusive, "[5, 5)", "∅", "[5,5)");
        public static readonly TestData<double> EmptyNonZeroHalfOpenB = new("Empty (non-zero) half-open b", 5, 5, Exclusive, "(5, 5]", "∅", "(5,5]");
        public static readonly TestData<double> Open = new("Open", 0.0, 10.0, Exclusive, "(0, 10)", "(0, 10)");
        public static readonly TestData<double> Closed = new("Closed", 0.0, 10.0, Inclusive, "[0, 10]", "[0, 10]");
        public static readonly TestData<double> LeftClosedRightOpen = new("Left closed, right open", 0.0, 10.0, Topology.LeftClosedRightOpen, "[0, 10)", "[0, 10)");
        public static readonly TestData<double> LeftOpenRightClosed = new("Left open, right closed", 0.0, 10.0, Topology.LeftOpenRightClosed, "(0, 10]", "(0, 10]");
        public static readonly TestData<double> LeftClosedRightUnbounded = new("Left closed, right unbounded", 5, double.PositiveInfinity, Topology.Closed, "(5, ∞]", "≥5", ">=5", "≥5");
        public static readonly TestData<double> LeftOpenRightUnbounded = new("Left open, right unbounded", 5, double.PositiveInfinity, Topology.LeftOpenRightClosed, "[5, ∞]", ">5", ">5");
        public static readonly TestData<double> LeftUnboundedRightClosed = new("Left unbounded, right closed", double.NegativeInfinity, 5, Topology.Closed, "[-∞, 5)", "≤5", "<=5", "≤5");
        public static readonly TestData<double> LeftUnboundedRightOpen = new("Left unbounded, right open", double.NegativeInfinity, 5, Topology.LeftOpenRightClosed, "[-∞, 5]", "<5", "<5");
        public static readonly TestData<double> Unbounded = new("Unbounded", double.NegativeInfinity, double.PositiveInfinity, Topology.Closed, "(-∞, -∞)", "(-∞, -∞)", "[-∞, -∞]");
        public static readonly TestData<double> Degenerate0 = new("Degenerate 0", 0.0, 0.0, Inclusive, "[0, 0]", "0", "0");
        public static readonly TestData<double> DegenerateEpsilon = new("Degenerate epsilon", 0.0, 0.0, Inclusive, "[0.1, 0.1]", "0.1", "ε");
        public static readonly TestData<double> Degenerate333Recurring = new("Degenerate 3.33 recurring", 10.0 / 3.0, 10.0 / 3.0, Inclusive, "[3.3333333333333, 3.3333333333333]", "3.3333333333333", "3.3333333333333");
        public static readonly TestData<double> DegenerateInfinity = new("Degenerate infinity", double.PositiveInfinity, double.PositiveInfinity, Inclusive, "[∞, ∞]", "∞", "∞");
        public static readonly TestData<double> DegenerateNegativeInfinity = new("Degenerate -infinity", double.NegativeInfinity, double.NegativeInfinity, Inclusive, "[-∞, -∞]", "-∞", "-∞");
        public static readonly TestData<double> Tolerance = new("Tolerance", 2.5, 2.5, Inclusive, "[2.5, 7.5]", "[2.5, 7.5]", "5±2.5", "5+-2.5");
        public static readonly TestData<double> Approximation = new("Approximation", 4.75, 5.25, Inclusive, "[4.75, 5.25]", "[4.75, 5.25]", "≈5");
        public static readonly TestData<double> SameOrderOMagnitude = new("Same order of magnitude", 1, 10, MinimumInclusiveMaximumExclusive, "[1, 10)", "[1, 10)", "~5");
        public static readonly TestData<double> ThresholdWithEpsilon = new("Threshold with epsilon", Epsilon - Epsilon, Epsilon + Epsilon, Inclusive, "[0, 0.2]", "[0, 0.2]", "ε±ε");
        public static readonly TestData<double> ThresholdWithInfinity = new("Threshold with infinity", double.NegativeInfinity, double.PositiveInfinity, Topology.Closed, "(-∞, -∞)", "(-∞, -∞)", "123±∞");
        public static readonly TestData<double> IntervalWithEpsilon = new("Interval with epsilon", Epsilon, 5, MinimumExclusiveMaximumInclusive, "(0.1, 5]", "(-∞, -∞)", "(ε, 5]");
        public static readonly TestData<double> OrderOfMagnitudeWithEpsilon = new("Order of magnitude with epsilon", 0.1, 1, MinimumInclusiveMaximumExclusive, "[0.1, 1)", "[0.1, 1)", "~ε");

        public static readonly TheoryData<TestData<double>> DoubleCases = new()
        {
            Unit,
            Empty,
            EmptyNonZero,
            EmptyNonZeroHalfOpenA,
            EmptyNonZeroHalfOpenB,
            Open,
            Closed,
            LeftClosedRightOpen,
            LeftOpenRightClosed,
            LeftClosedRightUnbounded,
            LeftOpenRightUnbounded,
            LeftUnboundedRightClosed,
            LeftUnboundedRightOpen,
            Unbounded,
            Degenerate0,
            DegenerateEpsilon,
            Degenerate333Recurring,
            DegenerateInfinity,
            DegenerateNegativeInfinity,
            Tolerance,
            Approximation,
            SameOrderOMagnitude,
            ThresholdWithEpsilon,
            ThresholdWithInfinity,
            IntervalWithEpsilon,
            OrderOfMagnitudeWithEpsilon,
        };

        public static readonly HashSet<string> Empties = new()
        {
            Empty.Name,
            EmptyNonZero.Name,
            EmptyNonZeroHalfOpenA.Name,
            EmptyNonZeroHalfOpenB.Name,
        };

        public static readonly HashSet<string> Degenerates = new()
        {
            Degenerate0.Name,
            DegenerateEpsilon.Name,
            Degenerate333Recurring.Name,
            DegenerateInfinity.Name,
            DegenerateNegativeInfinity.Name,
        };

        public static readonly Interval[] OrderedList = new[] {
            Interval.Degenerate(double.NegativeInfinity),
            LeftUnboundedRightClosed,
            Interval.Empty,
            Interval.Degenerate(0),
            ((0, false), (1, false)),
            ((0, false), (1, true)),
            ((0, true), (1, false)),
            ((0, true), (1, true)),
            ((1, true), (1, true)),
            Interval.Degenerate(double.PositiveInfinity)
        };


        public static readonly TheoryData<Interval, Interval, IntersectionDetails> Sets = new()
        {
            { (1, 10), (1, 1), IntersectionDetails.ProperSubset },
            { (1, 10), (1, 10), IntersectionDetails.Subset },
            { (1, 10, Exclusive), (1, 10), IntersectionDetails.AOverlapsLowerB },
            { (-5, 10), (1, 10), IntersectionDetails.AOverlapsLowerB },
            { (-5, 15), (1, 10), IntersectionDetails.ProperSuperset },
            { (1, 10, Inclusive), (1, 10, Inclusive), IntersectionDetails.Superset },
            { (1, 10), (15, 19, Inclusive), IntersectionDetails.FullyBelow },
            { (1, 10), (-15, -19, Inclusive), IntersectionDetails.FullyAbove },
        };

        public static readonly TheoryData<Interval, Interval, Interval, SplitResult<Interval>, SplitResult<Interval>, BiSplitResult<Interval>, Interval> Unions = new()
        {
            // { a,     b,     union,   a diff b,            b diff a,             a symmdiff b,        intersection }
            { (0, 10), (1, 1), (0, 10), Bi((0, 1), (1, 10)), Mono(Interval.Empty), Bi((0, 1), (1, 10)), (1,1)  },
            { (1, 10), (5, 15), (1, 15), Mono((1, 5)),  Mono((10, 15)), Bi((1,5), (10, 15)), (5, 10) },
             { Interval.Empty, (5, 15), Interval.Empty, Mono(Interval.Empty), Mono((5, 15)), Bi(Interval.Empty, (5, 15)), Interval.Empty },
            { (1, 10), Interval.Degenerate(5), (1, 10), Bi((0, 5), (5, 10)), Mono( Interval.Degenerate(5)), Bi((0, 5), (5, 10)), Interval.Degenerate(5)  },
            { (1, 10, Exclusive), (10, 20, Exclusive), Interval.Empty, Mono((1,10,Exclusive)), Mono((10,20,Exclusive)), Bi((1,10,Exclusive), (10, 20, Exclusive)), Interval.Empty },
            { (1, 10, Exclusive), (10, 20, Inclusive), (1, 20, MinimumExclusiveMaximumInclusive), Mono((1, 10, Exclusive)),  Mono((10, 20, MinimumExclusiveMaximumInclusive)), Bi((1, 10, Exclusive), (10, 20, MinimumExclusiveMaximumInclusive)), Interval.Degenerate(10)  },
            { (20, 30, Exclusive), (10, 20, Inclusive),  (10, 30, MinimumExclusiveMaximumInclusive), Mono((20, 30, Exclusive)),  Mono((10, 20, MinimumInclusiveMaximumExclusive)), Bi((10, 20, MinimumInclusiveMaximumExclusive), (20, 30, Exclusive)), Interval.Degenerate(20) },
        };

        public static readonly TheoryData<Interval, double, bool, PartitionResult<Interval>> PointsInRanges = new()
        {
            { LeftClosedRightOpen, -10, false, Partition( Interval.Degenerate(-10) ) },
            { LeftClosedRightOpen, 0.0, true, Partition(Interval.EmptyAt(0), Interval.Degenerate(0), (0, 10, Exclusive)) },
            { LeftClosedRightOpen, 5.0, true, Partition((0, 5, MinimumInclusiveMaximumExclusive), Interval.Degenerate(5), (5, 10, Exclusive)) },
            { LeftClosedRightOpen, 10.0, false, Partition( Interval.Degenerate(10) )},
            { LeftClosedRightOpen, 20.0, false, Partition(Interval.Degenerate(20)) },
            { LeftOpenRightClosed, -10, false, Partition(Interval.Degenerate(-10)) },
            { LeftOpenRightClosed, 0.0, false, Partition( Interval.Degenerate(0))  },
            { LeftOpenRightClosed, 5.0, true, Partition((0, 5, Exclusive), Interval.Degenerate(5), (5, 10, MinimumExclusiveMaximumInclusive)) },
            { LeftOpenRightClosed, 10.0, true, Partition((0, 10, Exclusive), Interval.Degenerate(10), Interval.EmptyAt(10)) },
            { LeftOpenRightClosed, 20.0, false, Partition(Interval.Degenerate(20)) },
            { Empty, 0, false, Partition(Interval.Degenerate(0))},
            { EmptyNonZero, 5, false, Partition(Interval.Degenerate(5)) },
            { EmptyNonZeroHalfOpenA, 5.0, false, Partition(Interval.Degenerate(5))  },
            { EmptyNonZeroHalfOpenB, 5.0, false, Partition(Interval.Degenerate(5))  },
            { Degenerate0, 5.0, false, Partition(Interval.Degenerate(5))  },
            { Degenerate0, 0.0, true, Partition(Interval.EmptyAt(0), Interval.Degenerate(0), Interval.EmptyAt(0)) },
            { Unbounded, 0.0, true, Partition((NegativeInfinity, 0, Exclusive), Interval.Degenerate(0), (0, PositiveInfinity, Exclusive)) },
        };

         public static readonly TheoryData<Interval, SplitResult<Interval>> Complements = new()
         {
             {Unit, Bi((NegativeInfinity, 0, MinimumInclusiveMaximumExclusive), (1, PositiveInfinity, MinimumExclusiveMaximumInclusive)) },
             { Unbounded, Mono(Interval.Empty) },
             { LeftUnboundedRightOpen, Mono(5, PositiveInfinity, Inclusive) },
             { LeftOpenRightUnbounded, Mono(NegativeInfinity, 5, Inclusive) },
         };

        public static TestData<double> FindCase(string name) => DoubleCases
            .Select(x => x.First())
            .Cast<TestData<double>>()
            .Single(x => x.Name == name);

        public static readonly TheoryData<string, string> ParseErrors = new() {
            {"[∞, 0]", "The minimum must be less than the maximum"},
            {"[0, -∞]", "The maximum must be greater than the minimum"},
            {"", "Cannot be an empty"},
            {"", "cannot be null"},
            {"3±-1", "tolerance cannot be negative"},
            {"3±-1+3", "characters left over"},
            {"[3±1, 10]", "characters left over"},
            {"[3, 10]1", "characters left over"},
            {"[3, 10] ", "characters left over"},
            {"[3, 10]abc", "characters left over"},
            {"abc[3, 10]abc", "unknown interval format"},
            {"±1", "unknown interval format"},
            {"4≈1", "unknown interval format"},
            {"4ε1", "unknown interval format"},
            {"4~1", "unknown interval format"},
            {"4>1", "unknown interval format"},
            {"4≥1", "unknown interval format"},
            {"4>=1", "unknown interval format"},
            {"4<1", "unknown interval format"},
            {"4<=1", "unknown interval format"},
            {"4≤1", "unknown interval format"},
            {"4+", "unknown interval format"},
            {"4+-", "unknown interval format"},
            {"4±", "unknown interval format"},
        };

    }
}