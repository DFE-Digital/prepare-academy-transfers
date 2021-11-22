using FluentValidation;
using FluentValidation.TestHelper;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Frontend.Validators.TransferDates;
using Xunit;

namespace Frontend.Tests.ValidatorTests.TransferDates
{
    public class TargetDateValidatorTests
    {
        private readonly TargetDateValidator _validator = new TargetDateValidator();

        [Fact]
        public void ShouldHaveChildValidator()
        {
            _validator.ShouldHaveChildValidator(a => a.TargetDate, typeof(DateValidator));
        }

        [Fact]
        public async void GivenHtbDateAndTargetDateLessThanHtbDate_ShouldGiveError()
        {
            var vm = new TargetDateViewModel
            {
                TargetDate = new DateViewModel
                {
                    Date = new DateInputViewModel
                    {
                        Day = "01",
                        Month = "01",
                        Year = "2020"
                    }
                }
            };
            
            var validationContext = new ValidationContext<TargetDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["HtbDate"] = "01/01/2022"
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Single(result.Errors);
            Assert.Equal("The target transfer date must be on or after the Advisory Board date", result.ToString());
        }
        
        [Fact]
        public async void GivenHtbDateAndTargetDateGreaterThanHtbDate_ShouldNotGiveError()
        {
            var vm = new TargetDateViewModel
            {
                TargetDate = new DateViewModel
                {
                    Date = new DateInputViewModel
                    {
                        Day = "01",
                        Month = "01",
                        Year = "2022"
                    }
                }
            };
            
            var validationContext = new ValidationContext<TargetDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["HtbDate"] = "01/01/2001"
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Empty(result.Errors);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void GivenTargetDateAndNoHtbDate_ShouldNotGiveError(string targetDate)
        {
            var vm = new TargetDateViewModel
            {
                TargetDate = new DateViewModel
                {
                    Date = new DateInputViewModel
                    {
                        Day = "01",
                        Month = "01",
                        Year = "2000"
                    }
                }
            };
            
            var validationContext = new ValidationContext<TargetDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["HtbDate"] = targetDate
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Empty(result.Errors);
        }
    }
}