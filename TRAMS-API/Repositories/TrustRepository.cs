using API.HttpHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<string> SearchTrust()
        {
            var fields = new List<string>
            {
                "accountid",
                "name",
                "sip_companieshousenumber",
                "sip_compositeaddress",
                "_sip_establishmenttypeid_value",
                "_sip_establismenttypegroupid_value",
                "sip_trustreferencenumber",
                "sip_ukprn",
                "sip_upin",
                "sip_urn"
            };

            return null;
        }

        //var accountid = results.entities[i]["accountid"];
        //var name = results.entities[i]["name"];
        //var sip_companieshousenumber = results.entities[i]["sip_companieshousenumber"];
        //var sip_compositeaddress = results.entities[i]["sip_compositeaddress"];
        //var _sip_establishmenttypeid_value = results.entities[i]["_sip_establishmenttypeid_value"];
        //var _sip_establishmenttypeid_value_formatted = results.entities[i]["_sip_establishmenttypeid_value@OData.Community.Display.V1.FormattedValue"];
        //var _sip_establishmenttypeid_value_lookuplogicalname = results.entities[i]["_sip_establishmenttypeid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
        //var _sip_establismenttypegroupid_value = results.entities[i]["_sip_establismenttypegroupid_value"];
        //var _sip_establismenttypegroupid_value_formatted = results.entities[i]["_sip_establismenttypegroupid_value@OData.Community.Display.V1.FormattedValue"];
        //var _sip_establismenttypegroupid_value_lookuplogicalname = results.entities[i]["_sip_establismenttypegroupid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
        //var sip_trustreferencenumber = results.entities[i]["sip_trustreferencenumber"];
        //var sip_ukprn = results.entities[i]["sip_ukprn"];
        //var sip_upin = results.entities[i]["sip_upin"];
        //var sip_urn = results.entities[i]["sip_urn"];
    }
}
