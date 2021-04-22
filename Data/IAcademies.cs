using System.Threading.Tasks;
using Data.Models;

namespace Data
{
    public interface IAcademies
    {
        public Task<RepositoryResult<Academy>> GetAcademyByUkprn(string ukprn);
    }
}