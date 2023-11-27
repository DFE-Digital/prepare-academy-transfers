using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models;

namespace Dfe.PrepareTransfers.Data
{
    public interface ITrusts
    {
        public Task<List<Trust>> SearchTrusts(string searchQuery = "", string outgoingTrustId = "");

        public Task<Trust> GetByUkprn(string ukprn);
    }
}