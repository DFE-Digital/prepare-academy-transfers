using System.Collections.Generic;

namespace Data.Models
{
    public class TrustSearchResult
    {
        public string Ukprn { get; set; }
        public string TrustName { get; set; }


        public string CompaniesHouseNumber { get; set; }

        public List<TrustSearchAcademies> Academies { get; set; }
    }

    public class TrustSearchAcademies
    {
        public string Ukprn { get; set; }
        public string Name { get; set; }
    }
}