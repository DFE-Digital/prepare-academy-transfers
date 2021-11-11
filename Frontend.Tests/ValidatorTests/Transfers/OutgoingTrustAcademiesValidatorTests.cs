using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.TestHelper;
using Frontend.Validators.Transfers;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
{
    public class OutgoingTrustAcademiesValidatorTests
    {
        private readonly OutgoingTrustAcademiesValidator _validator;
        public OutgoingTrustAcademiesValidatorTests()
        {
            _validator = new OutgoingTrustAcademiesValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void WhenAcademyIdIsEmpty_ShouldSetError(string academyId)
        {
            var result = await _validator.TestValidateAsync(academyId);
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("Select an academy");
        }

        [Fact]
        public async void WhenAcademyIdIsNotEmpty_ShouldNotSetError()
        {
            string academyId = "academy id";
            var result = await _validator.TestValidateAsync(academyId);
            result.ShouldNotHaveValidationErrorFor(x => x);
        }


    }
}
