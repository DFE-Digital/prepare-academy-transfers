using API.HttpHelpers;
using API.Mapping;
using API.Models.D365;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class AcademiesRepository
    {
        private static readonly string _route = "accounts";

        private static readonly string _parentTrustIdFieldName = JsonFieldExtractor
            .GetPropertyAnnotation(typeof(GetAcademiesD365Model), nameof(GetAcademiesD365Model.ParentTrustId));

        private readonly AuthenticatedHttpClient _client;

        public AcademiesRepository(AuthenticatedHttpClient client)
        {
            _client = client;
        }

        public async Task<List<GetAcademiesD365Model>> GetAcademiesByTrustId(Guid id)
        {
            var fields = JsonFieldExtractor.GetAllFieldAnnotations(typeof(GetAcademiesD365Model));

            var filters = new List<string>
            {
                $"({_parentTrustIdFieldName} eq {id})"
            };

            var url = ODataUrlBuilder.BuildFilterUrl(_route, fields, filters);

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
