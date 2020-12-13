using Newtonsoft.Json;
using System;

namespace API.Models.D365
{
    public class GetAcademiesD365Model : BaseD365Model
    {
        [JsonProperty("accountid")]
        public Guid Id { get; set; }

        [JsonProperty("_parentaccountid_value")]
        public Guid ParentTrustId { get; set; }

        [JsonProperty("name")]
        public string AcademyName { get; set; }

        [JsonProperty("sip_urn")]
        public string Urn { get; set; }

        [JsonProperty("address1_composite")]
        public string Address { get; set; }

        [JsonProperty("_sip_establishmenttypeid_value@OData.Community.Display.V1.FormattedValue")]
        public string EstablishmentType { get; set; }

        [JsonProperty("sip_localauthoritynumber")]
        public string LocalAuthorityNumber { get; set; }

        [JsonProperty("_sip_localauthorityareaid_value@OData.Community.Display.V1.FormattedValue")]
        public string LocalAuthorityName { get; set; }

        [JsonProperty("_sip_religiouscharacterid_value@OData.Community.Display.V1.FormattedValue")]
        public string ReligiousCharacter { get; set; }

        [JsonProperty("_sip_dioceseid_value@OData.Community.Display.V1.FormattedValue")]
        public string DioceseName { get; set; }

        [JsonProperty("_sip_religiousethosid_value@OData.Community.Display.V1.FormattedValue")]
        public string ReligiousEthos { get; set; }

        [JsonProperty("sip_ofstedrating@OData.Community.Display.V1.FormattedValue")]
        public string OftstedRating { get; set; }

        [JsonProperty("sip_ofstedinspectiondate")]
        public DateTime? OfstedInspectionDate { get; set; }

        [JsonProperty("sip_pfi")]
        public string Pfi { get; set; }

        [JsonProperty("sip_PredecessorEstablishment")]
        public PredecessorEstablishment Predecessor { get; set; }

        public class PredecessorEstablishment
        {
            [JsonProperty("sip_pfi")]
            public string Pfi { get; set; }
        }
    }
}
