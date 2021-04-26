using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class Provider
    {
        [JsonProperty("URN")] public string Urn { get; set; }
        [JsonProperty("UKPRN")] public string Ukprn { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("group")] public string Group { get; set; }
    }
}