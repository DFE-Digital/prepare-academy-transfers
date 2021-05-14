using System.ComponentModel.DataAnnotations;
using Frontend.Helpers;
using Xunit;

namespace Frontend.Tests.HelpersTests
{
    public class EnumHelperTests
    {
        public enum TestEnum
        {
            [Display(Name = "First enum")] First,
            [Display(Name = "Second enum")] Second
        }

        [Fact]
        public void GetValuesReturnsEnumValues()
        {
            var result = EnumHelpers<TestEnum>.GetValues(TestEnum.First);
            Assert.Equal(TestEnum.First, result[0]);
            Assert.Equal(TestEnum.Second, result[1]);
        }

        [Theory]
        [InlineData("First", TestEnum.First)]
        [InlineData("Second", TestEnum.Second)]
        public void EnumParsesCorrectly(string toParse, TestEnum expected)
        {
            Assert.Equal(expected, EnumHelpers<TestEnum>.Parse(toParse));
        }

        [Fact]
        public void GetNamesReturnsEnumNames()
        {
            var result = EnumHelpers<TestEnum>.GetNames(TestEnum.First);

            Assert.Equal("First", result[0]);
            Assert.Equal("Second", result[1]);
        }

        [Fact]
        public void GetDisplayValuesReturnsDisplayValues()
        {
            var result = EnumHelpers<TestEnum>.GetDisplayValues(TestEnum.First);

            Assert.Equal("First enum", result[0]);
            Assert.Equal("Second enum", result[1]);
        }

        [Theory]
        [InlineData(TestEnum.First, "First enum")]
        [InlineData(TestEnum.Second, "Second enum")]
        public void GetDisplayValueReturnsDisplayValue(TestEnum testEnum, string displayValue)
        {
            Assert.Equal(displayValue, EnumHelpers<TestEnum>.GetDisplayValue(testEnum));
        }
    }
}