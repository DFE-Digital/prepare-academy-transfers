using System;
using System.Collections.Generic;
using System.Globalization;
using Data.Models;
using Data.TRAMS.Mappers.Response;
using Data.TRAMS.Models;
using Xunit;

namespace Data.TRAMS.Tests.Mappers.Response
{
    public class TramsEstablishmentMapperTests
    {
        private readonly TramsEstablishmentMapper _subject;

        public TramsEstablishmentMapperTests()
        {
            _subject = new TramsEstablishmentMapper();
        }

        [Fact]
        public void GivenTramsAcademy_MapsToInternalAcademySuccessfully()
        {
            var academyToMap = new TramsEstablishment
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
                    NumberOfBoys = "450",
                    NumberOfGirls = "455",
                    NumberOfPupils = "905"
                },
                EstablishmentName = "Fake Academy",
                EstablishmentType = new NameAndCode {Name = "Type of establishment"},
                LocalAuthorityName = "Fake LA",
                MisEstablishment = new MisEstablishment
                {
                    BehaviourAndAttitudes = "1",
                    EarlyYearsProvision = "9",
                    EffectivenessOfLeadershipAndManagement = "2",
                    OverallEffectiveness = "1",
                    PersonalDevelopment = "3",
                    QualityOfEducation = "4",
                    ReligiousEthos = "Does not apply",
                    SixthFormProvision = "1",
                    WebLink = "http://example.com"
                },
                SchoolCapacity = "1000",
                StatutoryLowAge = "4",
                StatutoryHighAge = "11",
                OfstedLastInspection = "01-01-2020",
                OfstedRating = "Good",
                PhaseOfEducation = new NameAndCode {Name = "Primary"},
                Ukprn = "Ukprn",
                Urn = "Urn"
            };

            var result = _subject.Map(academyToMap);
            var expectedAddress = new List<string> {"Example street", "Town", "Fakeshire", "FA11 1KE"};

            Assert.Equal(academyToMap.Ukprn, result.Ukprn);
            Assert.Equal(academyToMap.Urn, result.Urn);
            Assert.Equal(academyToMap.EstablishmentName, result.Name);
            Assert.Equal(expectedAddress, result.Address);
            Assert.Equal(academyToMap.LocalAuthorityName, result.LocalAuthorityName);
            Assert.Equal(academyToMap.EstablishmentType.Name, result.EstablishmentType);
            Assert.Equal(academyToMap.MisEstablishment.ReligiousEthos, result.FaithSchool);
            AssertGeneralInformationCorrect(result, academyToMap);
            AssertLatestOfstedJudgementCorrect(result);
            Assert.Equal(academyToMap.Census.NumberOfBoys, result.PupilNumbers.BoysOnRoll);
            Assert.Equal(academyToMap.Census.NumberOfGirls, result.PupilNumbers.GirlsOnRoll);
        }

        private static void AssertLatestOfstedJudgementCorrect(Academy result)
        {
            var latestOfstedJudgement = result.LatestOfstedJudgement;
            Assert.Equal("Fake Academy", latestOfstedJudgement.SchoolName);
            Assert.Equal("Outstanding", latestOfstedJudgement.OverallEffectiveness);
            Assert.Equal("01-01-2020", latestOfstedJudgement.InspectionDate);
            Assert.Equal("http://example.com", latestOfstedJudgement.OfstedReport);
        }

        private static void AssertGeneralInformationCorrect(Academy result, TramsEstablishment establishmentToMap)
        {
            var expectedAgeRange = $"{establishmentToMap.StatutoryLowAge} to {establishmentToMap.StatutoryHighAge}";
            var expectedPercentageFull = ExpectedPercentageFull(establishmentToMap);
            var generalInformation = result.GeneralInformation;
            Assert.Equal(establishmentToMap.PhaseOfEducation.Name, generalInformation.SchoolPhase);
            Assert.Equal(expectedAgeRange, generalInformation.AgeRange);
            Assert.Equal(establishmentToMap.SchoolCapacity, generalInformation.Capacity);
            Assert.Equal(establishmentToMap.Census.NumberOfPupils, generalInformation.NumberOnRoll);
            Assert.Equal(expectedPercentageFull, generalInformation.PercentageFull);
            Assert.Equal(establishmentToMap.EstablishmentType.Name, generalInformation.SchoolType);
        }

        private static string ExpectedPercentageFull(TramsEstablishment establishmentToMap)
        {
            return Math.Round(decimal.Parse(establishmentToMap.Census.NumberOfPupils) /
                decimal.Parse(establishmentToMap.SchoolCapacity) * 100, 1).ToString(CultureInfo.InvariantCulture);
        }
    }
}