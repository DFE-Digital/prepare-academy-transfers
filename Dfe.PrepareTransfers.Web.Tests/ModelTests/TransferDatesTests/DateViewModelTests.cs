using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ModelTests.TransferDatesTests
{
    public class DateViewModelTests
    {
        public class DateInputAsStringTests
        {
            [Theory]
            [InlineData("1", "1", "2000", "01/01/2000")]
            [InlineData("", "1", "2000", "/01/2000")]
            [InlineData("1", "", "2000", "01//2000")]
            [InlineData("1", "1", "", "01/01/")]
            [InlineData("", "", "", null)]
            public void GivenDate_ShouldReturnCorrectDataString(string day, string month, string year,
                string expected)
            {
                var dateInput = new DateViewModel()
                {
                    Date = new DateInputViewModel {
                        Day = day,
                        Month = month,
                        Year = year
                    }
                };

                var result = dateInput.DateInputAsString();
                
                Assert.Equal(expected, result);
            }
        }

        public class SplitDateIntoDayMonthYearTests
        {
            [Theory]
            [InlineData("01/01/2000", "01", "01", "2000")]
            [InlineData("/01/2000", "", "01", "2000")]
            [InlineData("01//2000", "01", "", "2000")]
            [InlineData("01/01/", "01", "01", "")]
            [InlineData(null, "", "", "")]
            public void GivenDateAsString_ShouldReturnDateInputViewModel(string dateString, string day, string month, string year)
            {
                var result = DateViewModel.SplitDateIntoDayMonthYear(dateString);
                
                Assert.Equal(result.Day, day);
                Assert.Equal(result.Month, month);
                Assert.Equal(result.Year, year);
            }
        }
    }
}