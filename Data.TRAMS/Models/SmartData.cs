using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class SmartData
    {
        [JsonProperty("probability_of_declining")]
        public string ProbabilityOfDeclining { get; set; }

        [JsonProperty("probability_of_staying_the_same")]
        public string ProbabilityOfStayingTheSame { get; set; }

        [JsonProperty("probability_of_improving")]
        public string ProbabilityOfImproving { get; set; }

        [JsonProperty("predicted_change_in_progress_8_score")]
        public string PredictedChangeInProgress8Score { get; set; }

        [JsonProperty("predicted_chance_of_change_occurring")]
        public string PredictedChanceOfChangeOccurring { get; set; }

        [JsonProperty("total_number_of_risks")]
        public string TotalNumberOfRisks { get; set; }

        [JsonProperty("total_risk_score")] public string TotalRiskScore { get; set; }
        [JsonProperty("risk_rating_num")] public string RiskRatingNum { get; set; }
    }
}