using System.Collections.Generic;
using Dfe.Academisation.ExtensionMethods;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;

namespace Dfe.PrepareTransfers.Data.Models
{
    public class ProjectSearchResult
    {
        public string Reference { get; set; }
        public string OutgoingTrustUkprn { get; set; }
        public string OutgoingTrustName { get; set; }
        public string OutgoingTrustNameInTitleCase => OutgoingTrustName.ToTitleCase();
        public string Status { get; set; }
        public List<TransferringAcademies> TransferringAcademies { get; set; }
        public User AssignedUser { get; set; }
        public string Urn { get; set; }
    }
}