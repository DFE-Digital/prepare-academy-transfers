using API.HttpHelpers;
using API.Models.D365.Enums;
using API.Models.Downstream.D365;

using API.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        private readonly IFetchXmlSanitizer _fetchXmlSanitizer;

        public ProjectsRepository(IAuthenticatedHttpClient client,
                                  IOdataUrlBuilder<GetProjectsD365Model> urlBuilder,
                                  IOdataUrlBuilder<AcademyTransfersProjectAcademy> projectAcademyUrlBuilder,
                                  IFetchXmlSanitizer fetchXmlSanitizer,
                                  ILogger<ProjectsRepository> logger)
        {
            _client = client;
            _urlBuilder = urlBuilder;
            _projectAcademyUrlBuilder = projectAcademyUrlBuilder;
            _fetchXmlSanitizer = fetchXmlSanitizer;
            _logger = logger;
        }

        public async Task<RepositoryResult<SearchProjectsD365PageModel>> SearchProject(string search,
                                                                                       ProjectStatusEnum status,
                                                                                       bool isAscending = true,
                                                                                       uint pageSize = 10,
                                                                                       uint pageNumber = 1)
        {
            var sanitizedSearch = _fetchXmlSanitizer.Sanitize(search);
                                      
            var fetchXml = BuildFetchXMLQuery(sanitizedSearch, status, isAscending);
            var encodedFetchXml = WebUtility.UrlEncode(fetchXml);

            var url = $"{_route}?fetchXml={encodedFetchXml}";

            var response = await _client.GetAsync(url);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var castedResults = JsonConvert.DeserializeObject<ResultSet<SearchProjectsD365Model>>(results);

                var distinctResults = castedResults.Items.Distinct().ToList();
                var totalPages = distinctResults.Count == 0 ? 0 : (distinctResults.Count / (int)pageSize) + 1;
                var pageResults = distinctResults
                                 .Skip(((int)pageNumber - 1) * (int)pageSize)
                                 .Take((int)pageSize)
                                 .ToList();

                var pageResult = new SearchProjectsD365PageModel
                {
                    CurrentPage = (int)pageNumber,
                    TotalPages = totalPages,
                    Projects = pageResults
                };

                return new RepositoryResult<SearchProjectsD365PageModel> { Result = pageResult };
            }

            //At this point, log the error and configure the repository result to inform the caller that the repo failed
            _logger.LogError(RepositoryErrorMessages.RepositoryErrorLogFormat, responseStatusCode, responseContent);

            return new RepositoryResult<SearchProjectsD365PageModel>
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = responseStatusCode,
                    ErrorMessage = responseContent
                }
            };
        }

        public async Task<RepositoryResult<AcademyTransfersProjectAcademy>> GetProjectAcademyById(Guid id)
        {
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

        public async Task<RepositoryResult<Guid?>> UpdateProjectAcademy(Guid id, PatchProjectAcademiesD365Model model)
        {
            var url = _projectAcademyUrlBuilder.BuildRetrieveOneUrl("sip_academytransfersprojectacademies", id);
            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _client.PatchAsync(url, content);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
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

        private static string BuildFetchXMLQuery(string search, ProjectStatusEnum status, bool isAscending)
        {
            var orderDirection = isAscending ? "" : "descending='true'";
            var fetchXml = new StringBuilder();

            fetchXml.AppendLine("<fetch distinct='true' mapping='logical' output-format='xml-platform' version='1.0'>");
            fetchXml.AppendLine("<entity name='sip_academytransfersproject'>");

            fetchXml.AppendLine("<attribute name='sip_academytransfersprojectid'/>");
            fetchXml.AppendLine("<attribute name='sip_projectname'/>");
            fetchXml.AppendLine("<attribute name='sip_projectinitiatorfullname'/>");
            fetchXml.AppendLine("<attribute name='sip_projectinitiatoruniqueid'/>");
            fetchXml.AppendLine("<attribute name='sip_projectstatus'/>");

            fetchXml.AppendLine("   <link-entity name='sip_academytransfersprojectacademy' from='sip_atprojectid' to='sip_academytransfersprojectid' link-type='outer' alias='projectAcademy'>");
            fetchXml.AppendLine("   <attribute name='sip_academytransfersprojectacademyid'/>");
            fetchXml.AppendLine("       <link-entity name='account' from='accountid' to='sip_academyid' link-type='outer' alias='academy'>");
            fetchXml.AppendLine("       <attribute name='name'/> ");
            fetchXml.AppendLine("           <link-entity name='account' from='accountid' to='parentaccountid' link-type='outer' alias='academyTrust'>");
            fetchXml.AppendLine("               <attribute name='name'/> ");
            fetchXml.AppendLine("               <attribute name='sip_companieshousenumber'/> ");
            fetchXml.AppendLine("           </link-entity>");
            fetchXml.AppendLine("       </link-entity>");
            fetchXml.AppendLine("   </link-entity>");

            if (status != default)
            {
                fetchXml.AppendLine("<filter type='or'>");
                fetchXml.AppendLine($"   <condition attribute = 'sip_projectstatus' operator='eq' value='{(int)status}' />");
                fetchXml.AppendLine("</filter >");
            }

            if (!string.IsNullOrEmpty(search))
            {
                fetchXml.AppendLine("<filter type='or'>");
                fetchXml.AppendLine($"   <condition entityname='academy' attribute = 'name' operator='like' value='%{search}%' />");
                fetchXml.AppendLine($"   <condition entityname='academyTrust' attribute = 'name' operator='like' value='%{search}%' />");
                fetchXml.AppendLine($"   <condition entityname='academyTrust' attribute = 'sip_companieshousenumber' operator='like' value='%{search}%' />");
                fetchXml.AppendLine($"   <condition attribute = 'sip_projectname' operator='like' value='%{search}%' />");
                fetchXml.AppendLine("</filter >");
            }
            
            fetchXml.AppendLine($"<order attribute='sip_projectname' {orderDirection} />");

            fetchXml.AppendLine("</entity>");
            fetchXml.AppendLine("</fetch>");
            return fetchXml.ToString();
        }
    }
}