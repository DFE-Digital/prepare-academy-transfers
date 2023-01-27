using System;
using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using Dfe.PrepareTransfers.Web.Validators.TransferDates;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.TransferDates
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
            _validator.ErrorMessage = "The date the transfer was first discussed with a trust must be in the past";
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
                .WithErrorMessage(_validator.ErrorMessage);
        }

        [Fact]
        public async void WhenDateIsFuture_AndErrorMessageNotSet_ShouldShowDefaultErrorMessage()
        {
            _validator.ErrorMessage = null;
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
                .WithErrorMessage("The date must be in the past");
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