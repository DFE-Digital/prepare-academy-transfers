using Data;
using FluentValidation.TestHelper;
using Frontend.Pages.Transfers;
using Frontend.Validators.Transfers;
using Moq;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
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
                .WithErrorMessage("Select the outgoing trust");
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
