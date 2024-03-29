using System;
using Xunit;

namespace Dfe.PrepareTransfers.Helpers.Tests
{
    public class DatesHelperTests
    {
        public class SourceDateStringIsGreaterThanTargetDateStringTests
        {
            [Theory]
            [InlineData("01/01/2010", "05/05/2010", false)]
            [InlineData("01/05/2010", "05/01/2010", true)]
            [InlineData("01/05/2010", "01/05/2010", false)]
            public void GivenTwoDateStrings_ShouldReturnCorrectResult(string sourceDate, string targetDate, bool expectedResult)
            {
                var result = DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(sourceDate, targetDate);
                Assert.Equal(expectedResult, result);
            }
            
            [Theory]
            [InlineData(null, "05/01/2010")]
            [InlineData("05/01/2010", null)]
            [InlineData(null, null)]
            public void GivenMissingStrings_ShouldThrowArgumentNullException(string sourceDate, string targetDate)
            {
                Assert.Throws<ArgumentNullException>(() =>
                    DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(sourceDate, targetDate));
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

        public class FormatDateStringTests
        {
            [Fact]
            public void GivenDateNoKnown_FormatsDateCorrectly()
            {
                var result = DatesHelper.FormatDateString("", false);
                
                Assert.Equal("I do not know this", result);
            }
            
            [Fact]
            public void GivenDateNoKnownAndUnknownDateString_FormatsDateCorrectly()
            {
                var result = DatesHelper.FormatDateString("", false, "test");
                
                Assert.Equal("test", result);
            }

            [Fact]
            public void GivenDateString_FormatsDateCorrectly()
            {
                var dateString = "01/01/2018";
                var result = DatesHelper.FormatDateString(dateString, true);
                
                Assert.Equal(DatesHelper.DateStringToGovUkDate(dateString), result);
            }
        }
    }
}