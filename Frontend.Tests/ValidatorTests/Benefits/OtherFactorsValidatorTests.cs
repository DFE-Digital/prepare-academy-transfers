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
        public async void GivenUrnAndNoOtherFactors_Valid()
        {
            var vm = new OtherFactorsViewModel()
            {
                ProjectUrn = "0001",
            };

            var result = await _otherFactorsValidator.TestValidateAsync(vm);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}