using Data;
using FluentValidation.TestHelper;
using Frontend.Pages.Transfers;
using Frontend.Validators.Transfers;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
{
    public class OutgoingTrustAcademiesValidatorTests
    {
        private readonly OutgoingTrustAcademiesValidator _validator;
        private readonly Mock<ITrusts> _trustsRepository;

        public OutgoingTrustAcademiesValidatorTests()
        {
            _validator = new OutgoingTrustAcademiesValidator();
            _trustsRepository = new Mock<ITrusts>();
        }

        [Fact]
        public async void WhenSelectedAcademyIdsIsNull_ShouldSetError()
        {
            var request = new OutgoingTrustAcademiesModel(_trustsRepository.Object)
            {
                SelectedAcademyIds = null
            };

            var result = await _validator.TestValidateAsync(request);

            result.ShouldHaveValidationErrorFor(request => request.SelectedAcademyIds)
                .WithErrorMessage("Select an academy");
        }

        [Fact]
        public async void WhenAcademyIdIsNotEmpty_ShouldNotSetError()
        {
            var request = new OutgoingTrustAcademiesModel(_trustsRepository.Object)
            {
                SelectedAcademyIds = new List<string> { "academy id" }
            };

            var result = await _validator.TestValidateAsync(request);

            result.ShouldNotHaveValidationErrorFor(request => request);
        }


    }
}
