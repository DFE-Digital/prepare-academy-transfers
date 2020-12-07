using API.HttpHelpers;
using API.Mapping;
using API.Models.GET;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class TrustRepository
    {
        private static string _route = "accounts";
        private readonly AuthenticatedHttpClient _client;

        public TrustRepository(AuthenticatedHttpClient client)
        {
            _client = client;
        }

        public async Task<List<GetTrustD365Model>> SearchTrusts()
        {
            var fields = JsonFieldExtractor.GetFields(typeof(GetTrustD365Model));

            List<string> filters = BuildFilters();

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

        private List<string> BuildFilters()
        {
            var allowedEstablishementTypeIds = new List<string>()
            {
                //Multi-academy trust
                "F0C125ED-6750-E911-A82E-000D3A385A17",
                //Single-academy trust
                "81014326-5D51-E911-A82E-000D3A385A17",
                //Trust
                "4EEAEE65-9A3E-E911-A828-000D3A385A1C"
            };

            var allowedStatusCodes = new List<string>()
            {
                "907660000",
                "907660002"
            };

            var filters = new List<string>()
            {
                //status should be active
                "(statecode eq 0) and",
                //Restrict establishment types
                $"{ODataUrlBuilder.BuildInFilter("_sip_establishmenttypeid_value", allowedEstablishementTypeIds)} and",
                //Restrict status codes
                $"{ODataUrlBuilder.BuildInFilter("statuscode", allowedStatusCodes)}"
            };

            return filters;
        }
    }
}
