using System.Collections.Generic;
using Data.TRAMS.Models.AcademyTransferProject;

namespace Data.TRAMS.Models
{
    public class TramsProject
    {
        public TramsProject()
        {
            Benefits = new AcademyTransferProjectBenefits();
            Dates = new AcademyTransferProjectDates();
            Features = new AcademyTransferProjectFeatures();
            Rationale = new AcademyTransferProjectRationale();
            GeneralInformation = new AcademyTransferProjectAcademyAndTrustInformation();
            TransferringAcademies = new List<TransferringAcademy>();
            OutgoingTrust = new TrustSummary();
        }
        public AcademyTransferProjectBenefits Benefits { get; set; }
        public AcademyTransferProjectDates Dates { get; set; }
        public AcademyTransferProjectFeatures Features { get; set; }
        public string OutgoingTrustUkprn { get; set; }
        public string ProjectReference { get; set; }
        public string ProjectUrn { get; set; }
        public AcademyTransferProjectRationale Rationale { get; set; }
        public AcademyTransferProjectAcademyAndTrustInformation GeneralInformation { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public List<TransferringAcademy> TransferringAcademies { get; set; }
        public TrustSummary OutgoingTrust { get; set; }
        
        //todo: remove additional information from project level
        public string AcademyPerformanceAdditionalInformation { get; set; }
        public string PupilNumbersAdditionalInformation { get; set; }
        public string LatestOfstedJudgementAdditionalInformation { get; set; }
        public string KeyStage2PerformanceAdditionalInformation { get; set; }
        public string KeyStage4PerformanceAdditionalInformation { get; set; }
        public string KeyStage5PerformanceAdditionalInformation { get; set; }
    }
}