using System.Collections.Generic;
using Data.TRAMS.Models.AcademyTransferProject;

namespace Data.TRAMS.Models
{
    public class TramsProjectUpdate
    {
        public AcademyTransferProjectBenefits Benefits { get; set; }
        public AcademyTransferProjectDates Dates { get; set; }
        public AcademyTransferProjectFeatures Features { get; set; }
        public string OutgoingTrustUkprn { get; set; }
        public string ProjectUrn { get; set; }
        public AcademyTransferProjectRationale Rationale { get; set; }
        public AcademyTransferProjectAcademyAndTrustInformation GeneralInformation { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public List<TransferringAcademyUpdate> TransferringAcademies { get; set; }
    }
}