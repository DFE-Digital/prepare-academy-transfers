using FluentValidation;
using FluentValidation.TestHelper;
using Frontend.Models.Benefits;
using Frontend.Validators.Benefits;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Benefits
{
    public class OtherFactorsValidatorTests : AbstractValidator<OtherFactorsValidator>
    {
        private readonly OtherFactorsValidator _otherFactorsValidator;
        public OtherFactorsValidatorTests() => _otherFactorsValidator = new OtherFactorsValidator();
        
        [Fact]
        public void ShouldHaveChildValidator()
        {
            _otherFactorsValidator.ShouldHaveChildValidator(a => a.OtherFactorsVm, typeof(OtherFactorsItemValidator));
        }
        
        [Fact]
        public async void GivenNoOtherFactors_Valid()
        {
            var result = await _otherFactorsValidator.TestValidateAsync(new OtherFactorsViewModel());
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}