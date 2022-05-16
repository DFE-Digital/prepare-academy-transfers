using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Helpers.Tests
{
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

        public class ToTitleCase
        {
            [Theory]
            [InlineData("A TITLE", "A Title")]
            [InlineData("a title", "A Title")]
            public void FormatsAsTitleCase(string input, string expectedOutput)
            {
                var result = input.ToTitleCase();

                Assert.Equal(expectedOutput, result);
            }
        }

        public class ToHyphenated
        {
            [Theory]
            [InlineData("some text", "some-text")]
            [InlineData("some    text", "some-text")]
            [InlineData("some\ttext", "some-text")]
            public void ReplacesWhiteSpaceWithHyphens(string input, string expectedOutput)
            {
                var result = input.ToHyphenated();

                Assert.Equal(expectedOutput, result);
            }
        }

        public class RemoveNonAlphanumericOrWhiteSpace
        {
            [Fact]
            public void RemovesCharacters()
            {
                const string text = "some text-with-punctuation_and'numbers99][()";

                var result = text.RemoveNonAlphanumericOrWhiteSpace();

                Assert.Equal("some text-with-punctuation_andnumbers99", result);
            }
        }
    }
}
