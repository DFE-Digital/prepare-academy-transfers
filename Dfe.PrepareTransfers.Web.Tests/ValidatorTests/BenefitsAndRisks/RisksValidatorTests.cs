using Data.Models.Projects;
using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Models.Benefits;
using Dfe.PrepareTransfers.Web.Models.Features;
using Dfe.PrepareTransfers.Web.Validators.Features;
using System.Collections.Generic;
using Dfe.PrepareTransfers.Web.Validators.BenefitsAndRisks;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.BenefitsAndRisks
{
    public class RisksValidatorTests
    {
        private readonly RisksValidator _risksValidator;
        public RisksValidatorTests() => _risksValidator = new RisksValidator();

        [Fact]
        public async void GivenNoSelection_InvalidWithErrorMessage()
        {
            var vm = new RisksViewModel();
            var result = await _risksValidator.TestValidateAsync(vm);
            result.ShouldHaveValidationErrorFor(x => x.RisksInvolved).WithErrorMessage("Select yes if there are risks to consider");
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void GivenSelection_ValidWithoutErrorMessage(bool yesNo)
        {
            var vm = new RisksViewModel
            {
                RisksInvolved = yesNo
            };
            var result = await _risksValidator.TestValidateAsync(vm);
            result.ShouldNotHaveValidationErrorFor(x => x.RisksInvolved);
        }
    }
}
