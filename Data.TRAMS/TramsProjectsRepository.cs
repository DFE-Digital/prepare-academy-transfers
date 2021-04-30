using System.Net.Http;
using System.Threading.Tasks;
using Data.Models;
using Data.TRAMS.Models;
using Newtonsoft.Json;

namespace Data.TRAMS
{
    public class TramsProjectsRepository : IProjects
    {
        private readonly ITramsHttpClient _httpClient;
        private readonly IMapper<TramsProject, Project> _externalToInternalProjectMapper;
        private readonly IMapper<Project, TramsProject> _internalToExternalProjectMapper;

        public TramsProjectsRepository(ITramsHttpClient httpClient,
            IMapper<TramsProject, Project> externalToInternalProjectMapper,
            IMapper<Project, TramsProject> internalToExternalProjectMapper)
        {
            _httpClient = httpClient;
            _externalToInternalProjectMapper = externalToInternalProjectMapper;
            _internalToExternalProjectMapper = internalToExternalProjectMapper;
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