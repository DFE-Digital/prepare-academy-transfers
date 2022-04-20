using FluentValidation.TestHelper;
using Frontend.Pages.Transfers;
using Frontend.Validators.Transfers;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
{
    public class OutgoingTrustNameValidatorTests
    {
        private readonly OutgoingTrustNameValidator _validator;
        public OutgoingTrustNameValidatorTests()
        {
            _validator = new OutgoingTrustNameValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void WhenOutgoingTrustNameIsEmpty_ShouldSetError(string trustName)
        {
            var result = await _validator.TestValidateAsync(trustName);
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("Enter the outgoing trust name");
        }

        [Fact]
        public async void WhenOutgoingTrustIdIsNotEmpty_ShouldNotSetError()
        {
            string trustName = "trust name";
            var result = await _validator.TestValidateAsync(trustName);
            result.ShouldNotHaveValidationErrorFor(x => x);
        }
    }
}
