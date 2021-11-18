using Data.Models.Projects;
using FluentValidation.TestHelper;
using Frontend.Models.Benefits;
using Frontend.Models.Features;
using Frontend.Validators.Benefits;
using Frontend.Validators.Features;
using System.Collections.Generic;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Benefits
{
    public class IntendedBenefitsValidatorTests
    {
        private readonly IntendedBenefitsValidator _intendedBenefitsValidator;
        public IntendedBenefitsValidatorTests() => _intendedBenefitsValidator = new IntendedBenefitsValidator();

        [Fact]
        public async void GivenUrnAndNoBenefits_InvalidWithErrorMessage()
        {
            var vm = new IntendedBenefitsViewModel
            {
                Urn = "0001",
                SelectedIntendedBenefits = new List<TransferBenefits.IntendedBenefit>(),
                OtherBenefit = ""
            };

            var result = await _intendedBenefitsValidator.TestValidateAsync(vm);
            result.ShouldHaveValidationErrorFor(x => x.SelectedIntendedBenefits).WithErrorMessage("Select at least one intended benefit");

        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void GivenOtherBenefitButNoDescription_InvalidWithErrorMessage(string otherBenefit)
        {
            var vm = new IntendedBenefitsViewModel
            {
                Urn = "0001",
                SelectedIntendedBenefits = new List<TransferBenefits.IntendedBenefit>() { TransferBenefits.IntendedBenefit.Other },
                OtherBenefit = otherBenefit
            };
            var result = await _intendedBenefitsValidator.TestValidateAsync(vm);
            result.ShouldHaveValidationErrorFor(x => x.OtherBenefit).WithErrorMessage("Enter the other benefit");
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void GivenManyBenefitsButNoDescription_InvalidWithErrorMessage(string otherBenefit)
        {
            var vm = new IntendedBenefitsViewModel
            {
                Urn = "0001",
                SelectedIntendedBenefits = new List<TransferBenefits.IntendedBenefit>() { 
                    TransferBenefits.IntendedBenefit.ImprovingSafeguarding, 
                    TransferBenefits.IntendedBenefit.StrongerLeadership,
                    TransferBenefits.IntendedBenefit.CentralFinanceTeamAndSupport,
                    TransferBenefits.IntendedBenefit.Other },
                OtherBenefit = otherBenefit
            };
            var result = await _intendedBenefitsValidator.TestValidateAsync(vm);
            result.ShouldHaveValidationErrorFor(x => x.OtherBenefit).WithErrorMessage("Enter the other benefit");
        }

    }
}
