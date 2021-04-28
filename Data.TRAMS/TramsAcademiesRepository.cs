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
        private readonly ITramsHttpClient _httpClient;
        private readonly IMapper<TramsAcademy, Academy> _academyMapper;

        public TramsAcademiesRepository(ITramsHttpClient httpClient, IMapper<TramsAcademy, Academy> academyMapper)
        {
            _httpClient = httpClient;
            _academyMapper = academyMapper;
        }

        public async Task<RepositoryResult<Academy>> GetAcademyByUkprn(string ukprn)
        {
            using var response = await _httpClient.GetAsync($"academy/{ukprn}");

            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TramsAcademy>(apiResponse);
            var mappedResult = _academyMapper.Map(result);

            return new RepositoryResult<Academy> {Result = mappedResult};
        }
    }
}