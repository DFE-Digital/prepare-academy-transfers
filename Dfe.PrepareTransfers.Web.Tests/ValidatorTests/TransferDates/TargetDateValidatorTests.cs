using System;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using Dfe.PrepareTransfers.Web.Validators.TransferDates;
using Dfe.PrepareTransfers.Helpers;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.TransferDates
{
    public class TargetDateValidatorTests
    {
        private readonly TargetDateValidator _validator = new();

        [Fact]
        public void ShouldHaveChildValidators()
        {
            _validator.ShouldHaveChildValidator(a => a.TargetDate, typeof(DateValidator));
        }

        [Fact]
        public async Task GivenAdvisoryBoardDateAndTargetDateLessThanAdvisoryBoardDate_ShouldGiveError()
        {
            var advisoryBoardDate = DateTime.Now.AddMonths(2);
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
                    ["AdvisoryBoardDate"] = advisoryBoardDate.ToShortDate()
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Single(result.Errors);
            Assert.Equal("The target transfer date must be on or after the Advisory board date", result.ToString());
        }
        
        [Fact]
        public async Task GivenAdvisoryBoardDateAndTargetDateGreaterThanAdvisoryBoardDate_ShouldNotGiveError()
        {
            var advisoryBoardDate = DateTime.Now.AddDays(3);
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
                    ["AdvisoryBoardDate"] = advisoryBoardDate.ToShortDate()
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Empty(result.Errors);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GivenTargetDateAndNoAdvisoryBoardDate_ShouldNotGiveError(string advisoryBoardDate)
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
                    ["AdvisoryBoardDate"] = advisoryBoardDate
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Empty(result.Errors);
        }
        
        [Fact]
        public async Task GivenAdvisoryBoardDateAndUnknownTargetDate_ShouldNotGiveError()
        {
            var vm = new TargetDateViewModel
            {
                TargetDate = new DateViewModel
                {
                    Date = new DateInputViewModel(),
                    UnknownDate = true
                }
            };
            
            var validationContext = new ValidationContext<TargetDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["AdvisoryBoardDate"] =  DateTime.Now.ToShortDate()
                }
            };
            
            var result = await _validator.ValidateAsync(validationContext);
            Assert.Empty(result.Errors);
        }
    }
}