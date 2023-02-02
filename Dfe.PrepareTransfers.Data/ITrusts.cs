using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models;

namespace Dfe.PrepareTransfers.Data
{
    public interface ITrusts
    {
        public Task<RepositoryResult<List<TrustSearchResult>>> SearchTrusts(string searchQuery = "", string outgoingTrustId = "");

        public Task<RepositoryResult<Trust>> GetByUkprn(string ukprn);
    }
}