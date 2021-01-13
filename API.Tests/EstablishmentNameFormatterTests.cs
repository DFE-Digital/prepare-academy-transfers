using API.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace API.Tests
{
    public class EstablishmentNameFormatterTests
    {
        private readonly EstablishmentNameFormatter _formatter;

        public EstablishmentNameFormatterTests()
        {
            _formatter = new EstablishmentNameFormatter();
        }

        [Fact]
        public void NullInput_Returns_EmptyString()
        {
            var input = (string)null;

            var result = _formatter.Format(input);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void EmptyInput_Returns_EmptyString()
        {
            var input = string.Empty;

            var result = _formatter.Format(input);

            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("    ")]
        public void WhiteSpaceInput_Returns_EmptyString(string input)
        {
            var result = _formatter.Format(input);

            Assert.Equal(string.Empty, result);
        }
        
        [Theory]
        [InlineData("NAME")]
        [InlineData(" NAME")]
        [InlineData("NAME ")]
        [InlineData(" NAME ")]
        [InlineData("   NAME   ")]
        public void OneWordName_AllCaps_VariousWhitespace_ReturnsFormattedName(string input)
        {
            var result = _formatter.Format(input);

            Assert.Equal("Name", result);
        }

        [Theory]
        [InlineData("name")]
        [InlineData(" name")]
        [InlineData("name ")]
        [InlineData(" name ")]
        [InlineData("   name   ")]
        public void OneWordName_AllLower_VariousWhitespace_ReturnsFormattedName(string input)
        {
            var result = _formatter.Format(input);

            Assert.Equal("Name", result);
        }

        [Theory]
        [InlineData("Name")]
        [InlineData(" Name")]
        [InlineData("Name ")]
        [InlineData(" Name ")]
        [InlineData("   Name   ")]
        public void OneWordName_FirstLetterCapitalised_VariousWhitespace_ReturnsFormattedName(string input)
        {
            var result = _formatter.Format(input);

            Assert.Equal("Name", result);
        }

        [Theory]
        [InlineData("NaMe")]
        [InlineData(" NaMe")]
        [InlineData("NaMe ")]
        [InlineData(" NaMe ")]
        [InlineData("   NaMe   ")]
        public void OneWordName_RandomCapitalization_VariousWhitespace_ReturnsFormattedName(string input)
        {
            var result = _formatter.Format(input);

            Assert.Equal("Name", result);
        }

        [Theory]
        [InlineData("dash-name")]
        [InlineData(" dash-name")]
        [InlineData("dash-name ")]
        [InlineData(" dash-name ")]
        [InlineData("   dash-name   ")]
        [InlineData("DASH-NAME")]
        [InlineData(" DASH-NAME")]
        [InlineData("DASH-NAME ")]
        [InlineData(" DASH-NAME ")]
        [InlineData("   DASH-NAME   ")]
        [InlineData("dAsH-NamE")]
        [InlineData(" DASh-NaME")]
        [InlineData("DAsh-naME ")]
        [InlineData(" DASH-nAmE ")]
        [InlineData("   dASH-nAME   ")]
        public void OneWordName_WithDash_VariousCapitalization_VariousWhitespace_ReturnsFormattedName(string input)
        {
            var result = _formatter.Format(input);

            Assert.Equal("Dash-name", result);
        }

        [Theory]
        [InlineData("SOME NAME")]
        [InlineData(" SOME NAME")]
        [InlineData("SOME NAME ")]
        [InlineData("SOME  NAME ")]
        [InlineData("SOME    NAME ")]
        [InlineData("  SOME    NAME  ")]
        public void TwoWordsName_AllCaps_VariousWhitespace_ReturnsFormattedName(string input)
        {
            var result = _formatter.Format(input);

            Assert.Equal("Some Name", result);
        }

        [Theory]
        [InlineData("SoMe NamE")]
        [InlineData(" SoMe NamE")]
        [InlineData("SoMe NamE ")]
        [InlineData("SoMe  NamE ")]
        [InlineData("SoMe    NamE ")]
        [InlineData("  SoMe    NamE  ")]
        public void TwoWordsName_VariousCapitalizations_VariousWhitespace_ReturnsFormattedName(string input)
        {
            var result = _formatter.Format(input);

            Assert.Equal("Some Name", result);
        }

        [Theory]
        [InlineData("SoMe DASH-NamE")]
        [InlineData(" SoMe daSH-NamE")]
        [InlineData("SoMe dash-NamE ")]
        [InlineData("SoMe  daSh-NamE ")]
        [InlineData("SoMe    dasH-NamE ")]
        [InlineData("  SoMe    DasH-NamE  ")]
        public void TwoWordsName_WithDash_VariousCapitalizations_VariousWhitespace_ReturnsFormattedName(string input)
        {
            var result = _formatter.Format(input);

            Assert.Equal("Some Dash-name", result);
        }

        [Theory]
        [InlineData("some name")]
        [InlineData(" some name")]
        [InlineData("some name ")]
        [InlineData("some  name ")]
        [InlineData("some    name ")]
        [InlineData("  some    name  ")]
        public void TwoWordsName_AllLower_VariousWhitespace_ReturnsFormattedName(string input)
        {
            var result = _formatter.Format(input);

            Assert.Equal("Some Name", result);
        }

        [Theory]
        [InlineData("a 5five word dash-name establishment")]
        [InlineData("  a 5five word dash-name establishment")]
        [InlineData("A 5five WORD DASH-NAME ESTABLISHMENT")]
        [InlineData("A 5five WORD DASH-NAME ESTABLISHMENT  ")]
        [InlineData("A 5five word dash-NAME estaBliSHMENT")]
        [InlineData("    A 5five word dash-NAME estaBliSHMENT   ")]
        [InlineData("  a 5five word dash-name establishment   ")]
        [InlineData("  A 5five WORD   DASH-NAME ESTABLISHMENT")]
        [InlineData("  A 5five WORD   DASH-NAME   ESTABLISHMENT  ")]
        [InlineData("  A 5five   WORD   DASH-NAME   ESTABLISHMENT  ")]
        [InlineData("  A 5five WORD   DASH-NAME ESTABLISHMENT  ")]
        [InlineData("A   5five word   dash-NAME    estaBliSHMENT")]
        public void FiveWordName_WithNumbers_WithDash_VariousWhitespace_VariousCapitalization_ReturnsFormattedName(string input)
        {
            var result = _formatter.Format(input);

            Assert.Equal("A 5five Word Dash-name Establishment", result);
        }
    }
}
