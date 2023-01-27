using System.Collections.Generic;
using Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Helpers;

namespace Data.Models
{
    public class ProjectSearchResult
    {
        public string Reference { get; set; }
        public string OutgoingTrustUkprn { get; set; }
        public string OutgoingTrustName { get; set; }
        public string OutgoingTrustNameInTitleCase => OutgoingTrustName.ToTitleCase();
        public List<TransferringAcademies> TransferringAcademies { get; set; }
        public User AssignedUser { get; set; }
        public string Urn { get; set; }
    }
}