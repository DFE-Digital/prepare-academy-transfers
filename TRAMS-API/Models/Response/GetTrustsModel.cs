using Newtonsoft.Json;
using System;

namespace API.Models.Response
{
    public class GetTrustsModel
    {
        /// <summary>
        /// The GUID of the Trust in TRAMS
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the trust
        /// </summary>
        /// <example>Sample Multi-Academy Trust</example>
        [JsonProperty("trustName")]
        public string TrustName { get; set; }

        /// <summary>
        /// The trust's Companies House Number
        /// </summary>
        /// <example>07658841</example>
        [JsonProperty("companiesHouseNumber")]
        public string CompaniesHouseNumber { get; set; }

        /// <summary>
        /// The trust's Reference Number
        /// </summary>
        /// /// <example>TR66684</example>
        [JsonProperty("trustReferenceNumber")]
        public string TrustReferenceNumber { get; set; }

        /// <summary>
        /// The multi-line address of the trust
        /// </summary>
        /// <example>Imaginary Trust \r\n Sample Road \r\n London SW14 5FJ</example>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// The trusts's establishment type
        /// </summary>
        /// <example>Multi-academy trust</example>
        [JsonProperty("estalishmentType")]
        public string EstablishmentType { get; set; }

        /// <summary>
        /// The trusts's establishment type group
        /// </summary>
        /// <example>Multi-academy trust</example>
        [JsonProperty("establishmentTypeGroup")]
        public string EstablishmentTypeGroup { get; set; }

        /// <summary>
        /// Trusts's UKPRN
        /// </summary>
        /// <example>10752526</example>
        [JsonProperty("ukprn")]
        public string Ukprn { get; set; }

        /// <summary>
        /// The trust's UPIN
        /// </summary>
        /// <example>142135</example>
        [JsonProperty("upin")]
        public string Upin { get; set; }
    }
}
