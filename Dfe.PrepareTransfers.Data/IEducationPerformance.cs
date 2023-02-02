using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;

namespace Dfe.PrepareTransfers.Data
{
    public interface IEducationPerformance
    {
        public Task<RepositoryResult<EducationPerformance>> GetByAcademyUrn(string urn);
    }
}