using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dfe.PrepareTransfers.Data.TRAMS.Models
{
    public class TramsTrustSearchResult
    {
        public List<TramsTrustSearchEstablishment> Establishments { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string GroupName { get; set; }
        public string Ukprn { get; set; }
    }
}