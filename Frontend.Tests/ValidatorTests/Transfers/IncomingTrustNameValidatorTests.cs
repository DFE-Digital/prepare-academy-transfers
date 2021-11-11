using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.TestHelper;
using Frontend.Validators.Transfers;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
{
    public class IncomingTrustNameValidatorTests
    {
        private readonly IncomingTrustNameValidator _validator;
        public IncomingTrustNameValidatorTests()
        {
            _validator = new IncomingTrustNameValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void WhenIncomingTrustNameIsEmpty_ShouldSetError(string trustName)
        {
            var result = await _validator.TestValidateAsync(trustName);
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("Enter the incoming trust name");
        }

        [Fact]
        public async void WhenIncomingTrustIdIsNotEmpty_ShouldNotSetError()
        {
            string trustName = "trust name";
            var result = await _validator.TestValidateAsync(trustName);
            result.ShouldNotHaveValidationErrorFor(x => x);
        }


    }
}
