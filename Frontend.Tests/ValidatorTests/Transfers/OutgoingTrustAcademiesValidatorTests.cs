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

        [Fact]
        public async void WhenAcademyIdIsEmpty_ShouldSetError()
        {
            var academyIds = new string[] {};
            var result = await _validator.TestValidateAsync(academyIds);
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("Select an academy");
        }
        
        [Fact]
        public async void WhenAcademyIdIsNull_ShouldSetError()
        {
            string[] academyIds = null;
            var result = await _validator.TestValidateAsync(academyIds);
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("Select an academy");
        }

        [Fact]
        public async void WhenAcademyIdIsNotEmpty_ShouldNotSetError()
        {
            var academyId = new [] {"academy id"};
            var result = await _validator.TestValidateAsync(academyId);
            result.ShouldNotHaveValidationErrorFor(x => x);
        }


    }
}
