using API.HttpHelpers;
using API.Mapping;
using API.Models.D365;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static API.Constants.D365Constants;

namespace API.Repositories
{
    public class TrustsRepository
    {
        private static readonly string _route = "accounts";
        private static readonly string establishmentTypeFieldName = JsonFieldExtractor
            .GetPropertyAnnotation(typeof(GetTrustsD365Model), nameof(GetTrustsD365Model.EstablishmentType));
        private static readonly string trustNameFieldName = JsonFieldExtractor
            .GetPropertyAnnotation(typeof(GetTrustsD365Model), nameof(GetTrustsD365Model.TrustName));
        private static readonly string companiesHouseFieldName = JsonFieldExtractor
            .GetPropertyAnnotation(typeof(GetTrustsD365Model), nameof(GetTrustsD365Model.CompaniesHouseNumber));
        private static readonly string trustReferenceNumberFieldName = JsonFieldExtractor
            .GetPropertyAnnotation(typeof(GetTrustsD365Model), nameof(GetTrustsD365Model.TrustReferenceNumber));

        private readonly AuthenticatedHttpClient _client;

        public TrustsRepository(AuthenticatedHttpClient client)
        {
            _client = client;
        }

        public async Task<GetTrustsD365Model> GetTrustById(Guid id)
        {
            var fields = JsonFieldExtractor.GetAllFieldAnnotations(typeof(GetTrustsD365Model));

            await _client.AuthenticateAsync();

            var url = ODataUrlBuilder.BuildRetrieveOneUrl(_route, id, fields);

            var response = await _client.GetAsync(url);
            var content = await response.Content?.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var castedResult = JsonConvert.DeserializeObject<GetTrustsD365Model>(result);

                return castedResult;
            }

            return null;
        }

        public async Task<List<GetTrustsD365Model>> SearchTrusts(string searchQuery)
        {
            var fields = JsonFieldExtractor.GetAllFieldAnnotations(typeof(GetTrustsD365Model));

            List<string> filters = BuildTrustSearchFilters(searchQuery);

            await _client.AuthenticateAsync();

            var url = ODataUrlBuilder.BuildFilterUrl(_route, fields, filters);

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
                $"({SharedFieldNames.StateCode} eq 0) and",
                //Restrict establishment types
                $"{ODataUrlBuilder.BuildInFilter(establishmentTypeFieldName, allowedEstablishementTypeIds)} and",
                //Restrict status codes
                $"{ODataUrlBuilder.BuildInFilter(SharedFieldNames.StatusCode, allowedStatusCodes)}"
            };

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var searchFields = new List<string>
                {
                    trustNameFieldName,
                    companiesHouseFieldName,
                    trustReferenceNumberFieldName
                };

                var reusableSearchFilter = $"and {ODataUrlBuilder.BuildOrSearchQuery(searchQuery, searchFields)}";
                filters.Add(reusableSearchFilter);
            }

            return filters;
        }
    }
}
