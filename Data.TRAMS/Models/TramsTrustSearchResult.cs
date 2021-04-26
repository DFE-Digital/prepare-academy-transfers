using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class TramsTrustSearchResult
    {
        [JsonProperty("urn")] public string Urn { get; set; }
        [JsonProperty("group_name")] public string GroupName { get; set; }

        [JsonProperty("companies_house_number")]
        public string CompaniesHouseNumber { get; set; }

        [JsonProperty("academies")] public List<TrustSearchAcademies> Academies { get; set; }
    }

    public class TrustSearchAcademies
    {
        [JsonProperty("urn")] public string Urn { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
    }
}