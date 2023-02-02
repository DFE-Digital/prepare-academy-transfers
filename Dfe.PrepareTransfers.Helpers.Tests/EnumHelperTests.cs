using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Dfe.PrepareTransfers.Helpers.Tests
{
    public class EnumHelperTests
    {
        public enum TestEnum
        {
            Empty = 0,
            [Display(Name = "First enum")] First,
            [Display(Name = "Second enum")] Second,
            Third = 3
        }

        [Fact]
        public void GetValuesReturnsEnumValues()
        {
            var result = EnumHelpers<TestEnum>.GetValues(TestEnum.First);
            Assert.Equal(TestEnum.First, result[1]);
            Assert.Equal(TestEnum.Second, result[2]);
        }

        [Theory]
        [InlineData(null, TestEnum.Empty)]
        [InlineData("First", TestEnum.First)]
        [InlineData("Second", TestEnum.Second)]
        [InlineData("3", TestEnum.Third)]
        public void EnumParsesCorrectly(string toParse, TestEnum expected)
        {
            Assert.Equal(expected, EnumHelpers<TestEnum>.Parse(toParse));
        }

        [Fact]
        public void GetNamesReturnsEnumNames()
        {
            var result = EnumHelpers<TestEnum>.GetNames(TestEnum.First);

            Assert.Equal("First", result[1]);
            Assert.Equal("Second", result[2]);
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