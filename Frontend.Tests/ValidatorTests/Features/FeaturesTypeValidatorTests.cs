using Data.Models.Projects;
using Frontend.Models.Features;
using Frontend.Tests.Helpers;
using Frontend.Validators.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Features
{
    public class FeaturesTypeValidatorTests
    {
        private readonly ModelStateDictionary _modelStateDictionary;
        public FeaturesTypeValidatorTests()
        {
            _modelStateDictionary = new ModelStateDictionary();
        }

        [Theory]
        [InlineData(TransferFeatures.TransferTypes.SatClosure)]
        [InlineData(TransferFeatures.TransferTypes.JoiningToFormMat)]
        [InlineData(TransferFeatures.TransferTypes.MatClosure)]
        [InlineData(TransferFeatures.TransferTypes.MatToMat)]
        [InlineData(TransferFeatures.TransferTypes.TrustsMerging)]
        [InlineData(TransferFeatures.TransferTypes.Other, "Other")]
        public async void GivenNotEmptyTypeOfTransfer_FeaturesTypeValidator_IsValid(TransferFeatures.TransferTypes transferType, string otherType = null)
        {
            var vm = new FeaturesTypeViewModel
            {
                TypeOfTransfer = transferType,
                OtherType = otherType
            };
            var results = await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesTypeValidator(), vm, _modelStateDictionary);
            Assert.True(results.IsValid);
        }

        [Fact]
        public async void GivenEmptyTypeOfTransfer_FeaturesTypeValidator_InvalidWithErrorMessage()
        {
            var vm = new FeaturesTypeViewModel
            {
                TypeOfTransfer = TransferFeatures.TransferTypes.Empty
            };
            var results = await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesTypeValidator(), vm, _modelStateDictionary);

            Assert.False(results.IsValid);
            Assert.Equal(nameof(vm.TypeOfTransfer), results.Errors[0].PropertyName);
            Assert.Equal("Select the type of transfer", results.Errors[0].ErrorMessage);
        }

        [Fact]
        public async void GivenEmptyOtherTypeOfTransfer_FeaturesTypeValidator_InvalidWithErrorMessage()
        {
            var vm = new FeaturesTypeViewModel
            {
                TypeOfTransfer = TransferFeatures.TransferTypes.Other
            };
            var results = await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesTypeValidator(), vm, _modelStateDictionary);

            Assert.False(results.IsValid);
            Assert.Equal(nameof(vm.OtherType), results.Errors[0].PropertyName);
            Assert.Equal("Enter the type of transfer", results.Errors[0].ErrorMessage);
        }

    }
}
