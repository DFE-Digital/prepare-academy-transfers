using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Xunit;

namespace Frontend.Tests.HelpersTests
{
    public class DatesHelperTests
    {
        [Fact]
        public void GivenDateInJanuary2020_GeneratesHTBDatesUntilJanuary2021()
        {
            var expectedDateValues = new List<string>
            {
                "03/02/2020", "02/03/2020", "01/04/2020", "01/05/2020",
                "01/06/2020", "01/07/2020", "03/08/2020", "01/09/2020",
                "01/10/2020", "02/11/2020", "01/12/2020", "01/01/2021"
            };

            var htbDates = DatesHelper.GetFirstWorkingDaysOfTheTheMonthForTheNextYear("02/01/2020");
            Assert.Equal(expectedDateValues, htbDates.Select(htbDate => htbDate.ToString("dd/MM/yyyy")).ToList());
        }

        [Theory]
        [InlineData("03/02/2020")]
        [InlineData("01/01/2020")]
        public void GivenStartingHtbDateIsFirstWorkingDayInMonth_GenerateHtbDatesIncludingThatDate(string startDate)
        {
            var htbDates = DatesHelper.GetFirstWorkingDaysOfTheTheMonthForTheNextYear(startDate);
            Assert.Equal(startDate, htbDates[0].ToString("dd/MM/yyyy"));
        }

        [Theory]
        [InlineData("01/02/2020", "03/02/2020")]
        [InlineData("04/02/2020", "02/03/2020")]
        [InlineData("17/05/2021", "01/06/2021")]
        public void GivenStartingHtbDateIsNotFirstWorkingDayInMonth_GenerateHtbDatesExcludingThatDate(string startDate,
            string expectedFirstDate)
        {
            var htbDates = DatesHelper.GetFirstWorkingDaysOfTheTheMonthForTheNextYear(startDate);
            Assert.Equal(expectedFirstDate, htbDates[0].ToString("dd/MM/yyyy"));
        }

        [Theory]
        [InlineData("01/01/2010", "1 January 2010")]
        [InlineData("01-01-2010", "1 January 2010")]
        public void GivenDate_ShouldFormatAsGovUkDate(string unformattedDate, string expectedFormattedDate)
        {
            var result = DatesHelper.DateStringToGovUkDate(unformattedDate);
            Assert.Equal(expectedFormattedDate, result);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GivenNullOrWhiteSpaceFormat_ShouldReturnNullOrWhiteSpace(string unformattedDate)
        {
            var result = DatesHelper.DateStringToGovUkDate(unformattedDate);
            Assert.Equal(unformattedDate,result);
        }
        
        [Theory]
        [InlineData("not a date")]
        [InlineData("40-40-1980")]
        public void GivenIncorrectFormat_ShouldThrowException(string unformattedDate){
            Assert.ThrowsAny<Exception>(() => DatesHelper.DateStringToGovUkDate(unformattedDate));
        }
    }
}