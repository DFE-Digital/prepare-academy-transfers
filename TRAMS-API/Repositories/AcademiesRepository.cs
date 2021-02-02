using API.HttpHelpers;
using API.Models.Downstream.D365;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class AcademiesRepository : IAcademiesRepository
    {
        private static readonly string _route = "accounts";

        private readonly IAuthenticatedHttpClient _client;
        private readonly IOdataUrlBuilder<GetAcademiesD365Model> _urlBuilder;
        private readonly ILogger<AcademiesRepository> _logger;

        public AcademiesRepository(IAuthenticatedHttpClient client, 
                                   IOdataUrlBuilder<GetAcademiesD365Model> urlBuilder,
                                   ILogger<AcademiesRepository> logger)
        {
            _client = client;
            _urlBuilder = urlBuilder;
            _logger = logger;
        }

        public async Task<RepositoryResult<GetAcademiesD365Model>> GetAcademyById(Guid id)
        {
            var url = _urlBuilder.BuildRetrieveOneUrl(_route, id);

            var response = await _client.GetAsync(url);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;

            if (responseStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new RepositoryResult<GetAcademiesD365Model> { Result = null };
            }

            if (response.IsSuccessStatusCode)
            {
                var castedResult = JsonConvert.DeserializeObject<GetAcademiesD365Model>(responseContent);

                if (castedResult == null || castedResult.ParentTrustId == null)
                {
                    return new RepositoryResult<GetAcademiesD365Model> { Result = null };
                }

                return new RepositoryResult<GetAcademiesD365Model> { Result = castedResult };
            }

            //At this point, log the error and configure the repository result to inform the caller that the repo failed
            _logger.LogError(RepositoryErrorMessages.RepositoryErrorLogFormat, responseStatusCode, responseContent);
                 
            return new RepositoryResult<GetAcademiesD365Model>
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = responseStatusCode,
                    ErrorMessage = responseContent
                }
            };
        }

        public async Task<RepositoryResult<List<GetAcademiesD365Model>>> GetAcademiesByTrustId(Guid id)
        {
            var _parentTrustIdFieldName = _urlBuilder.GetPropertyAnnotation(nameof(GetAcademiesD365Model.ParentTrustId));
            var filters = new List<string>
            {
                $"({_parentTrustIdFieldName} eq {id})"
            };

            var url = _urlBuilder.BuildFilterUrl(_route, filters);

            var response = await _client.GetAsync(url);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                var castedResults = JsonConvert.DeserializeObject<ResultSet<GetAcademiesD365Model>>(responseContent);

                return new RepositoryResult<List<GetAcademiesD365Model>> { Result = castedResults.Items };
            }

            //At this point, log the error and configure the repository result to inform the caller that the repo failed
            _logger.LogError(RepositoryErrorMessages.RepositoryErrorLogFormat, responseStatusCode, responseContent);

            return new RepositoryResult<List<GetAcademiesD365Model>>
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
