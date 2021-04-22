using API.Models.Downstream.D365;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models.Upstream.Response;
using Data;

namespace API.Repositories
{
    public interface IAcademiesRepository
    {
        public Task<RepositoryResult<GetAcademiesModel>> GetAcademyById(Guid id);

        public Task<RepositoryResult<List<GetAcademiesModel>>> GetAcademiesByTrustId(Guid id);
    }
}
