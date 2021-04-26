using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class NameAndCode
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("code")] public string Code { get; set; }
    }
}