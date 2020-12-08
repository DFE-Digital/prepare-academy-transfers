using API.HttpHelpers;
using API.Mapping;
using API.Models.GET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static API.Constants.D365Constants;

namespace API.Repositories
{
    public class TrustRepository
    {
        private static readonly string _route = "accounts";
        private static readonly string establishmentTypeFieldName = JsonFieldExtractor
            .GetPropertyAnnotation(typeof(GetTrustD365Model), nameof(GetTrustD365Model.EstablishmentType));
        private static readonly string trustNameFieldName = JsonFieldExtractor
            .GetPropertyAnnotation(typeof(GetTrustD365Model), nameof(GetTrustD365Model.TrustName));
        private static readonly string companiesHouseFieldName = JsonFieldExtractor
            .GetPropertyAnnotation(typeof(GetTrustD365Model), nameof(GetTrustD365Model.CompaniesHouseNumber));
        private static readonly string trustReferenceNumberFieldName = JsonFieldExtractor
            .GetPropertyAnnotation(typeof(GetTrustD365Model), nameof(GetTrustD365Model.TrustReferenceNumber));

        private readonly AuthenticatedHttpClient _client;

        public TrustRepository(AuthenticatedHttpClient client)
        {
            _client = client;
        }

        public async Task<GetTrustD365Model> GetTrustById(Guid id)
        {
            var fields = JsonFieldExtractor.GetAllFieldAnnotations(typeof(GetTrustD365Model));

            await _client.AuthenticateAsync();

            var url = ODataUrlBuilder.BuildUrl(_route, id, fields);

            var response = await _client.GetAsync(url);
            var content = await response.Content?.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var castedResult = JsonConvert.DeserializeObject<GetTrustD365Model>(result);

                return castedResult;
            }

            return null;
        }

        public async Task<List<GetTrustD365Model>> SearchTrusts(string searchQuery)
        {
            var fields = JsonFieldExtractor.GetAllFieldAnnotations(typeof(GetTrustD365Model));

            List<string> filters = BuildSearchFilters(searchQuery);

            await _client.AuthenticateAsync();

            var url = ODataUrlBuilder.BuildUrl(_route, fields, filters);

            var response = await _client.GetAsync(url);
            var content = await response.Content?.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var castedResults = JsonConvert.DeserializeObject<ResultSet<GetTrustD365Model>>(results);

                return castedResults.Items;
            }

            return new List<GetTrustD365Model>();
        }

        private List<string> BuildSearchFilters(string searchQuery)
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
