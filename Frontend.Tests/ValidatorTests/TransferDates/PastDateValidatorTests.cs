using System;
using FluentValidation.TestHelper;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Frontend.Validators.TransferDates;
using Xunit;

namespace Frontend.Tests.ValidatorTests.TransferDates
{
    public class PastDateValidatorTests
    {
        private readonly PastDateValidator _validator;
        
        public PastDateValidatorTests()
        {
            _validator = new PastDateValidator();
        }
        
        [Fact]
        public async void WhenDateIsFuture_ShouldShowError()
        {
            var pastDate = DateTime.Now.AddDays(+10);
            var dateVm = new DateViewModel
            {
                Date = new DateInputViewModel
                {
                    Day = pastDate.Day.ToString(),
                    Month = pastDate.Month.ToString(),
                    Year = pastDate.Year.ToString()
                },
                UnknownDate = false
            };

            var result = await _validator.TestValidateAsync(dateVm);

            result.ShouldHaveValidationErrorFor(a => a.Date.Day)
                .WithErrorMessage("The date the transfer was first discussed with a trust must be in the past");
        }
        
        [Fact]
        public async void WhenDateIsToday_ShouldNotShowError()
        {
            var pastDate = DateTime.Now;
            var dateVm = new DateViewModel
            {
                Date = new DateInputViewModel
                {
                    Day = pastDate.Day.ToString(),
                    Month = pastDate.Month.ToString(),
                    Year = pastDate.Year.ToString()
                },
                UnknownDate = false
            };

            var result = await _validator.TestValidateAsync(dateVm);
            result.ShouldNotHaveAnyValidationErrors();
        }
        
        
    }
}
