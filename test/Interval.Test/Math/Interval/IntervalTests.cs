

namespace Test.Math.Interval
{
    using Xunit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Math.Interval;
    using static TestData;


    public class IntervalTests
    {
        [Fact]
        public void DefaultTest()
        {
            var i = new Interval();
            Assert.Equal(Topology.Default, i.Topology);
            Assert.Equal(0.0, i.Minimum);
            Assert.Equal(0.0, i.Maximum);
            Assert.True(i.IsEmpty);
        }

        [Fact]
        public void ConstructorDefaultTopologyTest()
        {
            var i = new Interval(1, 10);
            Assert.Equal(Topology.Default, i.Topology);
            Assert.Equal(1.0, i.Minimum);
            Assert.Equal(10.0, i.Maximum);
        }

        [Fact]
        public void ConstructorFromTuplesTest()
        {
            var i = new Interval((1, false), (10, true));

            Assert.Equal(Topology.Default, i.Topology);
            Assert.Equal(1.0, i.Minimum);
            Assert.Equal(10.0, i.Maximum);
        }

        [Fact]
        public void MinMustBeLessThanMax() {
            Assert.Throws<ArgumentException>(() => new Interval(10, 1));
            Assert.Throws<ArgumentException>(() => new Interval(double.PositiveInfinity, 1));
        }

        [Fact]
        public void NeitherValueCanBeNaN() {
            Assert.Throws<ArgumentException>(() => new Interval(double.NaN, 1));
            Assert.Throws<ArgumentException>(() => new Interval(1, double.NaN));
        }

        [Fact]
        public void IsStructurallyEquivalentTest()
        {
            Interval a = (1, 10, Topology.Inclusive);
            Interval b = (1, 10, Topology.Inclusive);

            Assert.Equal(a, b);
            Assert.Equal(a, a);
            Assert.Equal(b, b);
            Assert.True(a.Equals(b));
            Assert.True(b.Equals(a));
            Assert.True(a == b);
            Assert.True(b == a);

            IInterval<double> c = a;
            IInterval<double> d = b;

            Assert.Equal(c, d);
            Assert.Equal(c, c);
            Assert.Equal(d, d);
            Assert.True(c.Equals(d));
            Assert.True(d.Equals(c));
        }

        [Theory]
        [MemberData(nameof(DoubleCases), MemberType = typeof(TestData))]
        public void ImplicitCastFromTupleTest(TestData<double> data)
        {
            Interval actual = new(data.Minimum, data.Maximum, data.Topology);
            Interval expected = (data.Minimum, data.Maximum, data.Topology);

            Assert.Equal(actual, expected);
        }

        [Theory]
        [MemberData(nameof(DoubleCases), MemberType = typeof(TestData))]
        public void DestructuringTest(TestData<double> data)
        {
            Interval actual = new(data.Minimum, data.Maximum, data.Topology);

            var (minimum, maximum, topology) = actual;
            Assert.Equal(data.Minimum, minimum);
            Assert.Equal(data.Maximum, maximum);
            Assert.Equal(data.Topology, topology);
        }

        [Theory]
        [MemberData(nameof(DoubleCases), MemberType = typeof(TestData))]
        public void DestructuringTestAlternate(TestData<double> data)
        {
            Interval actual = new(data.Minimum, data.Maximum, data.Topology);

            var (minimum, maximum) = actual;
            Assert.Equal(data.Minimum, minimum.Value);
            Assert.Equal(data.Maximum, maximum.Value);
            Assert.Equal(data.Topology.IsMinimumInclusive(), minimum.Inclusive);
            Assert.Equal(data.Topology.IsMaximumInclusive(), maximum.Inclusive);
        }

        [Theory]
        [InlineData(nameof(Unit), 0.5)]
        [InlineData(nameof(Empty), 0.0)]
        [InlineData(nameof(EmptyNonZeroHalfOpenA), 5.0)]
        [InlineData(nameof(Open), 5.0)]
        [InlineData(nameof(Closed), 5.0)]
        [InlineData(nameof(Unbounded), 0)]
        [InlineData(nameof(LeftUnboundedRightClosed), double.NegativeInfinity)]
        [InlineData(nameof(LeftOpenRightUnbounded), double.PositiveInfinity)]
        public void CenterTest(string name, double expected)
        {
            Interval i = FindCase(name);

            Assert.Equal(expected, i.Center);
        }

