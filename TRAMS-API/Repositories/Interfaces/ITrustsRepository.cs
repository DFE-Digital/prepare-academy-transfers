using API.Models.D365;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface ITrustsRepository
    {
        public Task<RepositoryResult<GetTrustsD365Model>> GetTrustById(Guid id);

        public Task<RepositoryResult<List<GetTrustsD365Model>>> SearchTrusts(string searchQuery);
    }
}
