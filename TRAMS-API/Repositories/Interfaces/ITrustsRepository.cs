using API.Models.D365;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface ITrustsRepository
    {
        public Task<GetTrustsD365Model> GetTrustById(Guid id);

        public Task<List<GetTrustsD365Model>> SearchTrusts(string searchQuery);
    }
}
