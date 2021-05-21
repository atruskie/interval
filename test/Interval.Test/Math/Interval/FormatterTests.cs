
using Xunit;

namespace Test.Math.Interval
{
    using System.Globalization;
    using global::Math.Interval;

    public class FormatterTests
    {

        [Theory]
        [MemberData(nameof(TestData.DoubleCases), MemberType = typeof(TestData))]
        public void CanFormat(TestData<double> data) {
            Interval i = (data.Minimum, data.Maximum, data.Topology);

            Assert.Equal(data.NormalizedString, i.ToString());
            Assert.Equal(data.SimplifiedString, i.ToString(IntervalFormattingOptions.SimplifyExpression));
        }

        [Fact]
        public void TheEndpointFormatCanBeSpecified()
        {
            Interval i = (1/3.0, 2/3.0);

            Assert.Equal("[3.3333, 6.6666)", i.ToString(IntervalFormattingOptions.Default, "F4"));
        }

        [Fact]
        public void TheCultureCanBeSpecified()
        {
            Interval i = (5.55, 9.99);
            var culture = new CultureInfo("de-DE");
            Assert.Equal("[5,55, 9,99)", i.ToString(IntervalFormattingOptions.Default, "G", culture));
        }
    }
}