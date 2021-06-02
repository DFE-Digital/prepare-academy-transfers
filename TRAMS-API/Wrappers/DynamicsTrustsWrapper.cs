using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Repositories;
using API.Repositories.Interfaces;
using Data;
using Data.Models;
using Data.Models.Academies;

namespace API.Wrappers
{
    public class DynamicsTrustsWrapper : ITrusts
    {
        private readonly ITrustsRepository _dynamicsTrustsRepository;
        private readonly IAcademiesRepository _dynamicsAcademiesRepository;

        public DynamicsTrustsWrapper(ITrustsRepository dynamicsTrustsRepository,
            IAcademiesRepository dynamicsAcademiesRepository)
        {
            _dynamicsTrustsRepository = dynamicsTrustsRepository;
            _dynamicsAcademiesRepository = dynamicsAcademiesRepository;
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
            var dynamicsAcademies = await _dynamicsAcademiesRepository.GetAcademiesByTrustId(Guid.Parse(ukprn));
            var result = dynamicsResult.Result;
            var academiesResult = dynamicsAcademies.Result;
            var mappedResult = new Trust
            {
                Name = result.TrustName,
                Ukprn = ukprn,
                EstablishmentType = result.EstablishmentType,
                CompaniesHouseNumber = result.CompaniesHouseNumber,
                GiasGroupId = result.TrustReferenceNumber,
                Address = Regex.Split(result.Address, "\\r\\n|,").ToList(),
                Academies = academiesResult.Select(academy => new Academy()
                {
                    Name = academy.AcademyName,
                    Ukprn = academy.Id.ToString(),
                    Urn = academy.Urn,
                    LocalAuthorityName = academy.LocalAuthorityName,
                    EstablishmentType = academy.EstablishmentType,
                    FaithSchool = academy.ReligiousCharacter,
                    Pfi = academy.Pfi,
                    LatestOfstedJudgement = new LatestOfstedJudgement()
                    {
                        OverallEffectiveness = academy.OfstedRating,
                        InspectionDate = academy.OfstedInspectionDate?.ToString("d MMMM yyyy")
                    }
                }).ToList()
            };

            return new RepositoryResult<Trust>
            {
                Result = mappedResult
            };
        }
    }
}