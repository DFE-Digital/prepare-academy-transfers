using Xunit;

namespace Dfe.PrepareTransfers.Helpers.Tests
{
    public class BooleanHelperTests
    {
        [Theory]
        [InlineData(null, "")]
        [InlineData(true, "Yes")]
        [InlineData(false, "No")]
        public void Should_return_display_value(bool? input, string expectedDisplayValue)
        {
            Assert.Equal(expectedDisplayValue, input.ToDisplay());
        }

    }
}
