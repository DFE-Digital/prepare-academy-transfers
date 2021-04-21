using System.Threading.Tasks;
using Data.Models;

namespace Data
{
    public interface IAcademies
    {
        public Task<Academy> GetAcademyByUkprn(string ukprn);
    }
}