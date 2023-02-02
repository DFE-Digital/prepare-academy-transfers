using Xunit;

namespace Dfe.PrepareTransfers.Helpers.Tests
{
    public class PercentageHelperTests
    {
        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", null)]
        [InlineData(null, "0", null)]
        [InlineData(null, "20", "0.0%")]
        [InlineData("0", null, null)]
        [InlineData("0", "0", null)]
        [InlineData("10", "20", "50.0%")]
        public void GivenTwoRelevantStrings_ShouldProduceCorrectPercentageOutput(string value, string total,
            string expected)
        {
            var result = PercentageHelper.CalculatePercentageFromStrings(value, total);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("23", "23%")]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("text", "text")]
        public void GivenStringPercentage_ShouldDisplayAsPercentage(string value, string expected)
        {
            var result = PercentageHelper.DisplayAsPercentage(value);
            Assert.Equal(expected, result);
        }
        
    }
}