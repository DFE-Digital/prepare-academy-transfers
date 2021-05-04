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

            var resultToMap = new TramsTrustSearchResult()
            {
                Urn = "1234",
                GroupName = "Group name",
                CompaniesHouseNumber = "12345",
                Academies = new List<TramsTrustSearchAcademy>()
                {
                    new TramsTrustSearchAcademy() {Name = "Academy 1 name", Urn = "0001"},
                    new TramsTrustSearchAcademy() {Name = "Academy 2 name", Urn = "0002"}
                }
            };

            var result = subject.Map(resultToMap);

            Assert.Equal(resultToMap.Urn, result.Ukprn);
            Assert.Equal(resultToMap.GroupName, result.TrustName);
            Assert.Equal(resultToMap.CompaniesHouseNumber, result.CompaniesHouseNumber);

            AssertAcademyMappedCorrectly(resultToMap.Academies[0], result.Academies[0]);
            AssertAcademyMappedCorrectly(resultToMap.Academies[1], result.Academies[1]);
        }

        private static void AssertAcademyMappedCorrectly(TramsTrustSearchAcademy academyToMap, Data.Models.TrustSearchAcademy mappedAcademy)
        {
            Assert.Equal(academyToMap.Name, mappedAcademy.Name);
            Assert.Equal(academyToMap.Urn, mappedAcademy.Ukprn);
        }
    }
}