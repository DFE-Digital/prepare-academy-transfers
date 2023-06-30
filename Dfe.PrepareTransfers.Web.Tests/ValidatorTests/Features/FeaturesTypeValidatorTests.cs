using Dfe.PrepareTransfers.Data.Models.Projects;
using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Models.Features;
using Dfe.PrepareTransfers.Web.Validators.Features;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.Features
{
    public class FeaturesTypeValidatorTests
    {
        private readonly FeaturesTypeValidator _featuresTypeValidator;
        public FeaturesTypeValidatorTests()
        {
            _featuresTypeValidator = new FeaturesTypeValidator();
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
            var result = await _featuresTypeValidator.TestValidateAsync(vm);
            result.ShouldNotHaveValidationErrorFor(x => x.TypeOfTransfer);
        }

        [Fact]
        public async void GivenEmptyTypeOfTransfer_FeaturesTypeValidator_InvalidWithErrorMessage()
        {
            var vm = new FeaturesTypeViewModel
            {
                TypeOfTransfer = TransferFeatures.TransferTypes.Empty
            };
            
            var result = await _featuresTypeValidator.TestValidateAsync(vm);
            result.ShouldHaveValidationErrorFor(x => x.TypeOfTransfer)
                .WithErrorMessage("Select the type of transfer");
        }

        [Fact]
        public async void GivenEmptyOtherTypeOfTransfer_FeaturesTypeValidator_InvalidWithErrorMessage()
        {
            var vm = new FeaturesTypeViewModel
            {
                TypeOfTransfer = TransferFeatures.TransferTypes.Other
            };
            var result = await _featuresTypeValidator.TestValidateAsync(vm);
            result.ShouldHaveValidationErrorFor(x => x.OtherType)
                .WithErrorMessage("Enter the type of transfer");
        }

    }
}
