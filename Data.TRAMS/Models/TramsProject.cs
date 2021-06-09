using System.Collections.Generic;
using Data.TRAMS.Models.AcademyTransferProject;

namespace Data.TRAMS.Models
{
    public class TramsProject
    {
        public AcademyTransferProjectBenefits Benefits { get; set; }
        public AcademyTransferProjectDates Dates { get; set; }
        public AcademyTransferProjectFeatures Features { get; set; }
        public string OutgoingTrustUkprn { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectUrn { get; set; }
        public AcademyTransferProjectRationale Rationale { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public List<TransferringAcademy> TransferringAcademies { get; set; }
        public TrustSummary OutgoingTrust { get; set; }
    }
}