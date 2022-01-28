using System;
using FluentValidation;
using FluentValidation.TestHelper;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Frontend.Validators.TransferDates;
using Helpers;
using Xunit;

namespace Frontend.Tests.ValidatorTests.TransferDates
{
    public class HtbDateValidatorTests
    {
        private readonly HtbDateValidator _validator;

        public HtbDateValidatorTests() => _validator = new HtbDateValidator();
        
        [Fact]
        public void ShouldHaveChildValidators()
        {
            _validator.ShouldHaveChildValidator(a => a.HtbDate, typeof(DateValidator));
            _validator.ShouldHaveChildValidator(a => a.HtbDate, typeof(FutureDateValidator));
        }

        [Fact]
        public async void GivenHtbDateGreaterThanTargetDate_ShouldGiveError()
        {
            var htbDate = DateTime.Now.AddMonths(2);
            var targetDate = DateTime.Now.AddMonths(1);
            var vm = new HtbDateViewModel
            {
                HtbDate = new DateViewModel
                {
                    Date = new DateInputViewModel
                    {
                        Day = htbDate.Day.ToString(),
                        Month = htbDate.Month.ToString(),
                        Year = htbDate.Year.ToString(),
                    }
                }
            };
            
            var validationContext = new ValidationContext<HtbDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["TargetDate"] = targetDate.ToShortDate()
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Single(result.Errors);
            Assert.Equal("The Advisory Board date must be on or before the target date for the transfer", result.ToString());
        }
        
        [Fact]
        public async void GivenTargetDateGreaterThanHtbDate_ShouldNotGiveError()
        {
            var htbDate = DateTime.Now.AddMonths(1);
            var targetDate = DateTime.Now.AddMonths(2);
            var vm = new HtbDateViewModel
            {
                HtbDate = new DateViewModel
                {
                    Date = new DateInputViewModel
                    {
                        Day = htbDate.Day.ToString(),
                        Month = htbDate.Month.ToString(),
                        Year = htbDate.Year.ToString(),
                    }
                }
            };
            
            var validationContext = new ValidationContext<HtbDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["TargetDate"] = targetDate.ToShortDate()
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Empty(result.Errors);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void GivenHtbDateAndNoTargetDate_ShouldNotGiveError(string targetDate)
        {
            var htbDate = DateTime.Today;
            var vm = new HtbDateViewModel
            {
                HtbDate = new DateViewModel
                {
                    Date = new DateInputViewModel
                    {
                        Day = htbDate.Day.ToString(),
                        Month = htbDate.Month.ToString(),
                        Year = htbDate.Year.ToString(),
                    }
                }
            };
            
            var validationContext = new ValidationContext<HtbDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["TargetDate"] = targetDate
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Empty(result.Errors);
        }
    }
}