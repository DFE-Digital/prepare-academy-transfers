using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models;

namespace Dfe.PrepareTransfers.Data
{
    public interface IAcademies
    {
        public Task<RepositoryResult<Academy>> GetAcademyByUkprn(string ukprn);
    }
}