        [Theory]
        [InlineData(nameof(Unit), 0.5)]
        [InlineData(nameof(Empty), 0.0)]
        [InlineData(nameof(EmptyNonZeroHalfOpenA), 5.0)]
        [InlineData(nameof(Open), 5.0)]
        [InlineData(nameof(Closed), 5.0)]
        [InlineData(nameof(Unbounded), 0)]
        [InlineData(nameof(LeftUnboundedRightClosed), 5)]
        [InlineData(nameof(LeftOpenRightUnbounded), 5)]
        public void AnchorTest(string name, double expected)
        {
            Interval i = FindCase(name);

            Assert.Equal(expected, i.Center);
        }



        [Theory]
        [MemberData(nameof(DoubleCases), MemberType = typeof(TestData))]
        public void EmptyTest(TestData<double> data)
        {
            Interval actual = data;


            Assert.Equal(Empties.Contains(data.Name), actual.IsEmpty);
            Assert.Equal(Empties.Contains(data.Name), actual.IsDegenerate);
            Assert.Equal(Empties.Contains(data.Name), actual.IsProper);
        }

        [Theory]
        [MemberData(nameof(DoubleCases), MemberType = typeof(TestData))]
        public void DegenerateTest(TestData<double> data)
        {
            Interval actual = data;

            // if degenerate, then it is not empty
            Assert.Equal(!Degenerates.Contains(data.Name), actual.IsEmpty);
            Assert.Equal(Degenerates.Contains(data.Name), actual.IsDegenerate);
            Assert.Equal(Degenerates.Contains(data.Name), actual.IsProper);
        }

        [Theory]
        [MemberData(nameof(DoubleCases), MemberType = typeof(TestData))]
        public void InteriorTest(TestData<double> data)
        {
            Interval i = data;
            var interior = i.Interior();

            Assert.False(interior.IsMinimumInclusive);
            Assert.False(interior.IsMaximumInclusive);

            Assert.Equal(new Interval(data.Minimum, data.Maximum, Topology.Exclusive), interior);

        }

        [Theory]
        [MemberData(nameof(DoubleCases), MemberType = typeof(TestData))]
        public void RangeAndRadiusTest(TestData<double> data)
        {
            Interval i = data;

            Assert.Equal(data.Maximum - data.Minimum, i.Range);
            Assert.Equal(Math.Abs(data.Minimum - data.Maximum) / 2, i.Radius);
        }

        [Theory]
        [MemberData(nameof(DoubleCases), MemberType = typeof(TestData))]
        public void BoundedTests(TestData<double> data)
        {
            Interval i = data;

            if (data.Name == LeftClosedRightUnbounded.Name || data.Name == LeftOpenRightUnbounded.Name)
            {
                Assert.False(i.IsBounded);
                Assert.True(i.IsLeftBounded);
                Assert.False(i.IsRightBounded);
            }
            else if (data.Name == LeftUnboundedRightOpen.Name || data.Name == LeftUnboundedRightClosed.Name)
            {
                Assert.False(i.IsBounded);
                Assert.False(i.IsLeftBounded);
                Assert.False(i.IsRightBounded);
            }
            else if (data.Name == Unbounded.Name)
            {
                Assert.False(i.IsBounded);
                Assert.False(i.IsLeftBounded);
                Assert.False(i.IsRightBounded);
            }
            else
            {
                Assert.True(i.IsBounded);
                Assert.True(i.IsLeftBounded);
                Assert.True(i.IsRightBounded);
            }
        }

        [Fact]
        public void UnboundedIsClosedAndOpenSimultaneously()
        {
            Interval i = Unbounded;


            Assert.True(i.IsMinimumInclusive);
            Assert.True(i.IsMaximumInclusive);

            // the interior (exclusive endpoints) should still produce an interval
            // with inclusive endpoints when the endpoints are infinite
            var interior = i.Interior();
            Assert.True(interior.IsMinimumInclusive);
            Assert.True(interior.IsMaximumInclusive);
        }

        [Fact]
        public void CompareMinimumsTest()
        {
            var max = (10, true);
            var intervals = new Interval[] {
                ((-1, false), max),
                ((-1, true), max),
                ((0, false), max),
                ((0, true), max),
                ((1, true), max),
                ((1, false), max),
            };

            // every item in the intervals list should have a minimum that is less that all successors
            for (int i = 0; i < intervals.Length - 1; i++)
            {
                var current = intervals[i];
                var next = intervals[i + 1];

                Assert.Equal(-1, current.CompareMinimums(next));
                Assert.Equal(0, current.CompareMinimums(current));
            }
        }

