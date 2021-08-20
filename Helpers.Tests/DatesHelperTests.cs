using Xunit;

namespace Helpers.Tests
{
    public class DatesHelperTests
    {
        public class SourceDateStringIsGreaterThanTargetDateStringTests
        {
            [Theory]
            [InlineData("01/01/2010", "05/05/2010", false)]
            [InlineData("01/05/2010", "05/01/2010", true)]
            [InlineData("01/05/2010", "01/05/2010", false)]
            [InlineData(null, "05/01/2010", null)]
            [InlineData("05/01/2010", null, null)]
            [InlineData(null, null, null)]
            public void GivenTwoDateStrings_ShouldReturnCorrectResult(string sourceDate, string targetDate,
                bool? expectedResult)
            {
                var result = DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(sourceDate, targetDate);
                Assert.Equal(expectedResult, result);
            }
        }
    }
}