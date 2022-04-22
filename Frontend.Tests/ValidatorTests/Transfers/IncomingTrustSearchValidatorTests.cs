﻿using System.Collections.Generic;
using Data;
using Data.Models;
using FluentValidation.TestHelper;
using Frontend.Pages.Transfers;
using Frontend.Validators.Transfers;
using Moq;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
{
    public class IncomingTrustSearchValidatorTests
    {
        private readonly IncomingTrustSearchValidator _validator;
        private readonly Mock<ITrusts> _trustsRepository;

        public IncomingTrustSearchValidatorTests()
        {
            _validator = new IncomingTrustSearchValidator();
            _trustsRepository = new Mock<ITrusts>();
        }

        [Fact]
        public async void WhenTrustResultsAreEmpty_ShouldSetError()
        {
            var trustSearch = new SearchIncomingTrustModel(_trustsRepository.Object)
            {
                Trusts = new List<TrustSearchResult>()
            };

            var result = await _validator.TestValidateAsync(trustSearch);

            result.ShouldHaveValidationErrorFor(x => x.Trusts)
                .WithErrorMessage("We could not find any trusts matching your search criteria");
        }

        [Fact]
        public async void WhenResultsAreNotEmpty_ShouldNotSetError()
        {
            var trustSearch = new SearchIncomingTrustModel(_trustsRepository.Object)
            {
                Trusts = new List<TrustSearchResult>
                {
                    new TrustSearchResult() { TrustName = "Trust One" },
                    new TrustSearchResult() { TrustName = "Trust Two" },
                }
            };

            var result = await _validator.TestValidateAsync(trustSearch);

            result.ShouldNotHaveValidationErrorFor(x => x);
        }


    }
}
