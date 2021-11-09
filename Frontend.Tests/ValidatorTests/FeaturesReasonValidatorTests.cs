using Frontend.Models.Features;
using Frontend.Tests.Helpers;
using Frontend.Validators.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace Frontend.Tests.ValidatorTests
{
    public class FeaturesReasonValidatorTests
    {
        private readonly ModelStateDictionary _modelStateDictionary;
        public FeaturesReasonValidatorTests()
        {
            _modelStateDictionary = new ModelStateDictionary();
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
                Urn = "001",
                IsSubjectToIntervention = subjectToIntervention,
                OutgoingAcademyName = "Test",
                MoreDetail = moreDetail
            };
            var results = await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesReasonValidator(), vm, _modelStateDictionary);
            Assert.True(results.IsValid);
        }

        [Fact]
        public async void GivenNoSubjectToIntervention_FeaturesReasonValidator_InvalidWithErrorMessage()
        {
            var vm = new FeaturesReasonViewModel
            {
                Urn = "001"
            };
            var results = await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesReasonValidator(), vm, _modelStateDictionary);

            Assert.False(results.IsValid);
            Assert.Equal(nameof(vm.IsSubjectToIntervention), results.Errors[0].PropertyName);
            Assert.Equal("Select whether or not the transfer is subject to intervention", results.Errors[0].ErrorMessage);
        }
    }
}
