using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Data
{
    public interface IProjects
    {
        public Task<RepositoryResult<List<ProjectSearchResult>>> GetProjects(GetProjectSearchModel searchModel);
        public Task<RepositoryResult<Project>> GetByUrn(string urn);
        public Task<RepositoryResult<Project>> Create(Project project);

        public Task<bool> UpdateRationale(Project project);
        public Task<bool> UpdateFeatures(Project project);
        public Task<bool> UpdateBenefits(Project project);
        public Task<bool> UpdateGeneralInfomation(Project project);
        public Task<bool> UpdateLegalRequirements(Project project);
        public Task<bool> UpdateDates(Project project);
        public Task<bool> UpdateAcademy(string urn, TransferringAcademy academy);
        public Task<bool> UpdateAcademyGeneralInformation(string projectUrn, TransferringAcademy transferringAcademy);
        public Task<bool> UpdateStatus(Project project);
        public Task<bool> UpdateIncomingTrust(string urn, string projectName, string incomingTrustUKPRN = "");

        public Task<bool> AssignUser(Project project);
        public Task<ApiResponse<FileStreamResult>> DownloadProjectExport(GetProjectSearchModel searchModel);
        public Task<ApiResponse<ProjectFilterParameters>> GetFilterParameters();
    }
}