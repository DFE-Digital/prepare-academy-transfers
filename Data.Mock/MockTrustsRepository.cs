using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models;

namespace Data.Mock
{
    public class MockTrustsRepository : ITrusts
    {
        public Task<RepositoryResult<List<TrustSearchResult>>> SearchTrusts(string searchQuery = "")
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
            throw new System.NotImplementedException();
        }
    }
}