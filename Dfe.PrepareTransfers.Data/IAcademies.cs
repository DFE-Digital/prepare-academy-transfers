using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models;

namespace Dfe.PrepareTransfers.Data
{
    public interface IAcademies
    {
        public Task<Academy> GetAcademyByUkprn(string ukprn);

        public Task<List<Academy>> GetAcademiesByTrustUkprn(string ukprn);
    }
}