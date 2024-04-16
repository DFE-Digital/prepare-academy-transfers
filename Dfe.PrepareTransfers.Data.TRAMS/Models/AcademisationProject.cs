using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject;
using Dfe.PrepareTransfers.Web.Models;

namespace Dfe.PrepareTransfers.Data.TRAMS.Models
{
    public class AcademisationProject
    {
        public AcademisationProject()
        {
            Benefits = new AcademyTransferProjectBenefits();
            LegalRequirements = new AcademyTransferProjectLegalRequirements();
            Dates = new AcademyTransferProjectDates();
            Features = new AcademyTransferProjectFeatures();
            Rationale = new AcademyTransferProjectRationale();
            GeneralInformation = new AcademyTransferProjectAcademyAndTrustInformation();
            TransferringAcademies = new List<TransferringAcademy>();
            OutgoingTrust = new TrustSummary();
        }
        public int Id { get; set; }
        public AcademyTransferProjectBenefits Benefits { get; set; }
        public AcademyTransferProjectLegalRequirements LegalRequirements { get; set; }
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
        public User AssignedUser { get; set; }
        public bool? IsFormAMat { get; set; }

    }
}