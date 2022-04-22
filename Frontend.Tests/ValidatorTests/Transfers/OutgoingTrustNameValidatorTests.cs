using Data;
using FluentValidation.TestHelper;
using Frontend.Pages.Transfers;
using Frontend.Validators.Transfers;
using Moq;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
{
    public class OutgoingTrustNameValidatorTests
    {
        private readonly Mock<ITrusts> _trustsRepository;
        private readonly OutgoingTrustNameValidator _validator;

        public OutgoingTrustNameValidatorTests()
        {
            _validator = new OutgoingTrustNameValidator();
            _trustsRepository = new Mock<ITrusts>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void WhenOutgoingTrustNameIsEmpty_ShouldSetError(string trustName)
        {
            var trustSearch = new TrustSearchModel(_trustsRepository.Object)
            {
                SearchQuery = trustName
            };

            var result = await _validator.TestValidateAsync(trustSearch);

            result.ShouldHaveValidationErrorFor(x => x.SearchQuery)
                .WithErrorMessage("Enter the outgoing trust name");
        }

        [Fact]
        public async void WhenOutgoingTrustIdIsNotEmpty_ShouldNotSetError()
        {
            var trustSearch = new TrustSearchModel(_trustsRepository.Object)
            {
                SearchQuery = "trust name"
            };
            var result = await _validator.TestValidateAsync(trustSearch);

            result.ShouldNotHaveValidationErrorFor(x => x);
        }
    }
}
