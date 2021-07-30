using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Data.TRAMS.Models;
using Data.TRAMS.Models.AcademyTransferProject;
using Newtonsoft.Json;

namespace Data.TRAMS
{
    public class TramsProjectsRepository : IProjects
    {
        private readonly ITramsHttpClient _httpClient;
        private readonly IMapper<TramsProject, Project> _externalToInternalProjectMapper;
        private readonly IMapper<TramsProjectSummary, ProjectSearchResult> _summaryToInternalProjectMapper;
        private readonly IMapper<Project, TramsProjectUpdate> _internalToUpdateMapper;
        private readonly IAcademies _academies;
        private readonly ITrusts _trusts;

        public TramsProjectsRepository(ITramsHttpClient httpClient,
            IMapper<TramsProject, Project> externalToInternalProjectMapper,
            IMapper<TramsProjectSummary, ProjectSearchResult> summaryToInternalProjectMapper, IAcademies academies,
            ITrusts trusts, IMapper<Project, TramsProjectUpdate> internalToUpdateMapper)
        {
            _httpClient = httpClient;
            _externalToInternalProjectMapper = externalToInternalProjectMapper;
            _summaryToInternalProjectMapper = summaryToInternalProjectMapper;
            _academies = academies;
            _trusts = trusts;
            _internalToUpdateMapper = internalToUpdateMapper;
        }

        public async Task<RepositoryResult<List<ProjectSearchResult>>> GetProjects(int page = 1)
        {
            var response = await _httpClient.GetAsync($"academyTransferProject?page={page}");
            
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var summaries = JsonConvert.DeserializeObject<List<TramsProjectSummary>>(apiResponse);


                var mappedSummaries = summaries.Select(summary =>
                {
                    summary.OutgoingTrust = new TrustSummary { Ukprn = summary.OutgoingTrustUkprn };
                    summary.TransferringAcademies = summary.TransferringAcademies.Select(async transferring =>
                    {
                        var incomingTrust = await _trusts.GetByUkprn(transferring.IncomingTrustUkprn);
                        var outgoingAcademy = await _academies.GetAcademyByUkprn(transferring.OutgoingAcademyUkprn);

                        transferring.IncomingTrust = new TrustSummary
                        {
                            GroupName = incomingTrust.Result.Name,
                            GroupId = incomingTrust.Result.GiasGroupId,
                            Ukprn = transferring.IncomingTrustUkprn
                        };

                        transferring.OutgoingAcademy = new AcademySummary
                        {
                            Name = outgoingAcademy.Result.Name,
                            Ukprn = transferring.OutgoingAcademyUkprn,
                            Urn = outgoingAcademy.Result.Urn
                        };

                        return transferring;
                    })
                        .Select(t => t.Result)
                        .ToList();

                    return _summaryToInternalProjectMapper.Map(summary);
                })
                    .ToList();

                return new RepositoryResult<List<ProjectSearchResult>>
                {
                    Result = mappedSummaries
                };
            }

            return CreateErrorResult<List<ProjectSearchResult>>(response);
        }

        public async Task<RepositoryResult<Project>> GetByUrn(string urn)
        {
            var response = await _httpClient.GetAsync($"academyTransferProject/{urn}");
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var project = JsonConvert.DeserializeObject<TramsProject>(apiResponse);

                #region API Interim

                project.OutgoingTrust = new TrustSummary { Ukprn = project.OutgoingTrustUkprn };
                project.TransferringAcademies = project.TransferringAcademies.Select(async transferring =>
                    {
                        var incomingTrust = await _trusts.GetByUkprn(transferring.IncomingTrustUkprn);
                        var outgoingAcademy = await _academies.GetAcademyByUkprn(transferring.OutgoingAcademyUkprn);

                        transferring.IncomingTrust = new TrustSummary
                        {
                            GroupName = incomingTrust.Result.Name,
                            GroupId = incomingTrust.Result.GiasGroupId,
                            Ukprn = transferring.IncomingTrustUkprn
                        };
                        transferring.OutgoingAcademy = new AcademySummary
                        {
                            Name = outgoingAcademy.Result.Name,
                            Ukprn = transferring.OutgoingAcademyUkprn,
                            Urn = outgoingAcademy.Result.Urn
                        };

                        return transferring;
                    })
                    .Select(t => t.Result)
                    .ToList();

                #endregion

                var mappedProject = _externalToInternalProjectMapper.Map(project);

                return new RepositoryResult<Project>
                {
                    Result = mappedProject
                };
            }

            return CreateErrorResult<Project>(response);
        }

        public async Task<RepositoryResult<Project>> Update(Project project)
        {
            var externalProject = _internalToUpdateMapper.Map(project);
            var content = new StringContent(JsonConvert.SerializeObject(externalProject), Encoding.Default,
                "application/json");
            var response = await _httpClient.PatchAsync($"academyTransferProject/{project.Urn}", content);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var createdProject = JsonConvert.DeserializeObject<TramsProject>(apiResponse);

                #region API Interim

                createdProject.OutgoingTrust = new TrustSummary { Ukprn = createdProject.OutgoingTrustUkprn };
                createdProject.TransferringAcademies = createdProject.TransferringAcademies.Select(async transferring =>
                    {
                        var outgoingAcademy = await _academies.GetAcademyByUkprn(transferring.OutgoingAcademyUkprn);

                        transferring.IncomingTrust = new TrustSummary() { Ukprn = transferring.IncomingTrustUkprn };
                        transferring.OutgoingAcademy = new AcademySummary
                        {
                            Name = outgoingAcademy.Result.Name,
                            Ukprn = transferring.OutgoingAcademyUkprn,
                            Urn = outgoingAcademy.Result.Urn
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

            return CreateErrorResult<Project>(response);
        }

        public async Task<RepositoryResult<Project>> Create(Project project)
        {
            var externalProject = _internalToUpdateMapper.Map(project);
            var content = new StringContent(JsonConvert.SerializeObject(externalProject), Encoding.Default,
                "application/json");
            var response = await _httpClient.PostAsync("academyTransferProject", content);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var createdProject = JsonConvert.DeserializeObject<TramsProject>(apiResponse);

                #region API Interim

                createdProject.OutgoingTrust = new TrustSummary { Ukprn = createdProject.OutgoingTrustUkprn };
                createdProject.TransferringAcademies = createdProject.TransferringAcademies.Select(async transferring =>
                    {
                        var outgoingAcademy = await _academies.GetAcademyByUkprn(transferring.OutgoingAcademyUkprn);

                        transferring.IncomingTrust = new TrustSummary { Ukprn = transferring.IncomingTrustUkprn };
                        transferring.OutgoingAcademy = new AcademySummary
                        {
                            Name = outgoingAcademy.Result.Name,
                            Ukprn = transferring.OutgoingAcademyUkprn,
                            Urn = outgoingAcademy.Result.Urn
                        };

                        return transferring;
                    })
                    .Select(t => t.Result)
                    .ToList();

                #endregion

                return new RepositoryResult<Project>()
                {
                    Result = _externalToInternalProjectMapper.Map(createdProject)
                };
            }

            return CreateErrorResult<Project>(response);
        }

        private RepositoryResult<T> CreateErrorResult<T>(HttpResponseMessage response)
        {
            var errorMessage = response.StatusCode == HttpStatusCode.NotFound
                                ? "Project not found"
                                : "API encountered an error";

            return new RepositoryResult<T>
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = response.StatusCode,
                    ErrorMessage = errorMessage
                }
            };
        }
    }
}