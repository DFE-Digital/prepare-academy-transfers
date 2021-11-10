using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.TestHelper;
using Frontend.Validators.Transfers;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
{
    public class IncomingTrustConfirmValidatorTests
    {
        private readonly IncomingTrustConfirmValidator _validator;
        public IncomingTrustConfirmValidatorTests()
        {
            _validator = new IncomingTrustConfirmValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void WhenIncomingTrustIdIsEmpty_ShouldSetError(string trustId)
        {
            var result = await _validator.TestValidateAsync(trustId);
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("Select an incoming trust");
        }

        [Fact]
        public async void WhenIncomingTrustIdIsNotEmpty_ShouldNotSetError()
        {
            string trustId = "trustId";
            var result = await _validator.TestValidateAsync(trustId);
            result.ShouldNotHaveValidationErrorFor(x => x);
        }


    }
}
