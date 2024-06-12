namespace Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject
{
    public class TransferringAcademy
    {
        public TrustSummary IncomingTrust { get; set; }
        public AcademySummary OutgoingAcademy { get; set; }
        public string OutgoingAcademyUkprn { get; set; }
        public string IncomingTrustUkprn { get; set; }
        public string IncomingTrustName { get; set; }
        public string Region { get; set; }
        public string LocalAuthority { get; set; }
        public string PupilNumbersAdditionalInformation { get; set; }
        public string LatestOfstedReportAdditionalInformation { get; set; }
        public string KeyStage2PerformanceAdditionalInformation { get; set; }
        public string KeyStage4PerformanceAdditionalInformation { get; set; }
        public string KeyStage5PerformanceAdditionalInformation { get; set; }
        public string PFIScheme { get; set; }
        public string PFISchemeDetails { get; set; }
        public string ViabilityIssues { get; set; }
        public string FinancialDeficit { get; set; }
        public string MPNameAndParty { get; set; }
        public string DistanceFromAcademyToTrustHq { get; set; }
        public string DistanceFromAcademyToTrustHqDetails { get; set; }
        public string PublishedAdmissionNumber { get; set; }

    }
}