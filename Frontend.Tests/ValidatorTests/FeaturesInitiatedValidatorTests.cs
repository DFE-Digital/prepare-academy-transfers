using Data.Models.Projects;
using Frontend.Models.Features;
using Frontend.Tests.Helpers;
using Frontend.Validators.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace Frontend.Tests.ValidatorTests
{
    public class FeaturesInitiatedValidatorTests
    {
        private readonly ModelStateDictionary _modelStateDictionary;
        public FeaturesInitiatedValidatorTests()
        {
            _modelStateDictionary = new ModelStateDictionary();
        }

        [Theory]
        [InlineData(TransferFeatures.ProjectInitiators.Dfe)]
        [InlineData(TransferFeatures.ProjectInitiators.OutgoingTrust)]
        public async void GivenValidWhoInitiated_FeaturesInitiated_IsValid(TransferFeatures.ProjectInitiators projectInitiator)
        {
            var vm = new FeaturesInitiatedViewModel
            {
                Urn = "001",
                WhoInitiated = projectInitiator,
                OutgoingAcademyName = "Test"
            };
            var results = await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesInitiatedValidator(), vm, _modelStateDictionary);

            Assert.True(results.IsValid);
        }

        [Theory]
        [InlineData(TransferFeatures.ProjectInitiators.Empty)]
        public async void GivenInvalidWhoInitiated_FeaturesInitiated_InvalidWithErrorMessage(TransferFeatures.ProjectInitiators projectInitiator)
        {
            var vm = new FeaturesInitiatedViewModel
            {
                Urn = "001",
                WhoInitiated = projectInitiator,
                OutgoingAcademyName = "Test"
            };
            var results = await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesInitiatedValidator(), vm, _modelStateDictionary);

            Assert.False(results.IsValid);
            Assert.Equal(nameof(vm.WhoInitiated), results.Errors[0].PropertyName);
            Assert.Equal("Select who initiated the project", results.Errors[0].ErrorMessage);
        }
    }
}
