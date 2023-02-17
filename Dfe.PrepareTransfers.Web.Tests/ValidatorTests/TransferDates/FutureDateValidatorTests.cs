using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using Dfe.PrepareTransfers.Web.Validators.TransferDates;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.TransferDates
{
    public class FutureDateValidatorTests
    {
        private readonly FutureDateValidator _validator;
        
        public FutureDateValidatorTests()
        {
            _validator = new FutureDateValidator();
        }
        
        [Fact]
        public async Task WhenDateIsPast_ShouldShowError()
        {
            DateTime pastDate = DateTime.Now.AddDays(-10);
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

            result.ShouldHaveValidationErrorFor(a => a.Date)
                .WithErrorMessage("You must enter a future date");
        }
        
        [Fact]
        public async Task WhenDateIsToday_ShouldNotShowError()
        {
            DateTime pastDate = DateTime.Now;
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

            TestValidationResult<DateViewModel> result = await _validator.TestValidateAsync(dateVm);

            result.ShouldNotHaveAnyValidationErrors();
        }
        
        
    }
}