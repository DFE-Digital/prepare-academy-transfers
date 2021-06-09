using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        private readonly IMapper<Project, TramsProject> _internalToExternalProjectMapper;
        private readonly IAcademies _academies;
        private readonly ITrusts _trusts;

        public TramsProjectsRepository(ITramsHttpClient httpClient,
            IMapper<TramsProject, Project> externalToInternalProjectMapper,
            IMapper<Project, TramsProject> internalToExternalProjectMapper,
            IMapper<TramsProjectSummary, ProjectSearchResult> summaryToInternalProjectMapper, IAcademies academies,
            ITrusts trusts)
        {
            _httpClient = httpClient;
            _externalToInternalProjectMapper = externalToInternalProjectMapper;
            _internalToExternalProjectMapper = internalToExternalProjectMapper;
            _summaryToInternalProjectMapper = summaryToInternalProjectMapper;
            _academies = academies;
            _trusts = trusts;
        }

        public async Task<RepositoryResult<List<ProjectSearchResult>>> GetProjects()
        {
            var response = await _httpClient.GetAsync("academyTransferProject");
            var apiResponse = await response.Content.ReadAsStringAsync();
            var summaries = JsonConvert.DeserializeObject<List<TramsProjectSummary>>(apiResponse);


            var mappedSummaries = summaries.Select(summary =>
                {
                    summary.OutgoingTrust = new TrustSummary {Ukprn = summary.OutgoingTrustUkprn};
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

        public async Task<RepositoryResult<Project>> GetByUrn(string urn)
        {
            var response = await _httpClient.GetAsync($"academyTransferProject/{urn}");
            var apiResponse = await response.Content.ReadAsStringAsync();
            var project = JsonConvert.DeserializeObject<TramsProject>(apiResponse);

            var mappedProject = _externalToInternalProjectMapper.Map(project);

            return new RepositoryResult<Project>
            {
                Result = mappedProject
            };
        }

        public async Task<RepositoryResult<Project>> Update(Project project)
        {
            var externalProject = _internalToExternalProjectMapper.Map(project);
            var content = new StringContent(JsonConvert.SerializeObject(externalProject));
            var response = await _httpClient.PatchAsync($"academyTransferProject/{project.Urn}", content);
            var apiResponse = await response.Content.ReadAsStringAsync();
            var createdProject = JsonConvert.DeserializeObject<TramsProject>(apiResponse);

            return new RepositoryResult<Project>()
            {
                Result = _externalToInternalProjectMapper.Map(createdProject)
            };
        }

        public async Task<RepositoryResult<Project>> Create(Project project)
        {
            var externalProject = _internalToExternalProjectMapper.Map(project);
            var content = new StringContent(JsonConvert.SerializeObject(externalProject));
            var response = await _httpClient.PostAsync("academyTransferProject", content);
            var apiResponse = await response.Content.ReadAsStringAsync();
            var createdProject = JsonConvert.DeserializeObject<TramsProject>(apiResponse);

            return new RepositoryResult<Project>()
            {
                Result = _externalToInternalProjectMapper.Map(createdProject)
            };
        }
    }
}