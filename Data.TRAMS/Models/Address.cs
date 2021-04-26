using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class Address
    {
        [JsonProperty("street")] public string Street { get; set; }
        [JsonProperty("locality")] public string Locality { get; set; }
        [JsonProperty("additional_line")] public string AdditionalLine { get; set; }
        [JsonProperty("town")] public string Town { get; set; }
        [JsonProperty("county")] public string County { get; set; }
        [JsonProperty("postcode")] public string Postcode { get; set; }
    }
}