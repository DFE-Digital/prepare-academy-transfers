using FluentValidation.TestHelper;
using Frontend.Models.Features;
using Frontend.Tests.Helpers;
using Frontend.Validators.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Features
{
    public class FeaturesReasonValidatorTests
    {
        private readonly FeaturesReasonValidator _featuresReasonValidator;
        public FeaturesReasonValidatorTests()
        {
            _featuresReasonValidator = new FeaturesReasonValidator();
        }

        [Theory]
        [InlineData(false, "More Detail")]
        [InlineData(true, "More Detail")]
        [InlineData(false, null)]
        [InlineData(true, null)]
        public async void GivenSubjectToIntervention_FeaturesReasonValidator_IsValid(bool subjectToIntervention, string moreDetail)
        {
            var vm = new FeaturesReasonViewModel()
            {
                IsSubjectToIntervention = subjectToIntervention,
                MoreDetail = moreDetail
            };
            var result = await _featuresReasonValidator.TestValidateAsync(vm);
            result.ShouldNotHaveValidationErrorFor(x => x.IsSubjectToIntervention);

        }

        [Fact]
        public async void GivenNoSubjectToIntervention_FeaturesReasonValidator_InvalidWithErrorMessage()
        {
            var vm = new FeaturesReasonViewModel();
            var result = await _featuresReasonValidator.TestValidateAsync(vm);

            result.ShouldHaveValidationErrorFor(x => x.IsSubjectToIntervention)
                .WithErrorMessage("Select whether or not the transfer is subject to intervention");
        }
    }
}
