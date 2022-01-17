﻿using System.Collections.Generic;
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

        public async Task<RepositoryResult<List<TrustSearchResult>>> SearchTrusts(string searchQuery = "",
            string outgoingTrustId = "")
        {
            var url = $"trusts?groupName={searchQuery}&ukprn={searchQuery}&companiesHouseNumber={searchQuery}";
            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new TramsApiException(response);
            }

            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<TramsTrustSearchResult>>(apiResponse);

            var mappedResult = result.Where(t => !string.IsNullOrEmpty(t.Ukprn) &&
                                                 t.Ukprn != outgoingTrustId)
                .Select(r => _searchResultMapper.Map(r)).ToList();

            return new RepositoryResult<List<TrustSearchResult>>
            {
                Result = mappedResult
            };
        }

        public async Task<RepositoryResult<Trust>> GetByUkprn(string ukprn)
        {
            var url = $"trust/{ukprn}";
            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new TramsApiException(response);
            }

            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TramsTrust>(apiResponse);
            var trust = _trustMapper.Map(result);

            return new RepositoryResult<Trust>
            {
                Result = trust
            };
        }
    }
}