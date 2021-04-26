using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Data.Models;
using Data.TRAMS.Models;
using Newtonsoft.Json;
using TrustSearchAcademies = Data.Models.TrustSearchAcademies;

namespace Data.TRAMS
{
    public class TramsTrustsRepository : ITrusts
    {
        public async Task<List<TrustSearchResult>> SearchTrusts()
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync("http://localhost:3000/trusts");

            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<TramsTrustSearchResult>>(apiResponse);

            var mappedResult = result.Select(r => new TrustSearchResult
            {
                Ukprn = r.Urn,
                TrustName = r.GroupName,
                CompaniesHouseNumber = r.CompaniesHouseNumber,
                Academies = r.Academies.Select(a => new TrustSearchAcademies {Name = a.Name, Urn = a.Urn}).ToList()
            }).ToList();


            return mappedResult;
        }
    }
}