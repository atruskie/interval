namespace Interval.Test.Math.Interval
{

    using Xunit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Math.Interval;
    using static TestData;
    
    public class IntervalExtensionsTests
    {



        [Theory]
        [InlineData(nameof(Unit), 0.5, 0.5)]
        [InlineData(nameof(Unit), 1, 1)]
        [InlineData(nameof(Unit), 0.0, 0.0)]
        public void UnitNormalizeTest(string name, double value, double expected)
        {
            Interval i = FindCase(name);

            Assert.Equal(expected, i.Center);
        }
    }
}