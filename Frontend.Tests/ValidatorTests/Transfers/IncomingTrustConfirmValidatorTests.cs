using System;
using System.Collections.Generic;
using System.Text;
using Data;
using FluentValidation.TestHelper;
using Frontend.Pages.Transfers;
using Frontend.Validators.Transfers;
using Moq;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
{
    public class IncomingTrustConfirmValidatorTests
    {
        private readonly Mock<ITrusts> _trustsRepository;
        private readonly IncomingTrustConfirmValidator _validator;

        public IncomingTrustConfirmValidatorTests()
        {
            _validator = new IncomingTrustConfirmValidator();
            _trustsRepository = new Mock<ITrusts>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void WhenIncomingTrustIdIsEmpty_ShouldSetError(string trustId)
        {
            var trustSearch = new SearchIncomingTrustModel(_trustsRepository.Object)
            {
                SelectedTrustId = trustId
            };

            var result = await _validator.TestValidateAsync(trustSearch);

            result.ShouldHaveValidationErrorFor(x => x.SelectedTrustId)
                .WithErrorMessage("Select an incoming trust");
        }

        [Fact]
        public async void WhenIncomingTrustIdIsNotEmpty_ShouldNotSetError()
        {
            var trustSearch = new SearchIncomingTrustModel(_trustsRepository.Object)
            {
                SelectedTrustId = "trustId"
            };

            var result = await _validator.TestValidateAsync(trustSearch);
            
            result.ShouldNotHaveValidationErrorFor(x => x.SelectedTrustId);
        }


    }
}
