using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class Census
    {
        [JsonProperty("census_date")] public string CensusDate { get; set; }
        [JsonProperty("number_of_pupils")] public string NumberOfPupils { get; set; }
        [JsonProperty("number_of_boys")] public string NumberOfBoys { get; set; }
        [JsonProperty("number_of_girls")] public string NumberOfGirls { get; set; }
        [JsonProperty("percentage_fsm")] public string PercentageFsm { get; set; }
    }
}