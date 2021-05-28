using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class Census
    {
        public string CensusDate { get; set; }
        public string NumberOfBoys { get; set; }
        public string NumberOfGirls { get; set; }
        public string NumberOfPupils { get; set; }
        public string PercentageFsm { get; set; }
    }
}