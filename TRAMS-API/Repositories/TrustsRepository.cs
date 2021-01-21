using API.HttpHelpers;
using API.Models.D365;
using API.Models.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static API.Constants.D365Constants;

namespace API.Repositories
{
    public class TrustsRepository : ITrustsRepository
    {
        private readonly string _route = "accounts";

        private readonly IOdataUrlBuilder<GetTrustsD365Model> _urlBuilder;
        private readonly IAuthenticatedHttpClient _client;
        private readonly IODataSanitizer _oDataSanitizer;
        private readonly ILogger<TrustsRepository> _logger;

        public TrustsRepository(IAuthenticatedHttpClient client, 
                                IOdataUrlBuilder<GetTrustsD365Model> urlBuilder,
                                IODataSanitizer oDataSanitizer,
                                ILogger<TrustsRepository> logger)
        {
            _client = client;
            _urlBuilder = urlBuilder;
            _oDataSanitizer = oDataSanitizer;
            _logger = logger;
        }

        public async Task<RepositoryResult<GetTrustsD365Model>> GetTrustById(Guid id)
        {
            await _client.AuthenticateAsync();

            var url = _urlBuilder.BuildRetrieveOneUrl(_route, id);

            var response = await _client.GetAsync(url);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;

            if (responseStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new RepositoryResult<GetTrustsD365Model> { Result = null };
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

                if (!allowedEstablishementTypeIds.Contains(castedResult.EstablishmentTypeId.ToString().ToUpperInvariant()))
                {
                    return new RepositoryResult<GetTrustsD365Model> { Result = null };
                }

                return new RepositoryResult<GetTrustsD365Model> { Result = castedResult };
            }

            //At this point, log the error and configure the repository result to inform the caller that the repo failed
            _logger.LogError(RepositoryErrorMessages.RepositoryErrorLogFormat, responseStatusCode, responseContent);

            return new RepositoryResult<GetTrustsD365Model>
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = responseStatusCode,
                    ErrorMessage = responseContent
                }
            };
        }

        public async Task<RepositoryResult<List<GetTrustsD365Model>>> SearchTrusts(string searchQuery)
        {
            var sanitizedQuery = _oDataSanitizer.Sanitize(searchQuery);
            var filters = BuildTrustSearchFilters(sanitizedQuery);

            await _client.AuthenticateAsync();

            var url = _urlBuilder.BuildFilterUrl(_route, filters);

            var response = await _client.GetAsync(url);
            var responseContent = await response.Content?.ReadAsStringAsync();
            var responseStatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
            { 
                var castedResults = JsonConvert.DeserializeObject<ResultSet<GetTrustsD365Model>>(responseContent);

                return new RepositoryResult<List<GetTrustsD365Model>> { Result = castedResults.Items };
            }

            //At this point, log the error and configure the repository result to inform the caller that the repo failed
            _logger.LogError(RepositoryErrorMessages.RepositoryErrorLogFormat, responseStatusCode, responseContent);

            return new RepositoryResult<List<GetTrustsD365Model>>
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
            var establishmentTypeFieldName = _urlBuilder.GetPropertyAnnotation(nameof(GetTrustsD365Model.EstablishmentType));
            var trustNameFieldName = _urlBuilder.GetPropertyAnnotation(nameof(GetTrustsD365Model.TrustName));
            var companiesHouseFieldName = _urlBuilder.GetPropertyAnnotation(nameof(GetTrustsD365Model.CompaniesHouseNumber));
            var trustReferenceNumberFieldName = _urlBuilder.GetPropertyAnnotation(nameof(GetTrustsD365Model.TrustReferenceNumber));

            var allowedEstablishementTypeIds = new List<string>()
            {
                EstablishmentType.MultiAcademyTrustGuid,
                EstablishmentType.SingleAcademyTrustGuid,
                EstablishmentType.TrustGuid
            };

            var allowedStatusCodes = new List<string>()
            {
                ((int)TrustStatusReason.Open).ToString(),
                ((int)TrustStatusReason.OpenButProposedToClose).ToString()
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
