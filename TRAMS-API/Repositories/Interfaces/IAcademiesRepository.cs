using API.Models.Downstream.D365;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IAcademiesRepository
    {
        public Task<RepositoryResult<GetAcademiesD365Model>> GetAcademyById(Guid id);

        public Task<RepositoryResult<List<GetAcademiesD365Model>>> GetAcademiesByTrustId(Guid id);

        public Task<RepositoryResult<List<GetAcademiesD365Model>>> SearchAcademies(string name);
    }
}
