using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;

namespace Dfe.PrepareTransfers.Data
{
    public interface IProjects
    {
        public Task<RepositoryResult<List<ProjectSearchResult>>> GetProjects(int page = 1, string title = default,
           int pageSize = 10);
        public Task<RepositoryResult<Project>> GetByUrn(string urn);
        public Task<RepositoryResult<Project>> Create(Project project);

        public Task<bool> UpdateRationale(Project project);
        public Task<bool> UpdateFeatures(Project project);
        public Task<bool> UpdateBenefits(Project project);
        public Task<bool> UpdateGeneralInfomation(Project project);
        public Task<bool> UpdateLegalRequirements(Project project);
        public Task<bool> UpdateDates(Project project);
        public Task<bool> UpdateAcademy(string urn, TransferringAcademies academy);
    }
}