using API.HttpHelpers;
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

        public async Task<List<GetAcademiesD365Model>> GetAcademiesByTrustId(Guid id)
        {
            var fields = _oDataHelper.GetSelectClause();
            var expandClause = _oDataHelper.GetExpandClause();
            var expand = "&$expand=sip_PredecessorEstablishment($select=sip_pfi,sip_predecessorurn,sip_urn)";
            var expand2 = "/api/data/v9.1/accounts?$select=_parentaccountid_value,_sip_establishmenttypeid_value,sip_pfi,_sip_predecessorestablishment_value,sip_predecessorurn&$expand=sip_PredecessorEstablishment($select=sip_pfi,sip_predecessorurn,sip_urn),sip_previoustrust($select=accountid)&$filter=_parentaccountid_value ne null&$count=true";

            var _parentTrustIdFieldName = _oDataHelper.GetPropertyAnnotation(nameof(GetAcademiesD365Model.ParentTrustId));
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
