using System;
using System.Collections.Generic;
using System.Text;
using Data;
using Data.Models;
using FluentValidation.TestHelper;
using Frontend.Pages.Transfers;
using Frontend.Validators.Transfers;
using Moq;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
{
    public class OutgoingTrustSearchValidatorTests
    {
        private readonly OutgoingTrustSearchValidator _validator;
        private readonly Mock<ITrusts> _trustsRepository;

        public OutgoingTrustSearchValidatorTests()
        {
            _validator = new OutgoingTrustSearchValidator();
            _trustsRepository = new Mock<ITrusts>();
        }

        [Fact]
        public async void WhenTrustResultsAreEmpty_ShouldSetError()
        {
            var trustSearch = new TrustSearchModel(_trustsRepository.Object)
            {
                Trusts = new List<TrustSearchResult>()
            };

            var result = await _validator.TestValidateAsync(trustSearch);
            result.ShouldHaveValidationErrorFor(x => x.Trusts)
                .WithErrorMessage("We could not find any trusts matching your search criteria");
        }

        [Fact]
        public async void WhenTrustResultsAreNotEmpty_ShouldNotSetError()
        {
            var trustSearch = new TrustSearchModel(_trustsRepository.Object)
            {
                Trusts = new List<TrustSearchResult>()
                {
                    new TrustSearchResult() { TrustName = "Trust One", Academies = new List<TrustSearchAcademy>{new TrustSearchAcademy()}},
                    new TrustSearchResult() { TrustName = "Trust Two", Academies = new List<TrustSearchAcademy>{new TrustSearchAcademy()}},
                }
            };

            var result = await _validator.TestValidateAsync(trustSearch);
            result.ShouldNotHaveValidationErrorFor(x => x);
        }
    }
}
