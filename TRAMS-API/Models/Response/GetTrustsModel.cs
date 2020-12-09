using Newtonsoft.Json;
using System;

namespace API.Models.Response
{
    public class GetTrustsModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("trustName")]
        public string TrustName { get; set; }

        [JsonProperty("companiesHouseNumber")]
        public string CompaniesHouseNumber { get; set; }

        [JsonProperty("trustReferenceNumber")]
        public string TrustReferenceNumber { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("estalishmentType")]
        public string EstablishmentType { get; set; }

        [JsonProperty("establishmentTypeGroup")]
        public string EstablishmentTypeGroup { get; set; }

        [JsonProperty("ukprn")]
        public string Ukprn { get; set; }

        [JsonProperty("upin")]
        public string Upin { get; set; }
    }
}
