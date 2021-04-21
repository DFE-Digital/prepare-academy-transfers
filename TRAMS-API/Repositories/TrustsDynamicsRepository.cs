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
using API.Repositories.Interfaces;
using static API.Constants.D365Constants;

namespace API.Repositories
{
    public class TrustsDynamicsRepository : ITrustsRepository
    {
        private const string Route = "accounts";

        private readonly IOdataUrlBuilder<GetTrustsD365Model> _urlBuilder;
        private readonly IAuthenticatedHttpClient _client;
        private readonly IODataSanitizer _oDataSanitizer;
        private readonly ILogger<TrustsDynamicsRepository> _logger;
        private readonly IMapper<GetTrustsD365Model, GetTrustsModel> _getTrustMapper;

        public TrustsDynamicsRepository(IAuthenticatedHttpClient client,
            IOdataUrlBuilder<GetTrustsD365Model> urlBuilder,
            IODataSanitizer oDataSanitizer,
            ILogger<TrustsDynamicsRepository> logger, IMapper<GetTrustsD365Model, GetTrustsModel> getTrustMapper)
        {
            _client = client;
            _urlBuilder = urlBuilder;
            _oDataSanitizer = oDataSanitizer;
            _logger = logger;
            _getTrustMapper = getTrustMapper;
        }

        public async Task<RepositoryResult<GetTrustsModel>> GetTrustById(Guid id)
        {
            var url = _urlBuilder.BuildRetrieveOneUrl(Route, id);

            var response = await _client.GetAsync(url);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;

            if (responseStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new RepositoryResult<GetTrustsModel> {Result = null};
            }

            if (response.IsSuccessStatusCode)
            {
                var castedResult = JsonConvert.DeserializeObject<GetTrustsD365Model>(responseContent);

                //If this establishment is not a trust, return null. The controller will return a Not Found response
                var allowedEstablishementTypeIds = new List<string>()
                {
                    EstablishmentType.MultiAcademyTrustGuid,
                    EstablishmentType.SingleAcademyTrustGuid,
                    EstablishmentType.TrustGuid
                };

                if (!allowedEstablishementTypeIds.Contains(castedResult.EstablishmentTypeId.ToString()
                    .ToUpperInvariant()))
                {
                    return new RepositoryResult<GetTrustsModel> {Result = null};
                }

                var mappedResult = _getTrustMapper.Map(castedResult);
                return new RepositoryResult<GetTrustsModel> {Result = mappedResult};
            }

            //At this point, log the error and configure the repository result to inform the caller that the repo failed
            _logger.LogError(RepositoryErrorMessages.RepositoryErrorLogFormat, responseStatusCode, responseContent);

            return new RepositoryResult<GetTrustsModel>
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = responseStatusCode,
                    ErrorMessage = responseContent
                }
            };
        }

        public async Task<RepositoryResult<List<GetTrustsModel>>> SearchTrusts(string searchQuery)
        {
            var sanitizedQuery = _oDataSanitizer.Sanitize(searchQuery);
            var filters = BuildTrustSearchFilters(sanitizedQuery);

            var url = _urlBuilder.BuildFilterUrl(Route, filters);

            var response = await _client.GetAsync(url);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                var castedResults = JsonConvert.DeserializeObject<ResultSet<GetTrustsD365Model>>(responseContent);
                var mappedResults = castedResults.Items.Select(r => _getTrustMapper.Map(r)).ToList();

                return new RepositoryResult<List<GetTrustsModel>> {Result = mappedResults};
            }

            //At this point, log the error and configure the repository result to inform the caller that the repo failed
            _logger.LogError(RepositoryErrorMessages.RepositoryErrorLogFormat, responseStatusCode, responseContent);

            return new RepositoryResult<List<GetTrustsModel>>
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = responseStatusCode,
                    ErrorMessage = responseContent
                }
            };
        }

        private List<string> BuildTrustSearchFilters(string searchQuery)
        {
            var stateCodeFieldName = _urlBuilder.GetPropertyAnnotation(nameof(GetTrustsD365Model.StateCode));
            var statusCodeFieldName = _urlBuilder.GetPropertyAnnotation(nameof(GetTrustsD365Model.StatusCode));
            var establishmentTypeFieldName =
                _urlBuilder.GetPropertyAnnotation(nameof(GetTrustsD365Model.EstablishmentType));
            var trustNameFieldName = _urlBuilder.GetPropertyAnnotation(nameof(GetTrustsD365Model.TrustName));
            var companiesHouseFieldName =
                _urlBuilder.GetPropertyAnnotation(nameof(GetTrustsD365Model.CompaniesHouseNumber));
            var trustReferenceNumberFieldName =
                _urlBuilder.GetPropertyAnnotation(nameof(GetTrustsD365Model.TrustReferenceNumber));

            var allowedEstablishementTypeIds = new List<string>()
            {
                EstablishmentType.MultiAcademyTrustGuid,
                EstablishmentType.SingleAcademyTrustGuid,
                EstablishmentType.TrustGuid
            };

            var allowedStatusCodes = new List<string>()
            {
                ((int) TrustStatusReason.Open).ToString(),
                ((int) TrustStatusReason.OpenButProposedToClose).ToString()
            };

            var filters = new List<string>()
            {
                //status should be active
                $"({stateCodeFieldName} eq 0) and",
                //Restrict establishment types
                $"{_urlBuilder.BuildInFilter(establishmentTypeFieldName, allowedEstablishementTypeIds)} and",
                //Restrict status codes
                $"{_urlBuilder.BuildInFilter(statusCodeFieldName, allowedStatusCodes)}"
            };

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var searchFields = new List<string>
                {
                    trustNameFieldName,
                    companiesHouseFieldName,
                    trustReferenceNumberFieldName
                };

                var reusableSearchFilter = $"and {_urlBuilder.BuildOrSearchQuery(searchQuery, searchFields)}";
                filters.Add(reusableSearchFilter);
            }

            return filters;
        }
    }
}