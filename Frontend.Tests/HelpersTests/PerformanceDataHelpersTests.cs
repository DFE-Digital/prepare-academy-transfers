using Data.Models.KeyStagePerformance;
using Frontend.Helpers;
using Microsoft.AspNetCore.Html;
using Xunit;

namespace Frontend.Tests.HelpersTests
{
    public class PerformanceDataHelpersTests
    {
        public class GetFormattedResultTests
        {
            [Theory]
            [InlineData(null, "no data")]
            [InlineData("", "no data")]
            [InlineData("3.45", "3.45")]
            [InlineData("3.00", "3")]
            [InlineData("3", "3")]
            [InlineData("some text", "some text")]
            public void GivenUnformattedValue_ReturnsFormattedValue(string result, string expectedResult)
            {
                var formattedResult = PerformanceDataHelpers.GetFormattedResult(result);

                Assert.Equal(expectedResult, formattedResult);
            }
            
            [Theory]
            [InlineData(null, null , "no data")]
            [InlineData("", "", "no data")]
            [InlineData("3.45", null, "3.45<br>(disadvantaged no data)")]
            [InlineData("3.45", "2.44", "3.45<br>(disadvantaged 2.44)")]
            [InlineData(null, "3.44", "no data<br>(disadvantaged 3.44)")]
            public void GivenUnformattedDisadvantagedPupilResult_ReturnsFormattedValue(string nonDisadvantagePupilResult, string disadvantagedPupilResult, string expectedResult)
            {
                var pupilResult = new DisadvantagedPupilsResult
                    {NotDisadvantaged = nonDisadvantagePupilResult, Disadvantaged = disadvantagedPupilResult};

                var expectedResultAsHtml = new HtmlString(expectedResult);
                
                var formattedResult = PerformanceDataHelpers.GetFormattedResult(pupilResult);

                Assert.Equal(expectedResultAsHtml.ToString(), formattedResult.ToString());
            }
        }

        public class GetFormattedConfidenceIntervalTests
        {
            [Fact]
            public void GivenConfidenceIntervals_ShouldFormatCorrectly()
            {
                var result = PerformanceDataHelpers.GetFormattedConfidenceInterval(1.2M, 2.4M);
                
                Assert.Equal("1.2 to 2.4", result);
            }
            
            [Fact]
            public void GivenNullConfidenceIntervals_ShouldReturnNoData()
            {
                var result = PerformanceDataHelpers.GetFormattedConfidenceInterval(null, null);
                
                Assert.Equal("no data", result);
            }
        }
        
        public class GetFormattedYearTests
        {
            [Theory]
            [InlineData(null, null)]
            [InlineData("", "")]
            [InlineData("2018-2019", "2018 - 2019")]
            [InlineData("2018 - 2019", "2018 - 2019")]
            [InlineData("randomness", "randomness")]
            public void GivenYear_ShouldFormatCorrectly(string unformattedYear, string expectedFormattedYear)
            {
                var result = PerformanceDataHelpers.GetFormattedYear(unformattedYear);
                
                Assert.Equal(expectedFormattedYear, result);
            }
        }
    }
}