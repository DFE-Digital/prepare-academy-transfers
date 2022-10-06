using System;
using System.Collections.Generic;
using DocumentGeneration;

namespace Frontend.Models.ProjectTemplate
{
    public class ProjectTemplateModel
    {
        [DocumentText("TrustName")] public string TrustName { get; set; }
        [DocumentText("TrustReferenceNumber")] public string TrustReferenceNumber { get; set; }
        public string ProjectReference { get; set; }
        public string IncomingTrustName { get; set; }
        public string IncomingTrustUkprn { get; set; }
        [DocumentText("Recommendation")] public string Recommendation { get; set; }
        [DocumentText("Author")] public string Author { get; set; }
        [DocumentText("ProjectName")] public string ProjectName { get; set; }
        [DocumentText("RationaleForProject")] public string RationaleForProject { get; set; }
        [DocumentText("TrustAgreement")] public string TrustAgreement { get; set; }
        [DocumentText("DiocesanConsent")] public string DiocesanConsent { get; set; }
        [DocumentText("FoundationConsent")] public string FoundationConsent { get; set; }
        [DocumentText("RationaleForTrust")] public string RationaleForTrust { get; set; }
        [DocumentText("ClearedBy")] public string ClearedBy { get; set; }
        [DocumentText("Version")] public string Version { get; set; }

        [DocumentText("DateOfHtb")] public string DateOfHtb { get; set; }

        [DocumentText("DateOfProposedTransfer")]
        public string DateOfProposedTransfer { get; set; }

        [DocumentText("ReasonForTransfer")]
        public string ReasonForTheTransfer { get; set; }

        [DocumentText("TypeOfTransfer")] public string TypeOfTransfer { get; set; }

        [DocumentText("TransferBenefits")] public string TransferBenefits { get; set; }

        [DocumentText("EqualitiesImpactAssessmentConsidered")] public string EqualitiesImpactAssessmentConsidered { get; set; }

        public List<ProjectTemplateAcademyModel> Academies { get; set; }
        
        [DocumentText("AnyRisks")] public string AnyRisks { get; set; }
        public List<Tuple<string,string>> OtherFactors { get; set; }
    }
}