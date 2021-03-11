using API.Models.Downstream.D365;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models.Upstream.Response;

namespace API.Repositories
{
    public interface ITrustsRepository
    {
        public Task<RepositoryResult<GetTrustsModel>> GetTrustById(Guid id);

        public Task<RepositoryResult<List<GetTrustsModel>>> SearchTrusts(string searchQuery);
    }
}
