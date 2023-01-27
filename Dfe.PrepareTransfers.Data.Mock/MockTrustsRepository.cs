using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models;

namespace Dfe.PrepareTransfers.Data.Mock
{
    public class MockTrustsRepository : ITrusts
    {
        public Task<RepositoryResult<List<TrustSearchResult>>> SearchTrusts(string searchQuery = "",
            string outgoingTrustId = "")
        {
            var result = new RepositoryResult<List<TrustSearchResult>>
            {
                Result = new List<TrustSearchResult>
                {
                    new TrustSearchResult
                    {
                        Ukprn = "0001",
                        TrustName = "Example trust",
                        CompaniesHouseNumber = "00001",
                        Academies = new List<TrustSearchAcademy>
                        {
                            new TrustSearchAcademy {Ukprn = "0002", Name = "Example Academy"}
                        }
                    }
                }
            };

            return Task.FromResult(result);
        }

        public Task<RepositoryResult<Trust>> GetByUkprn(string ukprn)
        {
            var result = new RepositoryResult<Trust>
            {
                Result = new Trust()
                {
                    Name = "Trust Name",
                    Ukprn = "ukprn",
                    Address = new List<string>() {"line1", "line2"},
                    Academies = new List<Academy>()
                    {
                        new Academy()
                        {
                            Name = "Academy name",
                            Urn = "Academy Urn",
                            Ukprn = "Academy Ukprn"
                        }
                    }
                }
            };

            return Task.FromResult(result);
        }
    }
}