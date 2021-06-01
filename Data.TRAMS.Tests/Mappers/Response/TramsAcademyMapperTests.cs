using System;
using System.Collections.Generic;
using System.Globalization;
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
            var academyToMap = new TramsAcademy
            {
                Address = new Address
                {
                    Street = "Example street",
                    Town = "Town",
                    County = "Fakeshire",
                    Postcode = "FA11 1KE"
                },
                Census = new Census
                {
                    NumberOfPupils = "905"
                },
                EstablishmentName = "Fake Academy",
                EstablishmentType = new NameAndCode {Name = "Type of establishment"},
                LocalAuthorityName = "Fake LA",
                SchoolCapacity = "1000",
                StatutoryLowAge = "4",
                StatutoryHighAge = "11",
                OfstedLastInspection = "01-01-2020",
                OfstedRating = "Good",
                PhaseOfEducation = new NameAndCode {Name = "Primary"},
                Ukprn = "Ukprn"
            };

            var result = _subject.Map(academyToMap);
            var expectedAddress = new List<string> {"Example street", "Town", "Fakeshire", "FA11 1KE"};

            Assert.Equal(academyToMap.Ukprn, result.Ukprn);
            Assert.Equal(academyToMap.EstablishmentName, result.Name);
            Assert.Equal(expectedAddress, result.Address);
            Assert.Equal(academyToMap.LocalAuthorityName, result.LocalAuthorityName);
            Assert.Equal(academyToMap.EstablishmentType.Name, result.EstablishmentType);
            Assert.Equal(academyToMap.MisEstablishment.ReligiousEthos, result.FaithSchool);
            AssertAcademyPerformanceCorrect(result, academyToMap);
            Assert.Equal(academyToMap.OfstedRating, result.LatestOfstedJudgement.OverallEffectiveness);
        }

        private static void AssertAcademyPerformanceCorrect(Academy result, TramsAcademy academyToMap)
        {
            var expectedAgeRange = $"{academyToMap.StatutoryLowAge} to {academyToMap.StatutoryHighAge}";
            var expectedPercentageFull = ExpectedPercentageFull(academyToMap);
            var performance = result.Performance;
            Assert.Equal(academyToMap.PhaseOfEducation.Name, performance.SchoolPhase);
            Assert.Equal(expectedAgeRange, performance.AgeRange);
            Assert.Equal(academyToMap.SchoolCapacity, performance.Capacity);
            Assert.Equal(academyToMap.Census.NumberOfPupils, performance.NumberOnRoll);
            Assert.Equal(expectedPercentageFull, performance.PercentageFull);
            Assert.Equal(academyToMap.EstablishmentType.Name, performance.SchoolType);
            Assert.Equal(academyToMap.OfstedRating, performance.OfstedRating);
            Assert.Equal(academyToMap.OfstedLastInspection, performance.OfstedJudgementDate);
        }

        private static string ExpectedPercentageFull(TramsAcademy academyToMap)
        {
            return Math.Round(decimal.Parse(academyToMap.Census.NumberOfPupils) /
                decimal.Parse(academyToMap.SchoolCapacity) * 100, 1).ToString(CultureInfo.InvariantCulture);
        }
    }
}