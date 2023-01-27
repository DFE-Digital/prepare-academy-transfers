using System.Collections.Generic;
using System.ComponentModel;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;

namespace Dfe.PrepareTransfers.Web.Models.ProjectTemplate
{
    public class ProjectTemplateAcademyModel
    {
        public string SchoolName { get; set; }
        public string SchoolUrn { get; set; }
        public string AcademyTypeAndRoute { get; set; }
        public string SchoolType { get; set; }
        public string SchoolPhase { get; set; }
        public string AgeRange { get; set; }
        public string SchoolCapacity { get; set; }
        public string PublishedAdmissionNumber { get; set; }
        public string NumberOnRoll { get; set; }
        public string PercentageFreeSchoolMeals { get; set; }
        public string OverallEffectiveness { get; set; }
        public string ViabilityIssues { get; set; }
        public string FinancialDeficit { get; set; }
        public string Pfi { get; set; }
        public string PercentageGoodOrOutstandingInDiocesanTrust { get; set; }
        public string DistanceFromTheAcademyToTheTrustHeadquarters { get; set; }
        public string MpAndParty { get; set; }
        public string GirlsOnRoll { get; set; }
        public string BoysOnRoll { get; set; }
        public string PupilsWithSen { get; set; }
        public string PupilsWithFirstLanguageNotEnglish { get; set; }
        public string PupilsFsm6Years { get; set; }
        public string PupilNumbersAdditionalInformation { get; set; }
        public string OfstedReport { get; set; }
        public string OfstedAdditionalInformation { get; set; }
        public List<KeyStage2> KeyStage2Performance { get; set; }
        public List<KeyStage4> KeyStage4Performance { get; set; }
        public List<KeyStage5> KeyStage5Performance { get; set; }
        public string KeyStage2AdditionalInformation { get; set; }
        public string KeyStage4AdditionalInformation { get; set; }
        public string KeyStage5AdditionalInformation { get; set; }
        public string LocalAuthorityName { get; set; }
        public string QualityOfEducation { get; set; }
        public string BehaviourAndAttitudes { get; set; }
        public string PersonalDevelopment { get; set; }
        public string EffectivenessOfLeadershipAndManagement { get; set; }
        public string InspectionEndDate { get; set; }
        public string EarlyYearsProvision { get; set; }
        public bool EarlyYearsProvisionApplicable { get; set; }
        public string SixthFormProvision { get; set; }
        public bool SixthFormProvisionApplicable { get; set; }
        public bool LatestInspectionIsSection8 { get; set; }
        public string DateOfLatestSection8Inspection { get; set; }
    }
}