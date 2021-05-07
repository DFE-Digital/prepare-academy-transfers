using System.Collections.Generic;
using Data.Models.Projects;

namespace Data.Models
{
    public class Project
    {
        public string Urn { get; set; }
        public string Name { get; set; }
        public string OutgoingTrustUkprn { get; set; }
        public string OutgoingTrustName { get; set; }
        
        public string State { get; set; }
        public string Status { get; set; }
        public List<TransferringAcademies> TransferringAcademies { get; set; }
        public TransferFeatures Features { get; set; }
    }
}