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

        private readonly AuthenticatedHttpClient _client;
        private readonly IODataModelHelper<GetAcademiesD365Model> _oDataHelper;

        public AcademiesRepository(AuthenticatedHttpClient client, IODataModelHelper<GetAcademiesD365Model> oDataHelper)
        {
            _client = client;
            _oDataHelper = oDataHelper;
        }

        public async Task<GetAcademiesD365Model> GetAcademyById(Guid id)
        {
            var fields = _oDataHelper.GetSelectClause();
            var expandClause = _oDataHelper.GetExpandClause();

            var url = ODataUrlBuilder.BuildRetrieveOneUrl(_route, id, fields, expandClause);

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
            var fields = _oDataHelper.GetSelectClause();
            var expandClause = _oDataHelper.GetExpandClause();
            
            var _parentTrustIdFieldName = _oDataHelper.GetPropertyAnnotation(nameof(GetAcademiesD365Model.ParentTrustId));
            var filters = new List<string>
            {
                $"({_parentTrustIdFieldName} eq {id})"
            };

            var url = ODataUrlBuilder.BuildFilterUrl(_route, fields, expandClause, filters);

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
