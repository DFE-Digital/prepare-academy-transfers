using Dfe.PrepareTransfers.Helpers;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ExtensionMethodsTests
{
    public class StringExtensionsTests
    {
        public class ToTitleCaseTests
        {
            [Theory]
            [InlineData(null, null)]
            [InlineData("", "")]
            [InlineData("All Title Case", "All Title Case")]
            [InlineData("all lower case", "All Lower Case")]
            [InlineData("ALL UPPER CASE", "All Upper Case")]
            public void GivenString_ShouldConvertToTitleCase(string givenString, string expectedStringAsTitleCase)
            {
                var result = givenString?.ToTitleCase();
                Assert.Equal(expectedStringAsTitleCase, result);
            }
        }
    }
}