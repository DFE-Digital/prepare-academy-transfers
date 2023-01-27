using System.Collections.Generic;
using Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.DocumentGeneration;

namespace Dfe.PrepareTransfers.Web.Models
{
    public class HtbDocument
    {
        [DocumentText("SchoolName")] public string SchoolName { get; set; }
        [DocumentText("SchoolUrn")] public string SchoolUrn { get; set; }
        [DocumentText("TrustName")] public string TrustName { get; set; }
        [DocumentText("TrustReferenceNumber")] public string TrustReferenceNumber { get; set; }
        [DocumentText("Recommendation")] public string Recommendation { get; set; }
        [DocumentText("Author")] public string Author { get; set; }
        [DocumentText("ProjectName")] public string ProjectName { get; set; }
        [DocumentText("AcademyTypeAndRoute")] public string AcademyTypeAndRoute { get; set; }
        [DocumentText("SchoolType")] public string SchoolType { get; set; }
        [DocumentText("SchoolPhase")] public string SchoolPhase { get; set; }
        [DocumentText("AgeRange")] public string AgeRange { get; set; }
        [DocumentText("SchoolCapacity")] public string SchoolCapacity { get; set; }
        [DocumentText("DiocesanConsent")] public string DiocesanConsent { get; set; }
        [DocumentText("TrustAgreement")] public string TrustAgreement { get; set; }

        [DocumentText("PublishedAdmissionNumber")]
        public string PublishedAdmissionNumber { get; set; }

        [DocumentText("NumberOnRoll")] public string NumberOnRoll { get; set; }

        [DocumentText("PercentageFreeSchoolMeals")]
        public string PercentageFreeSchoolMeals { get; set; }

        [DocumentText("OfstedLastInspection")] public string OfstedLastInspection { get; set; }
        [DocumentText("OverallEffectiveness")] public string OverallEffectiveness { get; set; }
        [DocumentText("RationaleForProject")] public string RationaleForProject { get; set; }
        [DocumentText("RationaleForTrust")] public string RationaleForTrust { get; set; }
        [DocumentText("ClearedBy")] public string ClearedBy { get; set; }
        [DocumentText("Version")] public string Version { get; set; }
        
        [DocumentText("DateOfHtb")] public string DateOfHtb { get; set; }
        [DocumentText("DateOfProposedTransfer")] public string DateOfProposedTransfer { get; set; }
        
        [DocumentText("ViabilityIssues")] public string ViabilityIssues { get; set; }
        [DocumentText("FinancialDeficit")] public string FinancialDeficit { get; set; }
        [DocumentText("Pfi")] public string Pfi { get; set; }
        
        [DocumentText("PercentageGoodOrOutstandingInDiocesanTrust")]
        public string PercentageGoodOrOutstandingInDiocesanTrust { get; set; }
        [DocumentText("DistanceFromTheAcademyToTheTrustHeadquarters")]
        public string DistanceFromTheAcademyToTheTrustHeadquarters { get; set; }
        [DocumentText("MpAndParty")] public string MpAndParty { get; set; }
        
        [DocumentText("WhoInitiatedTheTransfer")] public string WhoInitiatedTheTransfer { get; set; }
        [DocumentText("ReasonForTransfer")] public string ReasonForTransfer { get; set; }
        [DocumentText("MoreDetailsAboutTheTransfer")] public string MoreDetailsAboutTheTransfer { get; set; }
        [DocumentText("TypeOfTransfer")] public string TypeOfTransfer { get; set; }
        
        [DocumentText("TransferBenefits")] public string TransferBenefits { get; set; }
        [DocumentText("OtherFactors")] public string OtherFactors { get; set; }
        
        [DocumentText("GirlsOnRoll")] public string GirlsOnRoll { get; set; }
        [DocumentText("BoysOnRoll")] public string BoysOnRoll { get; set; }
        [DocumentText("PupilsWithSen")] public string PupilsWithSen { get; set; }
        [DocumentText("PupilsWithFirstLanguageNotEnglish")] public string PupilsWithFirstLanguageNotEnglish { get; set; }
        [DocumentText("PupilsFsm6Years")] public string PupilsFsm6Years { get; set; }
        [DocumentText("PupilNumbersAdditionalInformation")] public string PupilNumbersAdditionalInformation { get; set; }
        [DocumentText("OfstedReport")] public string OfstedReport { get; set; }
        [DocumentText("OfstedAdditionalInformation")] public string OfstedAdditionalInformation { get; set; }
        public List<KeyStage2> KeyStage2Performance { get; set; }
        public List<KeyStage4> KeyStage4Performance { get; set; }
        public List<KeyStage5> KeyStage5Performance { get; set; }
        public string KeyStage2AdditionalInformation { get; set; }
        public string KeyStage4AdditionalInformation { get; set; }
        public string KeyStage5AdditionalInformation { get; set; }
        public string LocalAuthorityName { get; set; }
    }
}