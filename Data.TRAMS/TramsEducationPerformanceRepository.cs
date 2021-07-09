using System.Net;
using System.Threading.Tasks;
using Data.Models.KeyStagePerformance;
using Data.TRAMS.Models;
using Data.TRAMS.Models.EducationPerformance;
using Newtonsoft.Json;

namespace Data.TRAMS
{
    public class TramsEducationPerformanceRepository : IEducationPerformance
    {
        private readonly ITramsHttpClient _httpClient;
        private readonly IMapper<TramsEducationPerformance, EducationPerformance> _educationPerformanceMapper;

        public TramsEducationPerformanceRepository(ITramsHttpClient httpClient, IMapper<TramsEducationPerformance, EducationPerformance> educationPerformanceMapper)
        {
            _httpClient = httpClient;
            _educationPerformanceMapper = educationPerformanceMapper;
        }
        
        public async Task<RepositoryResult<EducationPerformance>> GetByAcademyUrn(string urn)
        {
            using var response = await _httpClient.GetAsync($"educationPerformance/{urn}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TramsEducationPerformance>(apiResponse);
                return new RepositoryResult<EducationPerformance>
                {
                    Result = _educationPerformanceMapper.Map(result)
                };
            }
            
            var errorMessage = response.StatusCode == HttpStatusCode.NotFound
                ? "Academy not found"
                : "API encountered an error";

            return new RepositoryResult<EducationPerformance>
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = response.StatusCode,
                    ErrorMessage = errorMessage
                }
            };
        }
    }
}