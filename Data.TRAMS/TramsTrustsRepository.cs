using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Data.Models;
using Data.TRAMS.Models;
using Newtonsoft.Json;

namespace Data.TRAMS
{
    public class TramsTrustsRepository : ITrusts
    {
        private readonly ITramsHttpClient _httpClient;
        private readonly IMapper<TramsTrustSearchResult, TrustSearchResult> _searchResultMapper;

        public TramsTrustsRepository(ITramsHttpClient httpClient,
            IMapper<TramsTrustSearchResult, TrustSearchResult> searchResultMapper)
        {
            _httpClient = httpClient;
            _searchResultMapper = searchResultMapper;
        }

        public async Task<RepositoryResult<List<TrustSearchResult>>> SearchTrusts(string searchQuery = "")
        {
            var url = $"trusts?group_name={searchQuery}&urn={searchQuery}&companies_house_number={searchQuery}";
            using var response = await _httpClient.GetAsync(url);

            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<TramsTrustSearchResult>>(apiResponse);

            var mappedResult = result.Select(r => _searchResultMapper.Map(r)).ToList();

            return new RepositoryResult<List<TrustSearchResult>>
            {
                Result = mappedResult
            };
        }
    }
}