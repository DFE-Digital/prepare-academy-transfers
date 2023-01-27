using System;
using System.Collections.Generic;
using System.Text;
using Dfe.PrepareTransfers.Data;
using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Pages.Transfers;
using Dfe.PrepareTransfers.Web.Validators.Transfers;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.Transfers
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
                .WithErrorMessage("Select a trust");
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
