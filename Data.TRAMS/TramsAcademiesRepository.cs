using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Data.Models;
using Data.TRAMS.Models;
using Newtonsoft.Json;

namespace Data.TRAMS
{
    public class TramsAcademiesRepository : IAcademies
    {
        private readonly HttpClient _httpClient;

        public TramsAcademiesRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RepositoryResult<Academy>> GetAcademyByUkprn(string ukprn)
        {
            using var response = await _httpClient.GetAsync($"academy/{ukprn}");

            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TramsAcademy>(apiResponse);

            return new RepositoryResult<Academy>
            {
                Result = new Academy
                {
                    Ukprn = result.Ukprn,
                    Name = result.EstablishmentName,
                    Performance = new AcademyPerformance
                    {
                        AgeRange = $"{result.StatutoryLowAge} to {result.StatutoryHighAge}",
                        Capacity = result.SchoolCapacity
                    }
                }
            };
        }
    }
}