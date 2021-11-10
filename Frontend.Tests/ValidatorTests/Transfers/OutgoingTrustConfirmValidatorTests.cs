using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.TestHelper;
using Frontend.Validators.Transfers;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
{
    public class OutgoingTrustConfirmValidatorTests
    {
        private readonly OutgoingTrustConfirmValidator _validator;
        public OutgoingTrustConfirmValidatorTests()
        {
            _validator = new OutgoingTrustConfirmValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void WhenOutgoingTrustIdIsEmpty_ShouldSetError(string trustId)
        {
            var result = await _validator.TestValidateAsync(trustId);
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("Select the outgoing trust");
        }

        [Fact]
        public async void WhenOutgoingTrustIdIsNotEmpty_ShouldNotSetError()
        {
            string trustId = "trustId";
            var result = await _validator.TestValidateAsync(trustId);
            result.ShouldNotHaveValidationErrorFor(x => x);
        }


    }
}
