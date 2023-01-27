using System;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Dfe.PrepareTransfers.Data.TRAMS
{
    public class TramsEstablishmentRepository : IAcademies
    {
        private readonly ITramsHttpClient _httpClient;
        private readonly IMapper<TramsEstablishment, Academy> _academyMapper;
        private readonly IDistributedCache _distributedCache;

        public TramsEstablishmentRepository(ITramsHttpClient httpClient,
            IMapper<TramsEstablishment, Academy> academyMapper, IDistributedCache distributedCache)
        {
            _httpClient = httpClient;
            _academyMapper = academyMapper;
            _distributedCache = distributedCache;
        }

        public async Task<RepositoryResult<Academy>> GetAcademyByUkprn(string ukprn)
        {
            var cacheKey = $"GetAcademyByUkprn_{ukprn}";
            var cachedString = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(cachedString))
            {
                return JsonConvert.DeserializeObject<RepositoryResult<Academy>>(cachedString);
            }

            using var response = await _httpClient.GetAsync($"establishment/{ukprn}");

            if (!response.IsSuccessStatusCode)
            {
                throw new TramsApiException(response);
            }
            
            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TramsEstablishment>(apiResponse);
            var mappedResult = new RepositoryResult<Academy>
            {
                Result = _academyMapper.Map(result)
            };
            
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)
            };
            await _distributedCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(mappedResult), cacheOptions);
            return mappedResult;
        }
    }
}