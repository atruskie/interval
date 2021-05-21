
using Xunit;

namespace Test.Math.Interval
{
    using System;
    using global::Math.Interval;

    public class ParserTests
    {

        [Theory]
        [MemberData(nameof(TestData.DoubleCases), MemberType = typeof(TestData))]
        public void CanParse(TestData<double> data) {
            Interval expected = (data.Minimum, data.Maximum, data.Topology);

            foreach(var @string in data.ParseStrings) {
                var actual = Interval.Parse(@string);
                Assert.Equal(expected, actual);

                Assert.True(Interval.TryParse(@string, out actual, out var error));
                Assert.Equal(expected, actual);
                Assert.Null(error);
            }
        }

        [Theory]
        [MemberData(nameof(TestData.ParseErrors), MemberType = typeof(TestData))]
        public void CanHandleParsingErrors(string @case, string expectedError) {

            Assert.False(Interval.TryParse(@case, out var value, out var error));
            Assert.Equal(default, value);
            Assert.Contains(expectedError, error);

            var ex = Assert.Throws<ArgumentException>(
                () => Interval.Parse(@case)
            );
            Assert.Contains(ex.Message, expectedError);
        }
    }
}