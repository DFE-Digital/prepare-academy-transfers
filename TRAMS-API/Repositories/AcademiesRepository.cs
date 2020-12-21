using API.HttpHelpers;
using API.Mapping;
using API.Models.D365;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class AcademiesRepository : IAcademiesRepository
    {
        private static readonly string _route = "accounts";

        private readonly AuthenticatedHttpClient _client;
        private readonly IOdataUrlBuilder<GetAcademiesD365Model> _urlBuilder;

        public AcademiesRepository(AuthenticatedHttpClient client, 
                                   IOdataUrlBuilder<GetAcademiesD365Model> urlBuilder)
        {
            _client = client;
            _urlBuilder = urlBuilder;
        }

        public async Task<GetAcademiesD365Model> GetAcademyById(Guid id)
        {
            var url = _urlBuilder.BuildRetrieveOneUrl(_route, id);

            var response = await _client.GetAsync(url);
            var content = await response.Content?.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var castedResult = JsonConvert.DeserializeObject<GetAcademiesD365Model>(result);

                return castedResult;
            }

            return null;
        }

        public async Task<List<GetAcademiesD365Model>> GetAcademiesByTrustId(Guid id)
        { 
            var _parentTrustIdFieldName = _urlBuilder.GetPropertyAnnotation(nameof(GetAcademiesD365Model.ParentTrustId));
            var filters = new List<string>
            {
                $"({_parentTrustIdFieldName} eq {id})"
            };

            var url = _urlBuilder.BuildFilterUrl(_route, filters);

            var response = await _client.GetAsync(url);
            var content = await response.Content?.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var castedResults = JsonConvert.DeserializeObject<ResultSet<GetAcademiesD365Model>>(results);

                return castedResults.Items;
            }

            return new List<GetAcademiesD365Model>();
        }
    }
}
