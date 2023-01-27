using Dfe.PrepareTransfers.Data;
using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Pages.Transfers;
using Dfe.PrepareTransfers.Web.Validators.Transfers;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.Transfers
{
    public class OutgoingTrustConfirmValidatorTests
    {
        private readonly OutgoingTrustConfirmValidator _validator;
        private readonly Mock<ITrusts> _trustsRepository;

        public OutgoingTrustConfirmValidatorTests()
        {
            _validator = new OutgoingTrustConfirmValidator();
            _trustsRepository = new Mock<ITrusts>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void WhenOutgoingTrustIdIsEmpty_ShouldSetError(string trustId)
        {
            var outgoingTrustDetails = new OutgoingTrustDetailsModel(_trustsRepository.Object)
            {
                TrustId = trustId
            };

            var result = await _validator.TestValidateAsync(outgoingTrustDetails);

            result.ShouldHaveValidationErrorFor(x => x.TrustId)
                .WithErrorMessage("Select a trust");
        }

        [Fact]
        public async void WhenOutgoingTrustIdIsNotEmpty_ShouldNotSetError()
        {
            var outgoingTrustDetails = new OutgoingTrustDetailsModel(_trustsRepository.Object)
            {
                TrustId = "trustId"
            };

            var result = await _validator.TestValidateAsync(outgoingTrustDetails);

            result.ShouldNotHaveValidationErrorFor(x => x.TrustId);
        }
    }
}
