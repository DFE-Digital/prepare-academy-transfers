using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models.Upstream.Response;
using Data;

namespace API.Repositories.Interfaces
{
    public interface ITrustsRepository
    {
        public Task<RepositoryResult<GetTrustsModel>> GetTrustById(Guid id);

        public Task<RepositoryResult<List<GetTrustsModel>>> SearchTrusts(string searchQuery);
    }
}
