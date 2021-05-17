using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Frontend.Models;
using Xunit;

namespace Frontend.Tests.ModelTests
{
    public class TransferDatesViewModelTests
    {
        [Theory]
        [InlineData("", "", "", "")]
        [InlineData("20/02/2020", "20", "02", "2020")]
        [InlineData("13/01/2021", "13", "01", "2021")]
        public void GivenFirstDiscussedDate_ReturnsTheCorrectDateViewModel(string inputDate, string expectedDay,
            string expectedMonth, string expectedYear)
        {
            var project = new Project {TransferDates = new TransferDates {FirstDiscussed = inputDate}};
            var model = new TransferDatesViewModel {Project = project};
            Assert.Equal(expectedDay, model.TransferFirstDiscussed.Day);
            Assert.Equal(expectedMonth, model.TransferFirstDiscussed.Month);
            Assert.Equal(expectedYear, model.TransferFirstDiscussed.Year);
        }

        [Theory]
        [InlineData("", "", "", "")]
        [InlineData("20/02/2020", "20", "02", "2020")]
        [InlineData("13/01/2021", "13", "01", "2021")]
        public void GivenTargetTransferDate_ReturnsTheCorrectDateViewModel(string inputDate, string expectedDay,
            string expectedMonth, string expectedYear)
        {
            var project = new Project {TransferDates = new TransferDates {Target = inputDate}};
            var model = new TransferDatesViewModel {Project = project};
            Assert.Equal(expectedDay, model.TargetDate.Day);
            Assert.Equal(expectedMonth, model.TargetDate.Month);
            Assert.Equal(expectedYear, model.TargetDate.Year);
        }

        [Fact]
        public void GivenDateInJanuary2020_GeneratesHTBDatesUntilJanuary2021()
        {
            var expectedDateValues = new List<string>
            {
                "03/02/2020", "02/03/2020", "01/04/2020", "01/05/2020",
                "01/06/2020", "01/07/2020", "03/08/2020", "01/09/2020",
                "01/10/2020", "02/11/2020", "01/12/2020", "01/01/2021"
            };

            var expectedDateDisplay = new List<string>
            {
                "3 February 2020", "2 March 2020", "1 April 2020",
                "1 May 2020", "1 June 2020", "1 July 2020",
                "3 August 2020", "1 September 2020", "1 October 2020",
                "2 November 2020", "1 December 2020", "1 January 2021"
            };

            var htbDates = TransferDatesViewModel.PotentialHtbDates("01/01/2020");

            Assert.Equal(expectedDateValues, htbDates.Select(htbDate => htbDate.Value).ToList());
            Assert.Equal(expectedDateDisplay, htbDates.Select(htbDate => htbDate.DisplayName).ToList());
        }
    }
}