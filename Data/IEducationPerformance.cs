using System.Threading.Tasks;
using Data.Models.KeyStagePerformance;

namespace Data
{
    public interface IEducationPerformance
    {
        public Task<RepositoryResult<EducationPerformance>> GetByAcademyUrn(string urn);
    }
}