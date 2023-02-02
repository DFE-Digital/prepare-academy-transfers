using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Data.Models
{
    public class TrustSearchResult
    {
        public string Ukprn { get; set; }
        public string TrustName { get; set; }


        public string CompaniesHouseNumber { get; set; }

        public List<TrustSearchAcademy> Academies { get; set; }
    }

    public class TrustSearchAcademy
    {
        public string Urn { get; set; }
        public string Ukprn { get; set; }
        public string Name { get; set; }
    }
}