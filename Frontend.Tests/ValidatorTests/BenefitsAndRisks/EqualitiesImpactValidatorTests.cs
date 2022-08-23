using FluentValidation.TestHelper;
using Frontend.Models.Benefits;
using Frontend.Validators.BenefitsAndRisks;
using Xunit;

namespace Frontend.Tests.ValidatorTests.BenefitsAndRisks
{
    public class EqualitiesImpactValidatorTests
    {
        private readonly EqualitiesImpactValidator _validator;
        public EqualitiesImpactValidatorTests() => _validator = new EqualitiesImpactValidator();

        [Fact]
        public async void GivenNoSelection_InvalidWithErrorMessage()
        {
            var vm = new EqualitiesImpactAssessmentViewModel();
            var result = await _validator.TestValidateAsync(vm);
            result.ShouldHaveValidationErrorFor(x => x.EqualitiesImpactAssessmentConsidered)
                .WithErrorMessage("Select yes if an Equalities Impact Assessment has been considered");
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void GivenSelection_ValidWithoutErrorMessage(bool yesNo)
        {
            var vm = new EqualitiesImpactAssessmentViewModel
            {
               EqualitiesImpactAssessmentConsidered = yesNo
            };
            var result = await _validator.TestValidateAsync(vm);
            result.ShouldNotHaveValidationErrorFor(x => x.EqualitiesImpactAssessmentConsidered);
        }
    }
}
