using System.Collections.Generic;
using Data.TRAMS.Models;

namespace Data.TRAMS.Tests.TestFixtures
{
    public static class TrustSearchResults
    {
        public static List<TramsTrustSearchResult> GetTrustSearchResults(int numberOfResults = 1)
        {
            var counter = 1;

            var searchResults = new List<TramsTrustSearchResult>();

            for (var i = 0; i < numberOfResults; i++)
            {
                searchResults.Add(
                    new TramsTrustSearchResult
                    {
                        Establishments = new List<TramsTrustSearchEstablishment>(),
                        CompaniesHouseNumber = counter.ToString(),
                        GroupName = $"Trust {counter}",
                        Ukprn = counter.ToString()
                    }
                );
                counter++;
            }

            return searchResults;
        }
    }
}