        [Fact]
        public void CompareMaximumsTest()
        {
            var min = (0, true);
            var intervals = new Interval[] {
                (min, (9, false)),
                (min, (9, true)),
                (min, (10, false)),
                (min, (10, true)),
                (min, (11, true)),
                (min, (11, false)),
            };

            // every item in the intervals list should have a maximum that is less that all successors
            for (int i = 0; i < intervals.Length - 1; i++)
            {
                var current = intervals[i];
                var next = intervals[i + 1];

                Assert.Equal(-1, current.CompareMaximums(next));
                Assert.Equal(0, current.CompareMinimums(current));
            }
        }

        [Fact]
        public void IComparerBoxedTest()
        {
            Assert.Equal(1, Interval.Unit.CompareTo(null));
        }

        [Fact]
        public void IEquatableBoxedTest()
        {
            Interval unit = Unit;
            Assert.False(unit.Equals(new object()));
            Assert.False(unit.Equals(null));
            Assert.True(unit.Equals((object)unit));
        }

        [Fact]
        public void HashCodeTest()
        {
            Interval unit = Unit;
            Interval unbounded = Unbounded;
            Assert.Equal(unit.GetHashCode(), new Interval(0, 1, Topology.Default).GetHashCode());
            Assert.NotEqual(unit.GetHashCode(), unbounded.GetHashCode());
        }

        [Fact]
        public void IEquatableIIntervalBoxedTest()
        {
            Interval unit = Unit;
            IInterval<double> unit2 = Interval.Unit;
            IInterval<double> boxedNull = null;

            Assert.False(unit.Equals(boxedNull));
            Assert.True(unit.Equals(unit2));
        }

        [Fact]
        public void IEquatableIntervalTest()
        {
            Interval unit = Unit;
            Interval unbounded = Unbounded;
            Interval unit2 = Interval.Unit;


            Assert.False(unit.Equals(unbounded));
            Assert.True(unit.Equals(unit2));
        }

        [Fact]
        public void CompareToTest()
        {
            var intervals = OrderedList;

            // every item in the intervals list should be less than all successors
            for (int i = 1; i < intervals.Length - 1; i++)
            {
                var previous = intervals[i - 1];
                var current = intervals[i];
                var next = intervals[i + 1];

                Assert.Equal(-1, current.CompareMinimums(next));
                Assert.Equal(0, current.CompareMinimums(current));
                Assert.Equal(1, current.CompareTo(previous));
            }
        }

        [Fact]
        public void OperatorsTest()
        {
            var intervals = OrderedList;

            // every item in the intervals list should be less than all successors
            for (int i = 1; i < intervals.Length - 1; i++)
            {
                var previous = intervals[i - 1];
                var current = intervals[i];
                var next = intervals[i + 1];

                Assert.True(current == current);
                Assert.False(current == next);
                Assert.False(current == previous);

                Assert.False(current != current);
                Assert.True(current != next);
                Assert.True(current != previous);

                Assert.True(current <= current);
                Assert.True(current <= next);
                Assert.False(current <= previous);

                Assert.False(current < current);
                Assert.True(current < next);
                Assert.False(current < previous);

                Assert.True(current >= current);
                Assert.False(current >= next);
                Assert.True(current >= previous);

                Assert.False(current > current);
                Assert.False(current > next);
                Assert.True(current > previous);
            }
        }

        [Fact]
        public void ImplicitCastTest() {
            Interval i1 = Unit;
            Interval i2 = (0, 1);
            Interval i3 = (0,1, Topology.Default);
            Interval i4 = ((0, true), (1, false));

            Assert.Equal(i1, i2);
            Assert.Equal(i1, i3);
            Assert.Equal(i1, i4);
        }

        
        [Theory]
        [MemberData(nameof(PointsInRanges), MemberType = typeof(TestData))]
        public void ContainsTest(string name, double test, bool expected)
        {
            Interval i = FindCase(name);

            Assert.Equal(expected, i.Contains(test));
        }



        [Theory]
    
        [MemberData(nameof(Sets), MemberType = typeof(TestData))]
        public void IsProperSubsetTest(Interval a, Interval b, IntersectionDetails intersection)
        {
            var result = a.IsProperSubset(b);
            if (intersection.IsProperSubset) {
                Assert.True(result);
                Assert.True(a.IsSubset(b));
                Assert.True(a.IntersectsWith(b));
            }
            else {
                Assert.False(result);
                Assert.False(a.IsSuperset(b));
            }
        }


