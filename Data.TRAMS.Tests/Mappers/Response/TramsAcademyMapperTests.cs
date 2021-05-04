using System.Collections.Generic;
using Data.Models;
using Data.TRAMS.Mappers.Response;
using Data.TRAMS.Models;
using Xunit;

namespace Data.TRAMS.Tests.Mappers.Response
{
    public class TramsAcademyMapperTests
    {
        private readonly TramsAcademyMapper _subject;

        public TramsAcademyMapperTests()
        {
            _subject = new TramsAcademyMapper();
        }

        [Fact]
        public void GivenTramsAcademy_MapsToInternalAcademySuccessfully()
        {
            var academyToMap = new TramsAcademy()
            {
                Ukprn = "Ukprn",
                EstablishmentName = "Fake Academy",
                Address = new Address()
                {
                    Street = "Example street",
                    Town = "Town",
                    County = "Fakeshire",
                    Postcode = "FA11 1KE"
                },
                StatutoryLowAge = "4",
                StatutoryHighAge = "11"
            };

            var result = _subject.Map(academyToMap);
            var expectedAddress = new List<string> {"Example street", "Town", "Fakeshire", "FA11 1KE"};

            Assert.Equal(academyToMap.Ukprn, result.Ukprn);
            Assert.Equal(academyToMap.EstablishmentName, result.Name);
            Assert.Equal(expectedAddress, result.Address);
            AssertAcademyPerformanceCorrect(result, academyToMap);
        }

        private static void AssertAcademyPerformanceCorrect(Academy result, TramsAcademy academyToMap)
        {
            var expectedAgeRange = $"{academyToMap.StatutoryLowAge} to {academyToMap.StatutoryHighAge}";
            var performance = result.Performance;
            Assert.Equal(expectedAgeRange, performance.AgeRange);
            Assert.Equal(academyToMap.SchoolCapacity, performance.Capacity);
        }
    }
}