using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Data.TRAMS.ExtensionMethods;
using Dfe.PrepareTransfers.Data.TRAMS.Mappers.Request;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Dfe.PrepareTransfers.Data.TRAMS
{
    public class TramsProjectsRepository : IProjects
    {
        private readonly IAcademies _academies;
        private readonly IMapper<AcademisationProject, Project> _externalToInternalProjectMapper;
        private readonly ITramsHttpClient _httpClient;

        private readonly IAcademisationHttpClient _academisationHttpClient;
        private readonly IMapper<Project, TramsProjectUpdate> _internalToUpdateMapper;
        private readonly IMapper<TramsProjectSummary, ProjectSearchResult> _summaryToInternalProjectMapper;
        private readonly ITrusts _trusts;

        private readonly IReadOnlyDictionary<string, string> _aliasedStatuses = new Dictionary<string, string> { { "converter pre-ao (c)", "Pre advisory board" } };
        private readonly IReadOnlyDictionary<string, string> _invertedAliasedStatuses;


        public TramsProjectsRepository(ITramsHttpClient httpClient, IAcademisationHttpClient academisationHttpClient,
           IMapper<AcademisationProject, Project> externalToInternalProjectMapper,
           IMapper<TramsProjectSummary, ProjectSearchResult> summaryToInternalProjectMapper, IAcademies academies,
           ITrusts trusts, IMapper<Project, TramsProjectUpdate> internalToUpdateMapper)
        {
            _httpClient = httpClient;
            _academisationHttpClient = academisationHttpClient;
            _externalToInternalProjectMapper = externalToInternalProjectMapper;
            _summaryToInternalProjectMapper = summaryToInternalProjectMapper;
            _academies = academies;
            _trusts = trusts;
            _internalToUpdateMapper = internalToUpdateMapper;
            _invertedAliasedStatuses = _aliasedStatuses.ToDictionary(x => x.Value.ToLowerInvariant(), x => x.Key);
        }

        public async Task<RepositoryResult<List<ProjectSearchResult>>> GetProjects(GetProjectSearchModel searchModel)
        {
            var content = new StringContent(JsonConvert.SerializeObject(searchModel), Encoding.Default,
                "application/json");

            HttpResponseMessage response =
               await _academisationHttpClient.PostAsync("transfer-project/GetTransferProjects", content);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();

                var summaries = JsonConvert.DeserializeObject<ApiV2Wrapper<IEnumerable<TramsProjectSummary>>>(apiResponse);

                List<ProjectSearchResult> mappedSummaries =
                   summaries.Data.Select(summary => _summaryToInternalProjectMapper.Map(summary)).ToList();

                return new RepositoryResult<List<ProjectSearchResult>>
                {
                    Result = mappedSummaries,
                    TotalRecords = summaries.Paging.RecordCount
                };
            }

            throw new TramsApiException(response);
        }

        public async Task<RepositoryResult<Project>> GetByUrn(string urn)
        {
            HttpResponseMessage response = await _academisationHttpClient.GetAsync($"transfer-project/{urn}");
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var project = JsonConvert.DeserializeObject<AcademisationProject>(apiResponse);

                #region API Interim

                var outgoingTrust = await _trusts.GetByUkprn(project.OutgoingTrustUkprn);
                project.OutgoingTrust = new TrustSummary
                {
                    Ukprn = project.OutgoingTrustUkprn,
                    GroupName = outgoingTrust?.Name
                };
                project.TransferringAcademies = project.TransferringAcademies.Select(async transferring =>
                   {
                       Trust incomingTrust = null;

                       if (!string.IsNullOrEmpty(transferring.IncomingTrustUkprn))
                       {
                           incomingTrust = await _trusts.GetByUkprn(transferring.IncomingTrustUkprn);

                           transferring.IncomingTrust = new TrustSummary
                           {
                               GroupName = incomingTrust.Name,
                               GroupId = incomingTrust.GiasGroupId,
                               Ukprn = transferring.IncomingTrustUkprn
                           };
                       }
                       else {
                           // for form a mat
                           transferring.IncomingTrust = new TrustSummary
                           {
                               GroupName = transferring.IncomingTrustName,
                               Ukprn = transferring.IncomingTrustUkprn
                           };
                       }

                      Academy outgoingAcademy =
                      await _academies.GetAcademyByUkprn(transferring.OutgoingAcademyUkprn);

                       transferring.OutgoingAcademy = new AcademySummary
                       {
                           Name = outgoingAcademy.Name,
                           Ukprn = transferring.OutgoingAcademyUkprn,
                           Urn = outgoingAcademy.Urn
                       };

                       return transferring;
                   })
                   .Select(t => t.Result)
                   .ToList();

                #endregion

                Project mappedProject = _externalToInternalProjectMapper.Map(project);

                return new RepositoryResult<Project>
                {
                    Result = mappedProject
                };
            }

            throw new TramsApiException(response);
        }

        //public async Task<RepositoryResult<Project>> Update(Project project)
        //{
        //    TramsProjectUpdate externalProject = _internalToUpdateMapper.Map(project);
        //    var content = new StringContent(JsonConvert.SerializeObject(externalProject), Encoding.Default,
        //       "application/json");
        //    HttpResponseMessage response = await _httpClient.PatchAsync($"academyTransferProject/{project.Urn}", content);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var apiResponse = await response.Content.ReadAsStringAsync();
        //        var createdProject = JsonConvert.DeserializeObject<TramsProject>(apiResponse);

        //        #region API Interim

        //        createdProject.OutgoingTrust = new TrustSummary { Ukprn = createdProject.OutgoingTrustUkprn };
        //        createdProject.TransferringAcademies = createdProject.TransferringAcademies.Select(async transferring =>
        //           {
        //               RepositoryResult<Academy> outgoingAcademy =
        //              await _academies.GetAcademyByUkprn(transferring.OutgoingAcademyUkprn);

        //               transferring.IncomingTrust = new TrustSummary { Ukprn = transferring.IncomingTrustUkprn };
        //               transferring.OutgoingAcademy = new AcademySummary
        //               {
        //                   Name = outgoingAcademy.Result.Name,
        //                   Ukprn = transferring.OutgoingAcademyUkprn,
        //                   Urn = outgoingAcademy.Result.Urn
        //               };

        //               return transferring;
        //           })
        //           .Select(t => t.Result)
        //           .ToList();

        //        #endregion

        //        return new RepositoryResult<Project>
        //        {
        //            Result = _externalToInternalProjectMapper.Map(createdProject)
        //        };
        //    }

        //    throw new TramsApiException(response);
        //}

        public async Task<bool> UpdateRationale(Project project)
        {
            var rationale = InternalProjectToUpdateMapper.Rationale(project);
            //need to map to the command here to pull the rationale out

            var content = new StringContent(JsonConvert.SerializeObject(rationale), Encoding.Default,
               "application/json");
            HttpResponseMessage response = await _academisationHttpClient.PutAsync($"transfer-project/{project.Urn}/set-rationale", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // stay inline with current pattern
            throw new TramsApiException(response);
        }

        public async Task<bool> UpdateFeatures(Project project)
        {
            var features = InternalProjectToUpdateMapper.Features(project);
            //need to map to the command here to pull the rationale out

            var content = new StringContent(JsonConvert.SerializeObject(features), Encoding.Default,
               "application/json");
            HttpResponseMessage response = await _academisationHttpClient.PutAsync($"transfer-project/{project.Urn}/set-features", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // stay inline with current pattern
            throw new TramsApiException(response);
        }

        public async Task<bool> UpdateBenefits(Project project)
        {
            var benefits = InternalProjectToUpdateMapper.Benefits(project);
            //need to map to the command here to pull the rationale out

            var content = new StringContent(JsonConvert.SerializeObject(benefits), Encoding.Default,
               "application/json");
            HttpResponseMessage response = await _academisationHttpClient.PutAsync($"transfer-project/{project.Urn}/set-benefits", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // stay inline with current pattern
            throw new TramsApiException(response);
        }

        public async Task<bool> UpdateGeneralInfomation(Project project)
        {
            var generalInformation = InternalProjectToUpdateMapper.GeneralInformation(project);
            //need to map to the command here to pull the rationale out

            var content = new StringContent(JsonConvert.SerializeObject(generalInformation), Encoding.Default,
               "application/json");
            HttpResponseMessage response = await _academisationHttpClient.PutAsync($"transfer-project/{project.Urn}/set-general-information", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // stay inline with current pattern
            throw new TramsApiException(response);
        }

        public async Task<bool> UpdateLegalRequirements(Project project)
        {
            var legalRequirements = InternalProjectToUpdateMapper.LegalRequirements(project);
            //need to map to the command here to pull the rationale out

            var content = new StringContent(JsonConvert.SerializeObject(legalRequirements), Encoding.Default,
               "application/json");
            HttpResponseMessage response = await _academisationHttpClient.PutAsync($"transfer-project/{project.Urn}/set-legal-requirements", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // stay inline with current pattern
            throw new TramsApiException(response);
        }

        public async Task<bool> UpdateDates(Project project)
        {
            var dates = InternalProjectToUpdateMapper.Dates(project);
            //need to map to the command here to pull the rationale out

            var content = new StringContent(JsonConvert.SerializeObject(dates), Encoding.Default,
               "application/json");
            HttpResponseMessage response = await _academisationHttpClient.PutAsync($"transfer-project/{project.Urn}/set-transfer-dates", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // stay inline with current pattern
            throw new TramsApiException(response);
        }

        public async Task<bool> UpdateAcademy(string projectUrn, TransferringAcademies transferringAcademy)
        {
            var academy = InternalProjectToUpdateMapper.TransferringAcademy(transferringAcademy);
            //need to map to the command here to pull the rationale out

            var content = new StringContent(JsonConvert.SerializeObject(academy), Encoding.Default,
               "application/json");
            HttpResponseMessage response = await _academisationHttpClient.PutAsync($"transfer-project/{projectUrn}/set-school-additional-data", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // stay inline with current pattern
            throw new TramsApiException(response);
        }

        public async Task<RepositoryResult<Project>> Create(Project project)
        {
            var externalProject = InternalProjectToUpdateMapper.MapToCreate(project);
            var content = new StringContent(JsonConvert.SerializeObject(externalProject), Encoding.Default,
               "application/json");
            HttpResponseMessage response = await _academisationHttpClient.PostAsync("transfer-project", content);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var createdProject = JsonConvert.DeserializeObject<AcademisationProject>(apiResponse);

                #region API Interim

                createdProject.OutgoingTrust = new TrustSummary { Ukprn = createdProject.OutgoingTrustUkprn };

                createdProject.TransferringAcademies = createdProject.TransferringAcademies.Select(async transferring =>
                   {
                       Academy outgoingAcademy =
                      await _academies.GetAcademyByUkprn(transferring.OutgoingAcademyUkprn);
                       transferring.IncomingTrust = new TrustSummary
                       {
                           Ukprn = transferring.IncomingTrustUkprn
                       };
                       transferring.OutgoingAcademy = new AcademySummary
                       {
                           Name = outgoingAcademy.Name,
                           Ukprn = transferring.OutgoingAcademyUkprn,
                           Urn = outgoingAcademy.Urn
                       };

                       return transferring;
                   })
                   .Select(t => t.Result)
                   .ToList();

                #endregion

                return new RepositoryResult<Project>
                {
                    Result = _externalToInternalProjectMapper.Map(createdProject)
                };
            }

            throw new TramsApiException(response);
        }

        public async Task<bool> AssignUser(Project project)
        {
            dynamic user = new ExpandoObject();

            user.userId = project.AssignedUser.Id;
            user.userEmail = project.AssignedUser.EmailAddress;
            user.userFullName = project.AssignedUser.FullName;

            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.Default,
               "application/json");
            HttpResponseMessage response = await _academisationHttpClient.PutAsync($"transfer-project/{project.Urn}/assign-user", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // stay inline with current pattern
            throw new TramsApiException(response);
        }

        public async Task<ApiResponse<FileStreamResult>> DownloadProjectExport(GetProjectSearchModel searchModel)
        {
            var response = await _academisationHttpClient.PostAsync("/export/export-transfer-projects", JsonContent.Create(searchModel));

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<FileStreamResult>(response.StatusCode, null);
            }

            var stream = await response.Content.ReadAsStreamAsync();
            FileStreamResult fileStreamResult = new(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            return new ApiResponse<FileStreamResult>(response.StatusCode, fileStreamResult);
        }

        public async Task<bool> UpdateStatus(Project project)
        {
            var status = new
            {
                Status = project.Status
            };

            var content = new StringContent(JsonConvert.SerializeObject(status), Encoding.Default,
               "application/json");
            HttpResponseMessage response = await _academisationHttpClient.PutAsync($"transfer-project/{project.Urn}/set-status", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // stay inline with current pattern
            throw new TramsApiException(response);
        }

        public async Task<bool> UpdateIncomingTrustName(string urn, string name)
        {
            var projectName = new
            {
                ProjectName = name
            };

            var content = new StringContent(JsonConvert.SerializeObject(projectName), Encoding.Default,
               "application/json");
            HttpResponseMessage response = await _academisationHttpClient.PutAsync($"transfer-project/{urn}/set-name", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // stay inline with current pattern
            throw new TramsApiException(response);
        }

        public async Task<ApiResponse<ProjectFilterParameters>> GetFilterParameters()
        {
            HttpResponseMessage response = await _academisationHttpClient.GetAsync("/legacy/projects/status");

            if (response.IsSuccessStatusCode is false)
            {
                throw new TramsApiException(response);
            }

            var apiResponse = await response.Content.ReadAsStringAsync();
            var filterParameters = JsonConvert.DeserializeObject<ProjectFilterParameters>(apiResponse);

            filterParameters.Statuses =
             filterParameters.Statuses.Select(x => _aliasedStatuses.ContainsKey(x.ToLowerInvariant()) ? _aliasedStatuses[x.ToLowerInvariant()] : x)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            filterParameters.Regions = new List<string>
            {
                "East Midlands",
                "East of England",
                "London",
                "North East",
                "North West",
                "South East",
                "South West",
                "West Midlands",
                "Yorkshire and the Humber"
            };

            return new ApiResponse<ProjectFilterParameters>(response.StatusCode, filterParameters);
        }
    }
}
