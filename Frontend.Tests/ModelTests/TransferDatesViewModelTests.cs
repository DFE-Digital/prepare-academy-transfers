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
    }
}