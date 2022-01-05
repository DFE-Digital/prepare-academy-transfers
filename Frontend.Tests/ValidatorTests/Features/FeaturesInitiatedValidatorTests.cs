using Data.Models.Projects;
using FluentValidation.TestHelper;
using Frontend.Models.Features;
using Frontend.Tests.Helpers;
using Frontend.Validators.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Features
{
    public class IntendedBenefitsValidatorTests
    {
        private readonly FeaturesInitiatedValidator _featuresInitiatedValidator;
        public IntendedBenefitsValidatorTests()
        {
            _featuresInitiatedValidator = new FeaturesInitiatedValidator();
        }

        [Theory]
        [InlineData(TransferFeatures.ProjectInitiators.Dfe)]
        [InlineData(TransferFeatures.ProjectInitiators.OutgoingTrust)]
        public async void GivenValidWhoInitiated_FeaturesInitiated_IsValid(TransferFeatures.ProjectInitiators projectInitiator)
        {
            var vm = new FeaturesInitiatedViewModel
            {
                WhoInitiated = projectInitiator,
            };

            var result = await _featuresInitiatedValidator.TestValidateAsync(vm);
            result.ShouldNotHaveValidationErrorFor(x => x.WhoInitiated);
        }

        [Theory]
        [InlineData(TransferFeatures.ProjectInitiators.Empty)]
        public async void GivenInvalidWhoInitiated_FeaturesInitiated_InvalidWithErrorMessage(TransferFeatures.ProjectInitiators projectInitiator)
        {
            var vm = new FeaturesInitiatedViewModel
            {
                WhoInitiated = projectInitiator
            };

            var result = await _featuresInitiatedValidator.TestValidateAsync(vm);
            result.ShouldHaveValidationErrorFor(x => x.WhoInitiated)
                .WithErrorMessage("Select who initiated the project");
        }
    }
}
