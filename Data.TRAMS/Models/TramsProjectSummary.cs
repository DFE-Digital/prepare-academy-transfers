using System.Collections.Generic;
using Data.TRAMS.Models.AcademyTransferProject;

namespace Data.TRAMS.Models
{
    public class TramsProjectSummary
    {
        public string ProjectUrn { get; set; }
        public string ProjectReference { get; set; }
        public string OutgoingTrustUkprn { get; set; }
        public string OutgoingTrustName { get; set; }
        public List<TransferringAcademy> TransferringAcademies { get; set; }
    }
}