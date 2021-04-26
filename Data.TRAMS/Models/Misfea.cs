using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class Misfea
    {
        [JsonProperty("provider")] public Provider Provider { get; set; }
        [JsonProperty("local_authority")] public string LocalAuthority { get; set; }
        [JsonProperty("region")] public string Region { get; set; }
        [JsonProperty("ofsted_region")] public string OfstedRegion { get; set; }

        [JsonProperty("date_of_latest_short_inspection")]
        public string DateOfLatestShortInspection { get; set; }

        [JsonProperty("number_of_short_inspections_since_last_full_inspection_RAW")]
        public string NumberOfShortInspectionsSinceLastFullInspectionRaw { get; set; }

        [JsonProperty("number_of_short_inspections_since_last_full_inspection")]
        public string NumberOfShortInspectionsSinceLastFullInspection { get; set; }

        [JsonProperty("inspection_number")] public string InspectionNumber { get; set; }
        [JsonProperty("inspection_type")] public string InspectionType { get; set; }

        [JsonProperty("first_day_of_inspection")]
        public string FirstDayOfInspection { get; set; }

        [JsonProperty("last_day_of_inspection")]
        public string LastDayOfInspection { get; set; }

        [JsonProperty("date_published")] public string DatePublished { get; set; }

        [JsonProperty("overall_effectiveness_RAW")]
        public string OverallEffectivenessRaw { get; set; }

        [JsonProperty("overall_effectiveness")]
        public string OverallEffectiveness { get; set; }

        [JsonProperty("quality_of_education_RAW")]
        public string QualityOfEducationRaw { get; set; }

        [JsonProperty("quality_of_education")] public string QualityOfEducation { get; set; }

        [JsonProperty("behaviour_and_attitudes_RAW")]
        public string BehaviourAndAttitudesRaw { get; set; }

        [JsonProperty("behaviour_and_attitudes")]
        public string BehaviourAndAttitudes { get; set; }

        [JsonProperty("personal_development_RAW")]
        public string PersonalDevelopmentRaw { get; set; }

        [JsonProperty("personal_development")] public string PersonalDevelopment { get; set; }

        [JsonProperty("effectiveness_of_leadership_and_management_RAW")]
        public string EffectivenessOfLeadershipAndManagementRaw { get; set; }

        [JsonProperty("effectiveness_of_leadership_and_management")]
        public string EffectivenessOfLeadershipAndManagement { get; set; }

        [JsonProperty("is_safeguarding_effective")]
        public string IsSafeguardingEffective { get; set; }

        [JsonProperty("previous_inspection_number")]
        public string PreviousInspectionNumber { get; set; }

        [JsonProperty("previous_last_day_of_inspection")]
        public string PreviousLastDayOfInspection { get; set; }

        [JsonProperty("previous_overall_effectiveness_RAW")]
        public string PreviousOverallEffectivenessRaw { get; set; }

        [JsonProperty("previous_overall_effectiveness")]
        public string PreviousOverallEffectiveness { get; set; }
    }
}