using System.Collections.Generic;
using Data.TRAMS.Mappers.Response;
using Data.TRAMS.Models;
using Xunit;

namespace Data.TRAMS.Tests.Mappers.Response
{
    public class TramsSearchResultMapperTests
    {
        [Fact]
        public void GivenSearchResult_MapsCorrectly()
        {
            var subject = new TramsSearchResultMapper();

            var resultToMap = new TramsTrustSearchResult
            {
                Ukprn = "1234",
                GroupName = "Group name",
                CompaniesHouseNumber = "12345",
                Establishments = new List<TramsTrustSearchEstablishment>
                {
                    new TramsTrustSearchEstablishment {Name = "Academy 1 name", Urn = "0001", Ukprn = "1001"},
                    new TramsTrustSearchEstablishment {Name = "Academy 2 name", Urn = "0002", Ukprn = "2001"}
                }
            };

            var result = subject.Map(resultToMap);

            Assert.Equal(resultToMap.Ukprn, result.Ukprn);
            Assert.Equal(resultToMap.GroupName, result.TrustName);
            Assert.Equal(resultToMap.CompaniesHouseNumber, result.CompaniesHouseNumber);

            AssertAcademyMappedCorrectly(resultToMap.Establishments[0], result.Academies[0]);
            AssertAcademyMappedCorrectly(resultToMap.Establishments[1], result.Academies[1]);
        }

        private static void AssertAcademyMappedCorrectly(TramsTrustSearchEstablishment establishmentToMap,
            Data.Models.TrustSearchAcademy mappedAcademy)
        {
            Assert.Equal(establishmentToMap.Name, mappedAcademy.Name);
            Assert.Equal(establishmentToMap.Urn, mappedAcademy.Urn);
            Assert.Equal(establishmentToMap.Ukprn, mappedAcademy.Ukprn);
        }
    }
}