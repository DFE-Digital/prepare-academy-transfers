using System.Collections.Generic;

namespace Data.Models
{
    public class Trust
    {
        public string Name { get; set; }
        public string Ukprn { get; set; }
        
        public string LeadRscRegion { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string EstablishmentType { get; set; }
        public string GiasGroupId { get; set; }
        public List<string> Address { get; set; }
        
        public List<Academy> Academies { get; set; }
    }
}