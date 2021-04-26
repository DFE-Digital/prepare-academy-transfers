using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class MisEstablishment
    {
        [JsonProperty("site_name")] public string SiteName { get; set; }
        [JsonProperty("web_link")] public string WebLink { get; set; }
        [JsonProperty("LAESTAB")] public string Laestab { get; set; }
        [JsonProperty("school_name")] public string SchoolName { get; set; }
        [JsonProperty("ofsted_phase")] public string OfstedPhase { get; set; }
        [JsonProperty("type_of_education")] public string TypeOfEducation { get; set; }
        [JsonProperty("school_open_date")] public string SchoolOpenDate { get; set; }
        [JsonProperty("sixth_form")] public string SixthForm { get; set; }

        [JsonProperty("designated_religious_character")]
        public string DesignatedReligiousCharacter { get; set; }

        [JsonProperty("religious_ethos")] public string ReligiousEthos { get; set; }
        [JsonProperty("faith_grouping")] public string FaithGrouping { get; set; }
        [JsonProperty("ofsted_region")] public string OfstedRegion { get; set; }
        [JsonProperty("region")] public string Region { get; set; }
        [JsonProperty("local_authority")] public string LocalAuthority { get; set; }

        [JsonProperty("parliamentary_constituency")]
        public string ParliamentaryConstituency { get; set; }

        [JsonProperty("postcode")] public string Postcode { get; set; }

        [JsonProperty("income_deprivation_affecting_children_index_quintile")]
        public string IncomeDeprivationAffectingChildrenIndexQuintile { get; set; }

        [JsonProperty("total_number_of_pupils")]
        public string TotalNumberOfPupils { get; set; }

        [JsonProperty("latest_section_8_inspection_number_since_last_full_inspection")]
        public string LatestSection8InspectionNumberSinceLastFullInspection { get; set; }

        [JsonProperty("section_8_inspection_related_to_curren_school_urn")]
        public string Section8InspectionRelatedToCurrenSchoolUrn { get; set; }

        [JsonProperty("urn_at_time_of_section_8_inspection")]
        public string UrnAtTimeOfSection8Inspection { get; set; }

        [JsonProperty("LAESTAB_at_time_of_section_8_inspection")]
        public string LaestabAtTimeOfSection8Inspection { get; set; }

        [JsonProperty("school_name_at_time_of_section_8_inspection")]
        public string SchoolNameAtTimeOfSection8Inspection { get; set; }

        [JsonProperty("school_type_at_time_of_section_8_inspection")]
        public string SchoolTypeAtTimeOfSection8Inspection { get; set; }

        [JsonProperty("number_of_section_8_inspections_since_last_full_inspection")]
        public string NumberOfSection8InspectionsSinceLastFullInspection { get; set; }

        [JsonProperty("number_of_other_section_8_inspections_since_last_full_inspection")]
        public string NumberOfOtherSection8InspectionsSinceLastFullInspection { get; set; }

        [JsonProperty("date_of_latest_section_8_inspection")]
        public string DateOfLatestSection8Inspection { get; set; }

        [JsonProperty("section_8_inspection_publication_date")]
        public string Section8InspectionPublicationDate { get; set; }

        [JsonProperty("latest_section_8_inspection_converted_to_full_inspection")]
        public string LatestSection8InspectionConvertedToFullInspection { get; set; }

        [JsonProperty("section_8_inspection_overall_outcome")]
        public string Section8InspectionOverallOutcome { get; set; }

        [JsonProperty("inspection_number_of_latest_full_inspection")]
        public string InspectionNumberOfLatestFullInspection { get; set; }

        [JsonProperty("inspection_type")] public string InspectionType { get; set; }

        [JsonProperty("inspection_type_grouping")]
        public string InspectionTypeGrouping { get; set; }

        [JsonProperty("event_type_grouping")] public string EventTypeGrouping { get; set; }

        [JsonProperty("inspection_start_date")]
        public string InspectionStartDate { get; set; }

        [JsonProperty("inspection_end_date")] public string InspectionEndDate { get; set; }
        [JsonProperty("publication_date")] public string PublicationDate { get; set; }

        [JsonProperty("latest_full_inspection_relates_to_current_school_urn")]
        public string LatestFullInspectionRelatesToCurrentSchoolUrn { get; set; }

        [JsonProperty("school_urn_at_time_of_last_full_inspection")]
        public string SchoolUrnAtTimeOfLastFullInspection { get; set; }

        [JsonProperty("LAESTAB_at_time_of_last_full_inspection")]
        public string LaestabAtTimeOfLastFullInspection { get; set; }

        [JsonProperty("school_name_at_time_of_last_full_inspection")]
        public string SchoolNameAtTimeOfLastFullInspection { get; set; }

        [JsonProperty("school_type_at_time_of_last_full_inspection")]
        public string SchoolTypeAtTimeOfLastFullInspection { get; set; }

        [JsonProperty("overall_effectiveness")]
        public string OverallEffectiveness { get; set; }

        [JsonProperty("category_of_concern")] public string CategoryOfConcern { get; set; }
        [JsonProperty("quality_of_education")] public string QualityOfEducation { get; set; }

        [JsonProperty("behaviour_and_attitudes")]
        public string BehaviourAndAttitudes { get; set; }

        [JsonProperty("personal_development")] public string PersonalDevelopment { get; set; }

        [JsonProperty("effectiveness_of_leadership_and_management")]
        public string EffectivenessOfLeadershipAndManagement { get; set; }

        [JsonProperty("safeguarding_is_effective")]
        public string SafeguardingIsEffective { get; set; }

        [JsonProperty("early_years_provision")]
        public string EarlyYearsProvision { get; set; }

        [JsonProperty("sixth_form_provision")] public string SixthFormProvision { get; set; }

        [JsonProperty("previous_full_inspection_number")]
        public string PreviousFullInspectionNumber { get; set; }

        [JsonProperty("previous_inspection_start_date")]
        public string PreviousInspectionStartDate { get; set; }

        [JsonProperty("previous_inspection_end_date")]
        public string PreviousInspectionEndDate { get; set; }

        [JsonProperty("previous_publication_date")]
        public string PreviousPublicationDate { get; set; }

        [JsonProperty("previous_full_inspection_relates_to_urn_of_current_school")]
        public string PreviousFullInspectionRelatesToUrnOfCurrentSchool { get; set; }

        [JsonProperty("urn_at_the_time_of_previous_full_inspection")]
        public string UrnAtTheTimeOfPreviousFullInspection { get; set; }

        [JsonProperty("LAESTAB_at_the_time_of_previous_full_inspection")]
        public string LaestabAtTheTimeOfPreviousFullInspection { get; set; }

        [JsonProperty("school_name_at_the_time_of_previous_full_inspection")]
        public string SchoolNameAtTheTimeOfPreviousFullInspection { get; set; }

        [JsonProperty("school_type_at_the_time_of_previous_full_inspection")]
        public string SchoolTypeAtTheTimeOfPreviousFullInspection { get; set; }

        [JsonProperty("previous_full_inspection_overall_effectiveness")]
        public string PreviousFullInspectionOverallEffectiveness { get; set; }

        [JsonProperty("previous_category_of_concern")]
        public string PreviousCategoryOfConcern { get; set; }

        [JsonProperty("previous_quality_of_education")]
        public string PreviousQualityOfEducation { get; set; }

        [JsonProperty("previous_behaviour_and_attitudes")]
        public string PreviousBehaviourAndAttitudes { get; set; }

        [JsonProperty("previous_personal_development")]
        public string PreviousPersonalDevelopment { get; set; }

        [JsonProperty("previous_effectiveness_of_leadership_and_management")]
        public string PreviousEffectivenessOfLeadershipAndManagement { get; set; }

        [JsonProperty("previous_is_safeguarding_effective")]
        public string PreviousIsSafeguardingEffective { get; set; }

        [JsonProperty("previous_early_years_provision")]
        public string PreviousEarlyYearsProvision { get; set; }

        [JsonProperty("previous_sixth_form_provision")]
        public string PreviousSixthFormProvision { get; set; }
    }
}