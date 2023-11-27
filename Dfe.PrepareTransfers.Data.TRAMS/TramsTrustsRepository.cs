using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfe.Academies.Contracts.V4;
using Dfe.Academies.Contracts.V4.Establishments;
using Dfe.Academies.Contracts.V4.Trusts;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Newtonsoft.Json;

namespace Dfe.PrepareTransfers.Data.TRAMS
{
    public class TramsTrustsRepository : ITrusts
    {
        private readonly ITramsHttpClient _httpClient;
        private readonly IMapper<TrustDto, Trust> _trustMapper;

        public TramsTrustsRepository(ITramsHttpClient httpClient,
            IMapper<TrustDto, Trust> trustMapper
            )
        {
            _httpClient = httpClient;
            _trustMapper = trustMapper;
        }

        public async Task<List<Trust>> SearchTrusts(string searchQuery = "",
            string outgoingTrustId = "")
        {
            var url = $"v4/trusts?groupName={searchQuery}&ukprn={searchQuery}&companiesHouseNumber={searchQuery}";
            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new TramsApiException(response);
            }

            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagedDataResponse<TrustDto>>(apiResponse);

            var mappedResult = result.Data.Where(t => !string.IsNullOrEmpty(t.Ukprn) &&
                                                 t.Ukprn != outgoingTrustId)
                .Select(r => _trustMapper.Map(r)).ToList();

            return mappedResult;
        }

        public async Task<Trust> GetByUkprn(string ukprn)
        {
            var url = $"v4/trust/{ukprn}";
            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new TramsApiException(response);
            }

            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TrustDto>(apiResponse);
            var trust = _trustMapper.Map(result);

            return trust;
        }
    }
}