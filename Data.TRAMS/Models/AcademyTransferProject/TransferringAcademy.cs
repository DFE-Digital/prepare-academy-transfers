namespace Data.TRAMS.Models.AcademyTransferProject
{
    public class TransferringAcademy
    {
        public TrustSummary IncomingTrust { get; set; }
        public AcademySummary OutgoingAcademy { get; set; }
        public string OutgoingAcademyUkprn { get; set; }
        public string IncomingTrustUkprn { get; set; }
        public string IncomingTrustName { get; set; }
    }
}