using System;
using System.Net;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.Data.TRAMS.Models.EducationPerformance;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Dfe.PrepareTransfers.Data.TRAMS
{
    public class TramsEducationPerformanceRepository : IEducationPerformance
    {
        private readonly ITramsHttpClient _httpClient;
        private readonly IMapper<TramsEducationPerformance, EducationPerformance> _educationPerformanceMapper;
        private readonly IDistributedCache _distributedCache;

        public TramsEducationPerformanceRepository(ITramsHttpClient httpClient, IMapper<TramsEducationPerformance, EducationPerformance> educationPerformanceMapper,  IDistributedCache distributedCache)
        {
            _httpClient = httpClient;
            _educationPerformanceMapper = educationPerformanceMapper;
            _distributedCache = distributedCache;
        }
        
        public async Task<RepositoryResult<EducationPerformance>> GetByAcademyUrn(string urn)
        {
            var cacheKey = $"GetPerformanceByAcademy_{urn}";
            var cachedString = await _distributedCache.GetStringAsync(cacheKey);
            //Check for information in cache
            if (!string.IsNullOrWhiteSpace(cachedString))
            {
                return JsonConvert.DeserializeObject<RepositoryResult<EducationPerformance>>(cachedString);
            }
            
            using var response = await _httpClient.GetAsync($"educationPerformance/{urn}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TramsEducationPerformance>(apiResponse);
                var mappedResult = new RepositoryResult<EducationPerformance>
                {
                    Result = _educationPerformanceMapper.Map(result)
                };
                
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)
                };
                await _distributedCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(mappedResult), cacheOptions);
                return mappedResult;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new RepositoryResult<EducationPerformance>()
                {
                    Result = new EducationPerformance()
                };
            }

            throw new TramsApiException(response);
        }
    }
}