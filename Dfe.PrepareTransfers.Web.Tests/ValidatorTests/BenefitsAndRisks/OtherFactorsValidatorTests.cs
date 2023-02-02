using Dfe.PrepareTransfers.Data.Models.Projects;
using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Models.Benefits;
using Dfe.PrepareTransfers.Web.Models.Features;
using Dfe.PrepareTransfers.Web.Validators.Features;
using System.Collections.Generic;
using Dfe.PrepareTransfers.Web.Validators.BenefitsAndRisks;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.BenefitsAndRisks
{
    public class OtherFactorsValidatorTests
    {
        private readonly OtherFactorsValidator _otherFactorsValidator;
        public OtherFactorsValidatorTests() => _otherFactorsValidator = new OtherFactorsValidator();

        [Fact]
        public async void GivenNoSelection_InvalidWithErrorMessage()
        {
            var vm = new OtherFactorsViewModel
            {
                OtherFactorsVm = new List<OtherFactorsItemViewModel>(4)
            };
            var result = await _otherFactorsValidator.TestValidateAsync(vm);
            result.ShouldHaveValidationErrorFor(x => x.OtherFactorsVm)
                .WithErrorMessage("Select the risks with this transfer");
        }
        
        [Fact]
        public async void GivenSelection_ValidWithoutErrorMessage()
        {
            var vm = new OtherFactorsViewModel()
            {
                 OtherFactorsVm = new List<OtherFactorsItemViewModel>()
                 {
                     new OtherFactorsItemViewModel
                     {
                         Checked = true,
                         OtherFactor = TransferBenefits.OtherFactor.HighProfile
                     }
                 }
            };
            var result = await _otherFactorsValidator.TestValidateAsync(vm);
            result.ShouldNotHaveValidationErrorFor(x => x.OtherFactorsVm);
        }
    }
}
