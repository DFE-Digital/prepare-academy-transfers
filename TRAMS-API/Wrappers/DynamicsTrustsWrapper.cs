using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Repositories.Interfaces;
using Data;
using Data.Models;

namespace API.Wrappers
{
    public class DynamicsTrustsWrapper : ITrusts
    {
        private readonly ITrustsRepository _dynamicsTrustsRepository;

        public DynamicsTrustsWrapper(ITrustsRepository dynamicsTrustsRepository)
        {
            _dynamicsTrustsRepository = dynamicsTrustsRepository;
        }

        public async Task<RepositoryResult<List<TrustSearchResult>>> SearchTrusts(string searchQuery)
        {
            var searchResults = await _dynamicsTrustsRepository.SearchTrusts(searchQuery);
            var mappedResults = searchResults.Result.Select(result =>
                new TrustSearchResult
                {
                    Ukprn = result.Id.ToString(),
                    CompaniesHouseNumber = result.CompaniesHouseNumber,
                    TrustName = result.TrustName
                }
            ).ToList();

            return new RepositoryResult<List<TrustSearchResult>>
            {
                Result = mappedResults
            };
        }

        public async Task<RepositoryResult<Trust>> GetByUkprn(string ukprn)
        {
            var dynamicsResult = await _dynamicsTrustsRepository.GetTrustById(Guid.Parse(ukprn));
            var result = dynamicsResult.Result;
            var mappedResult = new Trust
            {
                Name = result.TrustName,
                Ukprn = ukprn,
                EstablishmentType = result.EstablishmentType,
                CompaniesHouseNumber = result.CompaniesHouseNumber,
                GiasGroupId = result.TrustReferenceNumber,
                Address = Regex.Split(result.Address, "\\r\\n|,").ToList()
            };

            return new RepositoryResult<Trust>
            {
                Result = mappedResult
            };
        }
    }
}