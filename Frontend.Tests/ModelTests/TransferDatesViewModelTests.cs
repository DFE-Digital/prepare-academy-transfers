using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.Projects;
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
            var project = new Project {Dates = new TransferDates {FirstDiscussed = inputDate}};
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
            var project = new Project {Dates = new TransferDates {Target = inputDate}};
            var model = new TransferDatesViewModel {Project = project};
            Assert.Equal(expectedDay, model.TargetDate.Day);
            Assert.Equal(expectedMonth, model.TargetDate.Month);
            Assert.Equal(expectedYear, model.TargetDate.Year);
        }

        [Theory]
        [InlineData("", "", "", "")]
        [InlineData("20/02/2020", "20", "02", "2020")]
        [InlineData("13/01/2021", "13", "01", "2021")]
        public void GivenHtbDate_ReturnsTheCorrectDateViewModel(string inputDate, string expectedDay,
            string expectedMonth, string expectedYear)
        {
            var project = new Project {Dates = new TransferDates {Htb = inputDate}};
            var model = new TransferDatesViewModel {Project = project};
            Assert.Equal(expectedDay, model.HtbDate.Day);
            Assert.Equal(expectedMonth, model.HtbDate.Month);
            Assert.Equal(expectedYear, model.HtbDate.Year);
        }
    }
}