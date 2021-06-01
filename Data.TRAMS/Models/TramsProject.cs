namespace Data.TRAMS.Models
{
    public class TramsProject
    {
        public AcademyTransferProjectFeatures Features { get; set; }
        public string OutgoingTrustUrn { get; set; }
        public string ProjectUrn { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public TransferringAcademy TransferringAcademies { get; set; }
    }
}