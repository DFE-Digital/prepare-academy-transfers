using FluentValidation.TestHelper;
using Frontend.Models.Rationale;
using Frontend.Validators.Rationale;
using Xunit;

namespace Frontend.Tests.ValidatorTests
{
    public class RationaleTrustOrSponsorValidatorTests
    {
        private RationaleTrustOrSponsorValidator validator;

        public RationaleTrustOrSponsorValidatorTests()
        {
            validator = new RationaleTrustOrSponsorValidator();
        }
        
        [Fact]
        public async void WhenProjectRationaleIsNull_ShouldSetError()
        {
            var vm = new RationaleTrustOrSponsorViewModel
            {
                TrustOrSponsorRationale = ""
            };

            var result = await validator.TestValidateAsync(vm);
            result.ShouldHaveValidationErrorFor(r => r.TrustOrSponsorRationale)
                .WithErrorMessage("Enter the rationale for the incoming trust or sponsor");
        }
        
        [Fact]
        public async void WhenProjectRationaleIsNotNull_ShouldNotSetError()
        {
            var vm = new RationaleTrustOrSponsorViewModel
            {
                TrustOrSponsorRationale = "test"
            };

            var result = await validator.TestValidateAsync(vm);
            result.ShouldNotHaveValidationErrorFor(r => r.TrustOrSponsorRationale);
        }
    }
}