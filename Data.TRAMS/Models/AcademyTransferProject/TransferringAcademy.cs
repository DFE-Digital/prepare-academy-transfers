namespace Data.TRAMS.Models.AcademyTransferProject
{
    public class TransferringAcademy
    {
        public TrustSummary IncomingTrust { get; set; }
        public AcademySummary OutgoingAcademy { get; set; }
        public string OutgoingAcademyUkprn { get; set; }
        public string IncomingTrustUkprn { get; set; }
        public string IncomingTrustName { get; set; }
        public string PupilNumbersAdditionalInformation { get; set; }
        public string LatestOfstedReportAdditionalInformation { get; set; }
        public string KeyStage2PerformanceAdditionalInformation { get; set; }
        public string KeyStage4PerformanceAdditionalInformation { get; set; }
        public string KeyStage5PerformanceAdditionalInformation { get; set; }
        
    }
}