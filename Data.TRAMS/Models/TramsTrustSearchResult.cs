using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class TramsTrustSearchResult
    {
        public List<TramsTrustSearchAcademy> Academies { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string GroupName { get; set; }
         public string Urn { get; set; }
    }
}