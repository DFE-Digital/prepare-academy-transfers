using Xunit;
using Dfe.PrepareTransfers.Web.Utils;

namespace Dfe.PrepareTransfers.Web.Tests.UtilsTests
{
    public class TypespaceExtensionsTests
    {
        [Theory]
        [InlineData("Hello World", "hello-world")]
        [InlineData("This is a test", "this-is-a-test")]
        [InlineData("123-456", "123-456")]
        [InlineData("", "")]
        [InlineData("Hello World!", "hello-world-")]
        public void Stub_ReturnsExpectedResult(string input, string expected)
        {
            // Arrange
            // Act
            var result = input.Stub();

            // Assert
            Assert.Equal(expected, result.Value);
        }
    }
}