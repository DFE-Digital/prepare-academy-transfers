using API.HttpHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace API.Tests.ODataHelpersTests
{
    public class QuerySanitisingTests
    {
        [Fact]
        public void ODataSanitizer_Handles_NullInput()
        {
            var sanitizer = new ODataSanitizer();

            var result = sanitizer.Sanitize(null);

            Assert.Null(result);
        }

        [Fact]
        public void ODataSanitizer_Handles_EmptyInput()
        {
            var sanitizer = new ODataSanitizer();

            var result = sanitizer.Sanitize(string.Empty);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("string","string")]
        [InlineData("a'b","a''b")]
        [InlineData("a&b", "a%26b")]
        [InlineData("a+B", "a%2BB")]
        [InlineData("a?B", "a%3FB")]
        [InlineData("a#B", "a%23B")]
        public void ODataSanitizer_ValueTests(string input, string expectedOutput)
        {
            var sanitizer = new ODataSanitizer();

            var result = sanitizer.Sanitize(input);

            Assert.Equal(expectedOutput, result);
        }

        [Fact]
        public void FetchXmlSanitizer_Handles_NullInput()
        {
            var sanitizer = new FetchXmlSanitizer();

            var result = sanitizer.Sanitize(null);

            Assert.Null(result);
        }

        [Fact]
        public void FetchXmlSanitizer_Handles_EnptyInput()
        {
            var sanitizer = new FetchXmlSanitizer();

            var result = sanitizer.Sanitize(string.Empty);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("string", "string")]
        [InlineData("a&b", "a&amp;b")]
        [InlineData("a<b", "a&lt;b")]
        [InlineData("a>B", "a&gt;B")]
        [InlineData("a\"B", "a&quot;B")]
        [InlineData("a'B", "a&apos;B")]
        public void FetchXmlSanitizer_ValueTests(string input, string expectedOutput)
        {
            var sanitizer = new FetchXmlSanitizer();

            var result = sanitizer.Sanitize(input);

            Assert.Equal(expectedOutput, result);
        }
    }
}
