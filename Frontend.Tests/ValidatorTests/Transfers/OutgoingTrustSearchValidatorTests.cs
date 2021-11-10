using System;
using System.Collections.Generic;
using System.Text;
using Data.Models;
using FluentValidation.TestHelper;
using Frontend.Validators.Transfers;
using Xunit;

namespace Frontend.Tests.ValidatorTests.Transfers
{
    public class OutgoingTrustSearchValidatorTests
    {
        private readonly OutgoingTrustSearchValidator _validator;
        public OutgoingTrustSearchValidatorTests()
        {
            _validator = new OutgoingTrustSearchValidator();
        }

        [Fact]
        public async void WhenOutgoingTrustSearchIsEmpty_ShouldSetError()
        {
            var result = await _validator.TestValidateAsync(new List<TrustSearchResult>());
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("We could not find any trusts matching your search criteria");
        }

        [Fact]
        public async void WhenOutgoingTrustSearchIsNotEmpty_ShouldNotSetError()
        {
            var list = new List<TrustSearchResult>()
            {
                new TrustSearchResult() { TrustName = "Trust One", Academies = new List<TrustSearchAcademy>{new TrustSearchAcademy()}},
                new TrustSearchResult() { TrustName = "Trust Two", Academies = new List<TrustSearchAcademy>{new TrustSearchAcademy()}},
            };
            var result = await _validator.TestValidateAsync(list);
            result.ShouldNotHaveValidationErrorFor(x => x);
        }


    }
}
