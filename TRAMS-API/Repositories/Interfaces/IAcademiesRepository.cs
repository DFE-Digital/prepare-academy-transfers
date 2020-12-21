using API.Models.D365;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IAcademiesRepository
    {
        public Task<GetAcademiesD365Model> GetAcademyById(Guid id);

        public Task<List<GetAcademiesD365Model>> GetAcademiesByTrustId(Guid id);
    }
}
