using FluentValidation;
using FluentValidation.TestHelper;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Frontend.Validators.TransferDates;
using Xunit;

namespace Frontend.Tests.ValidatorTests.TransferDates
{
    public class HtbDateValidatorTests
    {
        private readonly HtbDateValidator _validator;

        public HtbDateValidatorTests() => _validator = new HtbDateValidator();
        
        [Fact]
        public void ShouldHaveChildValidator()
        {
            _validator.ShouldHaveChildValidator(a => a.HtbDate, typeof(DateValidator));
        }

        [Fact]
        public async void GivenTargetDateAndHtbDateLessThanTargetDate_ShouldGiveError()
        {
            var vm = new HtbDateViewModel
            {
                HtbDate = new DateViewModel
                {
                    Date = new DateInputViewModel
                    {
                        Day = "01",
                        Month = "01",
                        Year = "2022"
                    }
                }
            };
            
            var validationContext = new ValidationContext<HtbDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["TargetDate"] = "01/01/2000"
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Single(result.Errors);
            Assert.Equal("The Advisory Board date must be on or before the target date for the transfer", result.ToString());
        }
        
        [Fact]
        public async void GivenTargetDateAndHtbDateGreaterThanTargetDate_ShouldNotGiveError()
        {
            var vm = new HtbDateViewModel
            {
                HtbDate = new DateViewModel
                {
                    Date = new DateInputViewModel
                    {
                        Day = "01",
                        Month = "01",
                        Year = "2000"
                    }
                }
            };
            
            var validationContext = new ValidationContext<HtbDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["TargetDate"] = "01/01/2001"
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
            var vm = new HtbDateViewModel
            {
                HtbDate = new DateViewModel
                {
                    Date = new DateInputViewModel
                    {
                        Day = "01",
                        Month = "01",
                        Year = "2000"
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