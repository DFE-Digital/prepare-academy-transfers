using System.Collections.Generic;
using System.Linq;
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
        private readonly IMapper<TramsTrust, Trust> _trustMapper;

        public TramsTrustsRepository(ITramsHttpClient httpClient,
            IMapper<TramsTrustSearchResult, TrustSearchResult> searchResultMapper,
            IMapper<TramsTrust, Trust> trustMapper)
        {
            _httpClient = httpClient;
            _searchResultMapper = searchResultMapper;
            _trustMapper = trustMapper;
        }

        #region API Interim

        public async Task<RepositoryResult<List<TrustSearchResult>>> SearchTrusts(string searchQuery = "")
        {
            var url = $"trust/{searchQuery}";
            using var response = await _httpClient.GetAsync(url);
            Trust trust;

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TramsTrust>(apiResponse);
                trust = _trustMapper.Map(result);
            }
            else
            {
                return new RepositoryResult<List<TrustSearchResult>>()
                {
                    Result = new List<TrustSearchResult>()
                };
            }

            return new RepositoryResult<List<TrustSearchResult>>
            {
                Result = new List<TrustSearchResult>
                {
                    new TrustSearchResult
                    {
                        Ukprn = trust.Ukprn,
                        TrustName = trust.Name,
                        CompaniesHouseNumber = trust.CompaniesHouseNumber,
                        Academies = trust.Academies
                            .Select(
                                academy =>
                                    new TrustSearchAcademy
                                    {
                                        Name = academy.Name,
                                        Ukprn = academy.Ukprn,
                                        Urn = academy.Urn
                                    }
                            )
                            .ToList()
                    }
                }
            };
        }

        public async Task<RepositoryResult<List<TrustSearchResult>>> SearchTrustsOriginal(string searchQuery = "")
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

        #endregion

        public async Task<RepositoryResult<Trust>> GetByUkprn(string ukprn)
        {
            var url = $"trust/{ukprn}";
            using var response = await _httpClient.GetAsync(url);
            Trust trust;
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TramsTrust>(apiResponse);
                trust = _trustMapper.Map(result);
            }
            else
            {
                trust = new Trust();
            }

            return new RepositoryResult<Trust>
            {
                Result = trust
            };
        }
    }
}