        [Theory]
    
        [MemberData(nameof(Sets), MemberType = typeof(TestData))]
        public void IsProperSupersetTest(Interval a, Interval b, IntersectionDetails intersection)
        {
            var result = a.IsProperSuperset(b);
            if (intersection.IsProperSuperset) {
                Assert.True(result);
                Assert.True(a.IsSuperset(b));
                Assert.True(a.IntersectsWith(b));
            }
            else {
                Assert.False(result);
                Assert.False(a.IsSubset(b));
            }
        }

        [Theory]
        [MemberData(nameof(Sets), MemberType = typeof(TestData))]
        public void ISubsetTest(Interval a, Interval b, IntersectionDetails intersection)
        {
            var result = a.IsSubset(b);
            if (intersection.IsSubset) {
                Assert.True(result);
                Assert.True(a.IntersectsWith(b));
            }
            else {
                Assert.False(result);
                Assert.False(a.IsSuperset(b));
            }
        }

        
        [Theory]
        [MemberData(nameof(Sets), MemberType = typeof(TestData))]
        public void IsSupersetTest(Interval a, Interval b, IntersectionDetails intersection)
        {
            var result = a.IsSuperset(b);
            if (intersection.IsProperSuperset) {
                Assert.True(result);
                Assert.True(a.IntersectsWith(b));
            }
            else {
                Assert.False(result);
                Assert.False(a.IsSubset(b));

            }
        }

        [Theory]
        [MemberData(nameof(Sets), MemberType = typeof(TestData))]
        public void IsIntersectingTest(Interval a, Interval b, IntersectionDetails intersection)
        {
            var result = a.IntersectsWith(b);
            if (intersection is Intersecting) {
                Assert.True(result);
            }
            else {
                Assert.False(result);
            }
        }

        [Theory]
        [MemberData(nameof(Unions), MemberType = typeof(TestData))]
        public void UnionTest(Interval a, Interval b, Interval expected, SplitResult<Interval> _,  SplitResult<Interval> _, BiSplitResult<Interval> _)
        {
            Assert.Equal(expected, a.Union(b));
            Assert.Equal(expected, b.Union(a));
            Assert.Equal(a, a.Union(a));
        }

        [Theory]
        [MemberData(nameof(Unions), MemberType = typeof(TestData))]
        public void DifferenceTest(Interval a, Interval b, Interval _, SplitResult<Interval> expectedA,  SplitResult<Interval> expectedB, BiSplitResult<Interval> _)
        {
            Assert.Equal(expectedA, a.Difference(b));
            Assert.Equal(expectedB, b.Difference(a));
            Assert.Equal(new MonoSplitResult<Interval>(Interval.Empty), a.Difference(a));
        }

        [Theory]
        [MemberData(nameof(Unions), MemberType = typeof(TestData))]
        public void SymmetricDifferenceTest(Interval a, Interval b, Interval _, SplitResult<Interval> _,  SplitResult<Interval> _, BiSplitResult<Interval> expected)
        {
            Assert.Equal(expected, a.SymmetricDifference(b));
            Assert.Equal(expected, b.SymmetricDifference(a));
        }

        [Theory]
        [MemberData(nameof(Unions), MemberType = typeof(TestData))]
        public void IntersectionTest(Interval a, Interval b, Interval _, SplitResult<Interval> _,  SplitResult<Interval> _, BiSplitResult<Interval> _, Interval expected)
        {
            Assert.Equal(expected, a.Intersection(b));
            Assert.Equal(expected, b.Intersection(a));
            Assert.Equal(a, a.Intersection(a));
        }

              
        [Theory]
        [MemberData(nameof(PointsInRanges), MemberType = typeof(TestData))]
        public void PartitionTest(Interval a, double point, bool _, PartitionResult<Interval> expected) 
        {
            Assert.Equal(expected, a.Partition(point));
        }

        [Theory]
        [MemberData(nameof(Complements), MemberType = typeof(TestData))]
        public void ComplementTest(Interval i, SplitResult<Interval> expected)
        {
            var actual = i.Complement();
            Assert.Equal(expected, actual);

            if (expected is MonoSplitResult<Interval> m) {
                var actual2 = m.Result.Complement();
                Assert.IsType<MonoSplitResult<Interval>>(actual2);
                Assert.Equal(i, ((MonoSplitResult<Interval>)actual2).Result);
            }
        }

    }
}