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
    public class TargetDateValidatorTests
    {
        private readonly TargetDateValidator _validator = new TargetDateValidator();

        [Fact]
        public void ShouldHaveChildValidators()
        {
            _validator.ShouldHaveChildValidator(a => a.TargetDate, typeof(DateValidator));
            _validator.ShouldHaveChildValidator(a => a.TargetDate, typeof(FutureDateValidator));
        }

        [Fact]
        public async void GivenHtbDateAndTargetDateLessThanHtbDate_ShouldGiveError()
        {
            var htbDate = DateTime.Now.AddMonths(2);
            var targetDate = DateTime.Now.AddMonths(1);
            var vm = new TargetDateViewModel
            {
                TargetDate = new DateViewModel
                {
                    Date = new DateInputViewModel
                    {
                        Day = targetDate.Day.ToString(),
                        Month = targetDate.Month.ToString(),
                        Year = targetDate.Year.ToString()
                    }
                }
            };
            
            var validationContext = new ValidationContext<TargetDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["HtbDate"] = htbDate.ToShortDate()
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Single(result.Errors);
            Assert.Equal("The target transfer date must be on or after the Advisory Board date", result.ToString());
        }
        
        [Fact]
        public async void GivenHtbDateAndTargetDateGreaterThanHtbDate_ShouldNotGiveError()
        {
            var htbDate = DateTime.Now.AddDays(3);
            var targetDate = DateTime.Now.AddMonths(1);
            var vm = new TargetDateViewModel
            {
                TargetDate = new DateViewModel
                {
                    Date = new DateInputViewModel
                    {
                        Day = targetDate.Day.ToString(),
                        Month = targetDate.Month.ToString(),
                        Year = targetDate.Year.ToString()
                    }
                }
            };
            
            var validationContext = new ValidationContext<TargetDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["HtbDate"] = htbDate.ToShortDate()
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Empty(result.Errors);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void GivenTargetDateAndNoHtbDate_ShouldNotGiveError(string htbDate)
        {
            var targetDate = DateTime.Now.AddMonths(1);
            var vm = new TargetDateViewModel
            {
                TargetDate = new DateViewModel
                {
                    Date = new DateInputViewModel
                    {
                        Day = targetDate.Day.ToString(),
                        Month = targetDate.Month.ToString(),
                        Year = targetDate.Year.ToString()
                    }
                }
            };
            
            var validationContext = new ValidationContext<TargetDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["HtbDate"] = htbDate
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Empty(result.Errors);
        }
    }
}