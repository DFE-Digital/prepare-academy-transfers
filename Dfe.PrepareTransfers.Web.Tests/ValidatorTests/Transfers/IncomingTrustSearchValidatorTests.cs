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
        public async Task WhenTrustResultsAreEmpty_ShouldSetError()
        {
            var trustSearch = new SearchIncomingTrustModel(_trustsRepository.Object);

            ((ISetTrusts)trustSearch).SetTrusts(Enumerable.Empty<Trust>());

            TestValidationResult<SearchIncomingTrustModel> result = await _validator.TestValidateAsync(trustSearch);

            result.ShouldHaveValidationErrorFor(x => x.Trusts)
                .WithErrorMessage("We could not find any trusts matching your search criteria");
        }

        [Fact]
        public async Task WhenResultsAreNotEmpty_ShouldNotSetError()
        {
            var trustSearch = new SearchIncomingTrustModel(_trustsRepository.Object);

            ((ISetTrusts)trustSearch).SetTrusts(new List<Trust>
            {
               new() { Name = "Trust One" },
               new() { Name = "Trust Two" },
            });

            var result = await _validator.TestValidateAsync(trustSearch);

            result.ShouldNotHaveValidationErrorFor(x => x);
        }


    }
}
