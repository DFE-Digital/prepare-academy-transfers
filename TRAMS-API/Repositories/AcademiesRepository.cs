using API.HttpHelpers;
using API.Models.Downstream.D365;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Mapping;
using API.Models.Upstream.Response;

namespace API.Repositories
{
    public class AcademiesRepository : IAcademiesRepository
    {
        private const string Route = "accounts";

        private readonly IAuthenticatedHttpClient _client;
        private readonly IOdataUrlBuilder<GetAcademiesD365Model> _urlBuilder;
        private readonly ILogger<AcademiesRepository> _logger;
        private readonly IMapper<GetAcademiesD365Model, GetAcademiesModel> _getAcademies365Mapper;

        public AcademiesRepository(IAuthenticatedHttpClient client,
            IOdataUrlBuilder<GetAcademiesD365Model> urlBuilder,
            ILogger<AcademiesRepository> logger,
            IMapper<GetAcademiesD365Model, GetAcademiesModel> getAcademies365Mapper)
        {
            _client = client;
            _urlBuilder = urlBuilder;
            _logger = logger;
            _getAcademies365Mapper = getAcademies365Mapper;
        }

        public async Task<RepositoryResult<GetAcademiesModel>> GetAcademyById(Guid id)
        {
            var url = _urlBuilder.BuildRetrieveOneUrl(Route, id);

            var response = await _client.GetAsync(url);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;

            if (responseStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new RepositoryResult<GetAcademiesModel> {Result = null};
            }

            if (response.IsSuccessStatusCode)
            {
                var castedResult = JsonConvert.DeserializeObject<GetAcademiesD365Model>(responseContent);

                if (castedResult?.ParentTrustId == null)
                {
                    return new RepositoryResult<GetAcademiesModel> {Result = null};
                }

                var mappedResult = _getAcademies365Mapper.Map(castedResult);
                return new RepositoryResult<GetAcademiesModel> {Result = mappedResult};
            }

            //At this point, log the error and configure the repository result to inform the caller that the repo failed
            _logger.LogError(RepositoryErrorMessages.RepositoryErrorLogFormat, responseStatusCode, responseContent);

            return new RepositoryResult<GetAcademiesModel>
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = responseStatusCode,
                    ErrorMessage = responseContent
                }
            };
        }

        public async Task<RepositoryResult<List<GetAcademiesModel>>> GetAcademiesByTrustId(Guid id)
        {
            var parentTrustIdFieldName =
                _urlBuilder.GetPropertyAnnotation(nameof(GetAcademiesD365Model.ParentTrustId));
            var filters = new List<string>
            {
                $"({parentTrustIdFieldName} eq {id})"
            };

            var url = _urlBuilder.BuildFilterUrl(Route, filters);

            var response = await _client.GetAsync(url);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                var castedResults = JsonConvert.DeserializeObject<ResultSet<GetAcademiesD365Model>>(responseContent);
                var mappedResults = castedResults.Items.Select(item => _getAcademies365Mapper.Map(item)).ToList();

                return new RepositoryResult<List<GetAcademiesModel>> {Result = mappedResults};
            }

            //At this point, log the error and configure the repository result to inform the caller that the repo failed
            _logger.LogError(RepositoryErrorMessages.RepositoryErrorLogFormat, responseStatusCode, responseContent);

            return new RepositoryResult<List<GetAcademiesModel>>
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