using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Web.Helpers;
using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Pages.Transfers;
using Dfe.PrepareTransfers.Web.Validators.Transfers;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.Transfers
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
        public async Task WhenTrustResultsAreEmpty_ShouldSetError()
        {
           var trustSearch = new TrustSearchModel(_trustsRepository.Object);

           ((ISetTrusts)trustSearch).SetTrusts(Enumerable.Empty<TrustSearchResult>());

            TestValidationResult<TrustSearchModel> result = await _validator.TestValidateAsync(trustSearch);
            result.ShouldHaveValidationErrorFor(x => x.Trusts)
                .WithErrorMessage("We could not find any trusts matching your search criteria");
        }

        [Fact]
        public async Task WhenTrustResultsAreNotEmpty_ShouldNotSetError()
        {
            var trustSearch = new TrustSearchModel(_trustsRepository.Object);

            ((ISetTrusts)trustSearch).SetTrusts(new List<TrustSearchResult>
            {
               new() { TrustName = "Trust One", Academies = new List<TrustSearchAcademy> { new() } },
               new() { TrustName = "Trust Two", Academies = new List<TrustSearchAcademy> { new() } }
            });

            TestValidationResult<TrustSearchModel> result = await _validator.TestValidateAsync(trustSearch);
            result.ShouldNotHaveValidationErrorFor(x => x);
        }
    }
}
