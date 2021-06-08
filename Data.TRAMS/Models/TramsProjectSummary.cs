using Data.TRAMS.Models.AcademyTransferProject;

namespace Data.TRAMS.Models
{
    public class TramsProjectSummary
    {
        public string OutgoingTrustUkprn { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectUrn { get; set; }
        public TransferringAcademy TransferringAcademies { get; set; }
    }
}