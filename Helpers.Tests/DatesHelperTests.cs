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

        public class DayMonthYearToDateStringTests
        {
            [Theory]
            [InlineData(null, null, null, null)]
            [InlineData("", "", "", null)]
            [InlineData("1", "1", "2020", "01/01/2020")]
            public void GivenDayMonthYear_ShouldReturnCorrectDateString(string day, string month, string year,
                string expectedResult)
            {
                var result = DatesHelper.DayMonthYearToDateString(day, month, year);
                Assert.Equal(expectedResult, result);
            }
        }
    }
}