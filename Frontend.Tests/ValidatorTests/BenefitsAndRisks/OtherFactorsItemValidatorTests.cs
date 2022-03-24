using System;
using Data.Models.Projects;
using FluentValidation;
using FluentValidation.TestHelper;
using Frontend.Models.Benefits;
using Frontend.Validators.BenefitsAndRisks;
using Xunit;

namespace Frontend.Tests.ValidatorTests.BenefitsAndRisks
{
    public class OtherFactorsItemValidatorTests : AbstractValidator<OtherFactorsItemValidator>
    {
        private readonly OtherFactorsItemValidator _otherFactorsItemValidator;
        public OtherFactorsItemValidatorTests() => _otherFactorsItemValidator = new OtherFactorsItemValidator();

        [Theory]
        [InlineData("HighProfile")]
        [InlineData("ComplexLandAndBuildingIssues")]
        [InlineData("FinanceAndDebtConcerns")]
        public async void GivenOtherFactorNotChecked_Valid(string otherFactorString)
        {
            var vm = new OtherFactorsItemViewModel
            {
                OtherFactor =
                    (TransferBenefits.OtherFactor) Enum.Parse(typeof(TransferBenefits.OtherFactor),
                        otherFactorString),
                Checked = false,
                Description = ""
            };
            
            var result = await _otherFactorsItemValidator.TestValidateAsync(vm);
            result.ShouldNotHaveAnyValidationErrors();
        }
        
        
        [Theory]
        [InlineData("HighProfile")]
        [InlineData("ComplexLandAndBuildingIssues")]
        [InlineData("FinanceAndDebtConcerns")]
        public async void GivenOtherFactorWithNoDescription_InvalidWithErrorMessage(string otherFactorString)
        {
            var vm = new OtherFactorsItemViewModel
            {
                OtherFactor =
                    (TransferBenefits.OtherFactor) Enum.Parse(typeof(TransferBenefits.OtherFactor),
                        otherFactorString),
                Checked = true,
                Description = ""
            };
            
            var result = await _otherFactorsItemValidator.TestValidateAsync(vm);

            result.ShouldHaveValidationErrorFor(i => i.Description);
        }
    }
}