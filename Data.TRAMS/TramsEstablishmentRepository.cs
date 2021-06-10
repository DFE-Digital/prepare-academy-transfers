using System.Threading.Tasks;
using Data.Models;
using Data.TRAMS.Models;
using Newtonsoft.Json;

namespace Data.TRAMS
{
    public class TramsEstablishmentRepository : IAcademies
    {
        private readonly ITramsHttpClient _httpClient;
        private readonly IMapper<TramsEstablishment, Academy> _academyMapper;

        public TramsEstablishmentRepository(ITramsHttpClient httpClient,
            IMapper<TramsEstablishment, Academy> academyMapper)
        {
            _httpClient = httpClient;
            _academyMapper = academyMapper;
        }

        public async Task<RepositoryResult<Academy>> GetAcademyByUkprn(string ukprn)
        {
            using var response = await _httpClient.GetAsync($"establishment/{ukprn}");

            Academy mappedResult;
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TramsEstablishment>(apiResponse);
                mappedResult = _academyMapper.Map(result);
            }
            else
            {
                mappedResult = new Academy();
            }

            return new RepositoryResult<Academy> {Result = mappedResult};
        }
    }
}