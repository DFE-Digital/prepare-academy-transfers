using System.Collections.Generic;
using Dfe.Academies.Contracts.V4;
using Dfe.Academies.Contracts.V4.Trusts;
using Dfe.PrepareTransfers.Data.TRAMS.Models;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests.TestFixtures
{
    public static class TrustSearchResults
    {
        public static PagedDataResponse<TrustDto> GetTrustSearchResults(int numberOfResults = 1)
        {
            var counter = 1;

            var searchResults = new PagedDataResponse<TrustDto>();
            var trusts = new List<TrustDto>();

            for (var i = 0; i < numberOfResults; i++)
            {
                trusts.Add(new TrustDto
                {
                    CompaniesHouseNumber = counter.ToString(),
                    Name = $"Trust {counter}",
                    Ukprn = counter.ToString()
                }
                );
                counter++;
            }
            searchResults.Data = trusts;

            return searchResults;
        }
    }
}