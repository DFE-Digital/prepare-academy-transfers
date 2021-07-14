using Frontend.Helpers;
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
            public void GivenNull_ReturnsNoData(string result, string expectedResult)
            {
                var formattedResult = PerformanceDataHelpers.GetFormattedResult(result);

                Assert.Equal(expectedResult, formattedResult);
            }
        }
    }
}