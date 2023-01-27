using Data;
using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Pages.Transfers;
using Dfe.PrepareTransfers.Web.Validators.Transfers;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.Transfers
{
    public class IncomingTrustNameValidatorTests
    {
        private readonly Mock<ITrusts> _trustsRepository;
        private readonly IncomingTrustNameValidator _validator;

        public IncomingTrustNameValidatorTests()
        {
            _validator = new IncomingTrustNameValidator();
            _trustsRepository = new Mock<ITrusts>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void WhenIncomingTrustNameIsEmpty_ShouldSetError(string trustName)
        {
            var incomingTrustSearch = new SearchIncomingTrustModel(_trustsRepository.Object)
            {
                SearchQuery = trustName
            };
            var result = await _validator.TestValidateAsync(incomingTrustSearch);

            result.ShouldHaveValidationErrorFor(x => x.SearchQuery)
                .WithErrorMessage("Enter the incoming trust name");
        }

        [Fact]
        public async void WhenIncomingTrustIdIsNotEmpty_ShouldNotSetError()
        {
            var incomingTrustSearch = new SearchIncomingTrustModel(_trustsRepository.Object)
            {
                SearchQuery = "trust name"
            };

            var result = await _validator.TestValidateAsync(incomingTrustSearch);

            result.ShouldNotHaveValidationErrorFor(x => x);
        }
    }
}
