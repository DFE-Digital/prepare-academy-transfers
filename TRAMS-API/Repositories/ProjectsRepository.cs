using API.HttpHelpers;
using API.Models.D365;
using API.Models.Downstream.D365;
using API.Models.Response;
using API.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class ProjectsRepository : IProjectsRepository
    {
        private static readonly string _route = "sip_academytransfersprojects";

        private readonly IAuthenticatedHttpClient _client;
        private readonly IOdataUrlBuilder<GetProjectsD365Model> _urlBuilder;
        private readonly ILogger<ProjectsRepository> _logger;
        private readonly IOdataUrlBuilder<AcademyTransfersProjectAcademy> _projectAcademyUrlBuilder;

        public ProjectsRepository(IAuthenticatedHttpClient client,
                                  IOdataUrlBuilder<GetProjectsD365Model> urlBuilder,
                                  IOdataUrlBuilder<AcademyTransfersProjectAcademy> projectAcademyUrlBuilder,
                                  ILogger<ProjectsRepository> logger)
        {
            _client = client;
            _urlBuilder = urlBuilder;
            _projectAcademyUrlBuilder = projectAcademyUrlBuilder;
            _logger = logger;
        }

        public async Task<List<GetProjectsD365Model>> GetAll()
        {
            var url = _urlBuilder.BuildFilterUrl(_route, null);

            await _client.AuthenticateAsync();

            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var castedResults = JsonConvert.DeserializeObject<ResultSet<GetProjectsD365Model>>(results);

                return castedResults.Items;
            }

            return new List<GetProjectsD365Model>();
        }

        public async Task<RepositoryResult<AcademyTransfersProjectAcademy>> GetProjectAcademyById(Guid id)
        {
            await _client.AuthenticateAsync();

            var url = _projectAcademyUrlBuilder.BuildRetrieveOneUrl("sip_academytransfersprojectacademies", id);

            var response = await _client.GetAsync(url);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;

            if (responseStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new RepositoryResult<AcademyTransfersProjectAcademy> { Result = null };
            }

            if (response.IsSuccessStatusCode)
            {
                var castedResults = JsonConvert.DeserializeObject<AcademyTransfersProjectAcademy>(responseContent);

                return new RepositoryResult<AcademyTransfersProjectAcademy> { Result = castedResults };
            }

            //At this point, log the error and configure the repository result to inform the caller that the repo failed
            _logger.LogError(RepositoryErrorMessages.RepositoryErrorLogFormat, responseStatusCode, responseContent);

            return new RepositoryResult<AcademyTransfersProjectAcademy>
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = responseStatusCode,
                    ErrorMessage = responseContent
                }
            };
        }

        public async Task<RepositoryResult<GetProjectsD365Model>> GetProjectById(Guid id)
        {
            var url = _urlBuilder.BuildRetrieveOneUrl(_route, id);

            await _client.AuthenticateAsync();

            var response = await _client.GetAsync(url);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;

            if (responseStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new RepositoryResult<GetProjectsD365Model> { Result = null };
            }

            if (response.IsSuccessStatusCode)
            { 
                var castedResults = JsonConvert.DeserializeObject<GetProjectsD365Model>(responseContent);

                return new RepositoryResult<GetProjectsD365Model> { Result = castedResults };
            }

            //At this point, log the error and configure the repository result to inform the caller that the repo failed
            _logger.LogError(RepositoryErrorMessages.RepositoryErrorLogFormat, responseStatusCode, responseContent);

            return new RepositoryResult<GetProjectsD365Model>
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = responseStatusCode,
                    ErrorMessage = responseContent
                }
            };
        }

        public async Task<RepositoryResult<Guid?>> InsertProject(PostAcademyTransfersProjectsD365Model project)
        {
            await _client.AuthenticateAsync();

            var jsonBody = JsonConvert.SerializeObject(project);

            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _client.PostAsync(_route, byteContent);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
                if (response.Headers.TryGetValues("OData-EntityId", out var headerValues))
                {
                    var value = headerValues.First();
                    var guidString = value.Substring(value.Length - 37, 36);

                    if (Guid.TryParse(guidString, out var guidValue))
                    {
                        return new RepositoryResult<Guid?> { Result = guidValue };
                    }
                }

            //At this point, log the error and configure the repository result to inform the caller that the repo failed
            _logger.LogError(RepositoryErrorMessages.RepositoryErrorLogFormat, responseStatusCode, responseContent);

            return new RepositoryResult<Guid?>
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = responseStatusCode,
                    ErrorMessage = responseContent
                }
            };
        }
    }
}
