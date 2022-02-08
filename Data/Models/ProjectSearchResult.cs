using System.Collections.Generic;
using Data.Models.Projects;

namespace Data.Models
{
    public class ProjectSearchResult
    {
        public string Name { get; set; }
        public string Reference { get; set; }
        public string OutgoingTrustUkprn { get; set; }
        public string OutgoingTrustName { get; set; }
        public List<TransferringAcademies> TransferringAcademies { get; set; }
        public string Urn { get; set; }
    }
}