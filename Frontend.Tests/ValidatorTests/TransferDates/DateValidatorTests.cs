using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Frontend.Validators.TransferDates;
using Xunit;

namespace Frontend.Tests.ValidatorTests.TransferDates
{
    public class DateValidatorTests
    {
        private readonly DateValidator _validator;
        
        public DateValidatorTests()
        {
            _validator = new DateValidator
            {
                ErrorDisplayName = "Advisory board date"
            };
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void WhenDateIsNullAndUnknownIsFalse_ShouldShowError(string dateValue)
        {
            var dateVm = new DateViewModel
            {
                Date = new DateInputViewModel
                {
                    Day = dateValue,
                    Month = dateValue,
                    Year = dateValue
                },
                UnknownDate = false
            };

            var result = await _validator.TestValidateAsync(dateVm);

            result.ShouldHaveValidationErrorFor(a => a.Date.Day)
                .WithErrorMessage("Enter Advisory board date or select I do not know this");
        }
        
        [Fact]
        public async void WhenDateIsNotNullAndUnknownIsTrue_ShouldShowError()
        {
            var dateVm = new DateViewModel
            {
                Date = new DateInputViewModel
                {
                    Day = "1",
                    Month = "1",
                    Year = "2000"
                },
                UnknownDate = true
            };

            var result = await _validator.TestValidateAsync(dateVm);

            result.ShouldHaveValidationErrorFor(a => a.Date.Day)
                .WithErrorMessage("Enter Advisory board date or select I do not know this");
        }

        [Fact]
        public async Task WhenFieldIsEmptyAndUnknownDate_DoesNotHaveMissingFieldError()
        {
            var dateVm = new DateViewModel
            {
                Date = new DateInputViewModel
                {
                    Day = "",
                    Month = "2",
                    Year = "2022"
                },
                UnknownDate = true
            };
            
            var result = await _validator.TestValidateAsync(dateVm);

            result.ShouldHaveValidationErrorFor(viewModel => viewModel.Date.Day).WithoutErrorMessage("The Advisory board date must include a day");
        }

        [Theory]
        [InlineData("", "1", "2022", "The Advisory board date must include a day", nameof(DateInputViewModel.Day))]
        [InlineData("1", "", "2022", "The Advisory board date must include a month", nameof(DateInputViewModel.Month))]
        [InlineData("1", "1", "", "The Advisory board date must include a year", nameof(DateInputViewModel.Year))]
        public async Task WhenFieldIsEmptyAndUnknownDateIsFalse_HasMissingFieldError(string day, string month, string year,
            string expectedErrorMessage, string expectedPropertyName)
        {
            var dateVm = new DateViewModel
            {
                Date = new DateInputViewModel
                {
                    Day = day,
                    Month = month,
                    Year = year
                },
                UnknownDate = false
            };
            
            var result = await _validator.TestValidateAsync(dateVm);

            result.ShouldHaveValidationErrorFor(expectedPropertyName)
                .WithErrorMessage(expectedErrorMessage);
        }

        [Theory]
        [InlineData("1//2021")]
        [InlineData("/1/2021")]
        [InlineData("//")]
        [InlineData("/2/")]
        [InlineData("1//2000")]
        [InlineData("1/1/20000")]
        [InlineData("35/1/2021")]
        [InlineData("01/14/2000")]
        [InlineData("01/11/9")]
        [InlineData("Date")]
        [InlineData("1-3-1900")]
        [InlineData(" ")]
        [InlineData("2020/12/1")]
        [InlineData("01012000")]
        [InlineData("20000101")]
        public async void WhenDateIsInvalidAndUnknownIsFalse_ShouldShowError(string dateValue)
        {
            var dateVm = new DateViewModel
            {
                Date = new DateInputViewModel
                {
                    Day = dateValue,
                    Month = dateValue,
                    Year = dateValue
                },
                UnknownDate = false
            };

            var result = await _validator.TestValidateAsync(dateVm);

            result.ShouldHaveValidationErrorFor(a => a.Date.Day)
                .WithErrorMessage("Enter a valid date");
        }
    }
}