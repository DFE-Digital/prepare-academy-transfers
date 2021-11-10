using System;
using System.Collections.Generic;
using System.Text;
using Data.Models;
using FluentValidation.TestHelper;
using Frontend.Validators.Transfers;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
{
    public class IncomingTrustSearchValidatorTests
    {
        private readonly IncomingTrustSearchValidator _validator;
        public IncomingTrustSearchValidatorTests()
        {
            _validator = new IncomingTrustSearchValidator();
        }

        [Fact]
        public async void WhenIncomingTrustSearchIsEmpty_ShouldSetError()
        {
            var result = await _validator.TestValidateAsync(new List<TrustSearchResult>());
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("We could not find any trusts matching your search criteria");
        }

        [Fact]
        public async void WhenIncomingTrustSearchIsNotEmpty_ShouldNotSetError()
        {
            var list = new List<TrustSearchResult>()
            {
                new TrustSearchResult() { TrustName = "Trust One" },
                new TrustSearchResult() { TrustName = "Trust Two" },
            };
            var result = await _validator.TestValidateAsync(list);
            result.ShouldNotHaveValidationErrorFor(x => x);
        }


    }
}
