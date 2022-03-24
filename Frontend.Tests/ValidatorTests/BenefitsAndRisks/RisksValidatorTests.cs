using Data.Models.Projects;
using FluentValidation.TestHelper;
using Frontend.Models.Benefits;
using Frontend.Models.Features;
using Frontend.Validators.Features;
using System.Collections.Generic;
using Frontend.Validators.BenefitsAndRisks;
using Xunit;

namespace Frontend.Tests.ValidatorTests.BenefitsAndRisks
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
