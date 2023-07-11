namespace Dfe.PrepareTransfers.Helpers.Tests;

using Xunit;

public class StringHelperTests
{
    public class ToHtmlName
    {
        [Fact]
        public void ReplacesCharacters()
        {
            const string text = "some]text[for.testing";

            var result = text.ToHtmlName();

            Assert.Equal("some_text_for_testing", result);
        }
    }
}