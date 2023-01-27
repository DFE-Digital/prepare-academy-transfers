using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.Projects;
using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Models.Features;
using Dfe.PrepareTransfers.Web.Pages.Projects.Features;
using Dfe.PrepareTransfers.Web.Validators.Features;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.Features
{
    public class FeaturesReasonValidatorTests
    {
        private readonly FeaturesReasonValidator _featuresReasonValidator;
        private readonly Mock<IProjects> _mockProjects;

        public FeaturesReasonValidatorTests()
        {
            _featuresReasonValidator = new FeaturesReasonValidator();
            _mockProjects = new Mock<IProjects>();
        }

        [Theory]
        [InlineData(TransferFeatures.ReasonForTheTransferTypes.Dfe)]
        [InlineData(TransferFeatures.ReasonForTheTransferTypes.OutgoingTrust)]
        [InlineData(TransferFeatures.ReasonForTheTransferTypes.SponsorOrTrustClosure)]
        public async void GivenValidWhoInitiated_FeaturesInitiated_IsValid(TransferFeatures.ReasonForTheTransferTypes reason)
        {
            var model = new Reason(_mockProjects.Object)
            {
                ReasonForTheTransfer = reason,
            };

            var result = await _featuresReasonValidator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ReasonForTheTransfer);
        }

        [Theory]
        [InlineData(TransferFeatures.ReasonForTheTransferTypes.Empty)]
        public async void GivenInvalidWhoInitiated_FeaturesInitiated_InvalidWithErrorMessage(TransferFeatures.ReasonForTheTransferTypes reason)
        {
            var model = new Reason(_mockProjects.Object)
            {
                ReasonForTheTransfer = reason,
            };

            var result = await _featuresReasonValidator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.ReasonForTheTransfer)
                .WithErrorMessage("Select a reason for the transfer");
        }
    }
}
