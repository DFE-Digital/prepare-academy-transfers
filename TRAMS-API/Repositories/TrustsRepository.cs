using API.HttpHelpers;
using API.Models.D365;
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
        private readonly AuthenticatedHttpClient _client;

        public TrustsRepository(AuthenticatedHttpClient client, IOdataUrlBuilder<GetTrustsD365Model> urlBuilder)
        {
            _client = client;
            _urlBuilder = urlBuilder;
        }

        public async Task<GetTrustsD365Model> GetTrustById(Guid id)
        {
            await _client.AuthenticateAsync();

            var url = _urlBuilder.BuildRetrieveOneUrl(_route, id);

            var response = await _client.GetAsync(url);
            var content = await response.Content?.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var castedResult = JsonConvert.DeserializeObject<GetTrustsD365Model>(result);

                //If this establishment is not a trust, return null. The controller will return a Not Found response
                var allowedEstablishementTypeIds = new List<string>()
                {
                    EstablishmentType.MultiAcademyTrustGuid,
                    EstablishmentType.SingleAcademyTrustGuid,
                    EstablishmentType.TrustGuid
                };

                if (!allowedEstablishementTypeIds.Contains(castedResult.EstablishmentTypeId.ToString().ToUpperInvariant()))
                {
                    return null;
                }

                return castedResult;
            }

            return null;
        }

        public async Task<List<GetTrustsD365Model>> SearchTrusts(string searchQuery)
        {
            var filters = BuildTrustSearchFilters(searchQuery);

            await _client.AuthenticateAsync();

            var url = _urlBuilder.BuildFilterUrl(_route, filters);

            var response = await _client.GetAsync(url);
            var content = await response.Content?.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var castedResults = JsonConvert.DeserializeObject<ResultSet<GetTrustsD365Model>>(results);

                return castedResults.Items;
            }

            return new List<GetTrustsD365Model>();
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
