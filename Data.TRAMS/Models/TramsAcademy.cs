using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class TramsAcademy
    {
        [JsonProperty("urn")] public string Urn { get; set; }
        [JsonProperty("local_authority_code")] public string LocalAuthorityCode { get; set; }
        [JsonProperty("local_authority_name")] public string LocalAuthorityName { get; set; }
        [JsonProperty("establishment_number")] public string EstablishmentNumber { get; set; }
        [JsonProperty("establishment_name")] public string EstablishmentName { get; set; }
        [JsonProperty("establishment_type")] public NameAndCode EstablishmentType { get; set; }

        [JsonProperty("establishment_type_group_code")]
        public string EstablishmentTypeGroupCode { get; set; }

        [JsonProperty("establishment_status")] public NameAndCode EstablishmentStatus { get; set; }

        [JsonProperty("reason_establishment_opened")]
        public NameAndCode ReasonEstablishmentOpened { get; set; }

        [JsonProperty("open_date")] public string OpenDate { get; set; }

        [JsonProperty("reason_establishment_closed")]
        public NameAndCode ReasonEstablishmentClosed { get; set; }

        [JsonProperty("close_date")] public string CloseDate { get; set; }
        [JsonProperty("phase_of_education")] public NameAndCode PhaseOfEducation { get; set; }
        [JsonProperty("statutory_low_age")] public string StatutoryLowAge { get; set; }
        [JsonProperty("statutory_high_age")] public string StatutoryHighAge { get; set; }
        [JsonProperty("boarders")] public NameAndCode Boarders { get; set; }
        [JsonProperty("nursery_provision")] public string NurseryProvision { get; set; }
        [JsonProperty("official_sixth_form")] public NameAndCode OfficialSixthForm { get; set; }
        [JsonProperty("gender")] public NameAndCode Gender { get; set; }
        [JsonProperty("religious_character")] public NameAndCode ReligiousCharacter { get; set; }
        [JsonProperty("religious_ethos")] public string ReligiousEthos { get; set; }
        [JsonProperty("diocese")] public NameAndCode Diocese { get; set; }
        [JsonProperty("admissions_policy")] public NameAndCode AdmissionsPolicy { get; set; }
        [JsonProperty("school_capacity")] public string SchoolCapacity { get; set; }
        [JsonProperty("special_classes")] public NameAndCode SpecialClasses { get; set; }
        [JsonProperty("census")] public Census Census { get; set; }
        [JsonProperty("trust_school_flag")] public NameAndCode TrustSchoolFlag { get; set; }
        [JsonProperty("trusts")] public NameAndCode Trusts { get; set; }
        [JsonProperty("school_sponsor_flag")] public string SchoolSponsorFlag { get; set; }
        [JsonProperty("school_sponsors")] public string SchoolSponsors { get; set; }
        [JsonProperty("federation_flag")] public string FederationFlag { get; set; }
        [JsonProperty("federations")] public NameAndCode Federations { get; set; }
        [JsonProperty("ukprn")] public string Ukprn { get; set; }
        [JsonProperty("fehei_identifier")] public string FeheiIdentifier { get; set; }

        [JsonProperty("further_education_type")]
        public string FurtherEducationType { get; set; }

        [JsonProperty("ofsted_last_inspection")]
        public string OfstedLastInspection { get; set; }

        [JsonProperty("ofsted_special_measures")]
        public NameAndCode OfstedSpecialMeasures { get; set; }

        [JsonProperty("last_changed_date")] public string LastChangedDate { get; set; }
        [JsonProperty("address")] public Address Address { get; set; }
        [JsonProperty("school_website")] public string SchoolWebsite { get; set; }
        [JsonProperty("telephone_number")] public string TelephoneNumber { get; set; }
        [JsonProperty("headteacher_title")] public string HeadteacherTitle { get; set; }

        [JsonProperty("headteacher_first_name")]
        public string HeadteacherFirstName { get; set; }

        [JsonProperty("headteacher_last_name")]
        public string HeadteacherLastName { get; set; }

        [JsonProperty("headteacher_preferred_job_title")]
        public string HeadteacherPreferredJobTitle { get; set; }

        [JsonProperty("inspectorate")] public string Inspectorate { get; set; }
        [JsonProperty("inspectorate_name")] public string InspectorateName { get; set; }
        [JsonProperty("inspectorate_report")] public string InspectorateReport { get; set; }

        [JsonProperty("date_of_last_inspection_visit")]
        public string DateOfLastInspectionVisit { get; set; }

        [JsonProperty("date_of_next_inspection_visit")]
        public string DateOfNextInspectionVisit { get; set; }

        [JsonProperty("teen_moth")] public string TeenMoth { get; set; }
        [JsonProperty("teen_moth_places")] public string TeenMothPlaces { get; set; }
        [JsonProperty("CCF")] public string Ccf { get; set; }
        [JsonProperty("SENPRU")] public string Senpru { get; set; }
        [JsonProperty("EBD")] public string Ebd { get; set; }
        [JsonProperty("places_PRU")] public string PlacesPru { get; set; }
        [JsonProperty("FTProv")] public string FtProv { get; set; }
        [JsonProperty("ed_by_other")] public string EdByOther { get; set; }
        [JsonProperty("section_14_approved")] public string Section14Approved { get; set; }
        [JsonProperty("SEN1")] public string Sen1 { get; set; }
        [JsonProperty("SEN2")] public string Sen2 { get; set; }
        [JsonProperty("SEN3")] public string Sen3 { get; set; }
        [JsonProperty("SEN4")] public string Sen4 { get; set; }
        [JsonProperty("SEN5")] public string Sen5 { get; set; }
        [JsonProperty("SEN6")] public string Sen6 { get; set; }
        [JsonProperty("SEN7")] public string Sen7 { get; set; }
        [JsonProperty("SEN8")] public string Sen8 { get; set; }
        [JsonProperty("SEN9")] public string Sen9 { get; set; }
        [JsonProperty("SEN10")] public string Sen10 { get; set; }
        [JsonProperty("SEN11")] public string Sen11 { get; set; }
        [JsonProperty("SEN12")] public string Sen12 { get; set; }
        [JsonProperty("SEN13")] public string Sen13 { get; set; }

        [JsonProperty("type_of_resourced_provision")]
        public string TypeOfResourcedProvision { get; set; }

        [JsonProperty("resourced_provision_on_roll")]
        public string ResourcedProvisionOnRoll { get; set; }

        [JsonProperty("resourced_provision_on_capacity")]
        public string ResourcedProvisionOnCapacity { get; set; }

        [JsonProperty("sen_unit_on_roll")] public string SenUnitOnRoll { get; set; }
        [JsonProperty("sen_unit_capacity")] public string SenUnitCapacity { get; set; }
        [JsonProperty("GOR")] public NameAndCode Gor { get; set; }

        [JsonProperty("district_administrative")]
        public NameAndCode DistrictAdministrative { get; set; }

        [JsonProperty("administrative_ward")] public NameAndCode AdministrativeWard { get; set; }

        [JsonProperty("parliamentary_constituency")]
        public NameAndCode ParliamentaryConstituency { get; set; }

        [JsonProperty("urban_rural")] public NameAndCode UrbanRural { get; set; }
        [JsonProperty("GSSLA_code")] public string GsslaCode { get; set; }
        [JsonProperty("easting")] public string Easting { get; set; }
        [JsonProperty("northing")] public string Northing { get; set; }

        [JsonProperty("census_area_statistic_ward")]
        public string CensusAreaStatisticWard { get; set; }

        [JsonProperty("MSOA")] public NameAndCode Msoa { get; set; }
        [JsonProperty("LSOA")] public NameAndCode Lsoa { get; set; }
        [JsonProperty("SENStat")] public string SenStat { get; set; }
        [JsonProperty("SENNoStat")] public string SenNoStat { get; set; }

        [JsonProperty("boarding_establishment")]
        public string BoardingEstablishment { get; set; }

        [JsonProperty("props_name")] public string PropsName { get; set; }

        [JsonProperty("previous_local_authority")]
        public NameAndCode PreviousLocalAuthority { get; set; }

        [JsonProperty("previous_establishment_number")]
        public string PreviousEstablishmentNumber { get; set; }

        [JsonProperty("ofsted_rating")] public string OfstedRating { get; set; }
        [JsonProperty("RSC_region")] public string RscRegion { get; set; }
        [JsonProperty("country")] public string Country { get; set; }
        [JsonProperty("UPRN")] public string Uprn { get; set; }
        [JsonProperty("MIS_establishment")] public MisEstablishment MisEstablishment { get; set; }

        [JsonProperty("MIS_further_education_establishment")]
        public Misfea MisFurtherEducationEstablishment { get; set; }

        [JsonProperty("SMART_data")] public SmartData SmartData { get; set; }
        [JsonProperty("financial")] public Placeholder Financial { get; set; }
        [JsonProperty("concerns")] public Placeholder Concerns { get; set; }
    }
}