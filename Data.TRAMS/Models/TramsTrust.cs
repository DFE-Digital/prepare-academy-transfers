using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class TramsTrust
    {
        [JsonProperty("ifd_data")] public TramsTrustIfdData IfdData { get; set; }
        [JsonProperty("gias_data")] public TramsTrustGiasData GiasData { get; set; }
        [JsonProperty("academies")] public List<TramsAcademy> Academies { get; set; }
    }


    public class TramsTrustIfdData
    {
        [JsonProperty("trust_open_date")] public string TrustOpenDate { get; set; }
        [JsonProperty("lead_RSC_region")] public string LeadRscRegion { get; set; }

        [JsonProperty("trust_contact_phone_number")]
        public string TrustContactPhoneNumber { get; set; }

        [JsonProperty("performance_and_risk_date_of_meeting")]
        public string PerformanceAndRiskDateOfMeeting { get; set; }

        [JsonProperty("prioritised_area_of_review")]
        public string PrioritisedAreaOfReview { get; set; }

        [JsonProperty("current_single_list_grouping")]
        public string CurrentSingleListGrouping { get; set; }

        [JsonProperty("date_of_grouping_decision")]
        public string DateOfGroupingDecision { get; set; }

        [JsonProperty("date_entered_onto_single_list")]
        public string DateEnteredOntoSingleList { get; set; }

        [JsonProperty("trust_review_writeup")] public string TrustReviewWriteup { get; set; }

        [JsonProperty("date_of_trust_review_meeting")]
        public string DateOfTrustReviewMeeting { get; set; }

        [JsonProperty("followup_letter_sent")] public string FollowupLetterSent { get; set; }

        [JsonProperty("date_action_planned_for")]
        public string DateActionPlannedFor { get; set; }

        [JsonProperty("WIP_summary_goes_to_minister")]
        public string WipSummaryGoesToMinister { get; set; }

        [JsonProperty("external_governance_review_date")]
        public string ExternalGovernanceReviewDate { get; set; }

        [JsonProperty("efficiency_ICF_preview_completed")]
        public string EfficiencyIcfPreviewCompleted { get; set; }

        [JsonProperty("efficiency_ICF_preview_other")]
        public string EfficiencyIcfPreviewOther { get; set; }

        [JsonProperty("link_to_workplace_for_efficiency_ICF_review")]
        public string LinkToWorkplaceForEfficiencyIcfReview { get; set; }

        [JsonProperty("number_in_trust")] public string NumberInTrust { get; set; }
    }

    public class TramsTrustGiasData
    {
        [JsonProperty("group_id")] public string GroupId { get; set; }
        [JsonProperty("group_name")] public string GroupName { get; set; }

        [JsonProperty("companies_house_number")]
        public string CompaniesHouseNumber { get; set; }

        [JsonProperty("group_contact_address")]
        public GroupContactAddress GroupContactAddress { get; set; }

        [JsonProperty("ukprn")] public string Ukprn { get; set; }
    }

    public class GroupContactAddress
    {
        [JsonProperty("street")] public string Street { get; set; }
        [JsonProperty("locality")] public string Locality { get; set; }
        [JsonProperty("additional_line")] public string AdditionalLine { get; set; }
        [JsonProperty("town")] public string Town { get; set; }
        [JsonProperty("county")] public string County { get; set; }
        [JsonProperty("postcode")] public string Postcode { get; set; }
    }
}