using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Mappers.Response;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Helpers;
using Xunit;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests.Mappers.Response
{
    public class TramsEstablishmentMapperTests
    {
        private readonly TramsEstablishmentMapper _subject;
        private TramsEstablishment _tramsEstablishment;

        public TramsEstablishmentMapperTests()
        {
            _subject = new TramsEstablishmentMapper();
            _tramsEstablishment = new TramsEstablishment
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
                    NumberOfPupils = "905",
                    NumberOfBoys = "450",
                    NumberOfGirls = "455",
                    PercentageFsm = "15.9",
                    PercentageSen = "19.5",
                    PercentageEnglishNotFirstLanguage = "4.7",
                    PercentageEligableForFSM6Years = "2.9"
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
                    WebLink = "http://example.com",
                    InspectionEndDate = "01-01-2020",
                    DateOfLatestSection8Inspection = "01-01-2020"
                },
                SchoolCapacity = "1000",
                StatutoryLowAge = "4",
                StatutoryHighAge = "11",
                OfstedLastInspection = "01-01-2020",
                OfstedRating = "Good",
                PhaseOfEducation = new NameAndCode {Name = "Primary"},
                Ukprn = "Ukprn",
                Urn = "Urn",
                ViewAcademyConversion = new ViewAcademyConversion
                {
                    Deficit = "Deficit",
                    Pan = "Pan",
                    Pfi = "Pfi",
                    ViabilityIssue = "Viability issue"
                }
            };
        }

        [Fact]
        public void GivenTramsAcademy_MapsToInternalAcademySuccessfully()
        {
            var academyToMap = _tramsEstablishment;

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
            Assert.Equal(PercentageHelper.CalculatePercentageFromStrings(academyToMap.Census.NumberOfBoys, academyToMap.Census.NumberOfPupils), result.PupilNumbers.BoysOnRoll);
            Assert.Equal(PercentageHelper.CalculatePercentageFromStrings(academyToMap.Census.NumberOfGirls, academyToMap.Census.NumberOfPupils), result.PupilNumbers.GirlsOnRoll);
            Assert.Equal(academyToMap.Census.PercentageEnglishNotFirstLanguage+"%", result.PupilNumbers.WhoseFirstLanguageIsNotEnglish);
            Assert.Equal(academyToMap.Census.PercentageEligableForFSM6Years+"%", result.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years);
            Assert.Equal(academyToMap.Census.PercentageSen+"%", result.PupilNumbers.WithStatementOfSen);
        }

        [Fact]
        public void GiveNullPercentages_ReturnsCorrectValue()
        {
            var academyToMap = _tramsEstablishment;
            academyToMap.Census.PercentageSen = null;
            academyToMap.Census.NumberOfPupils = null;
            academyToMap.Census.NumberOfBoys = null;
            academyToMap.Census.NumberOfGirls = null;
            academyToMap.Census.PercentageEligableForFSM6Years = null;
            academyToMap.Census.PercentageEnglishNotFirstLanguage = null;
            
            var result = _subject.Map(academyToMap);
            Assert.Null(result.PupilNumbers.BoysOnRoll);
            Assert.Null(result.PupilNumbers.GirlsOnRoll);
            Assert.Null(result.PupilNumbers.WhoseFirstLanguageIsNotEnglish);
            Assert.Null(result.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years);
            Assert.Null(result.PupilNumbers.WithStatementOfSen);
        }
        
        [Fact]
        public void GiveNullMisEstablishment_ReturnsCorrectValue()
        {
            var academyToMap = _tramsEstablishment;
            academyToMap.MisEstablishment = null;

            var result = _subject.Map(academyToMap);
            Assert.Null(result.FaithSchool);
            Assert.Equal("N/A", result.LatestOfstedJudgement.OverallEffectiveness);
            Assert.Null(result.LatestOfstedJudgement.OfstedReport);
        }

        [Fact]
        public void GivenViewAcademyConversionNull_DontPopulateInformation()
        {
            var academyToMap = _tramsEstablishment;
            academyToMap.ViewAcademyConversion = null;
            
            var result = _subject.Map(academyToMap);
            Assert.True(string.IsNullOrEmpty(result.GeneralInformation.Deficit));
            Assert.True(string.IsNullOrEmpty(result.GeneralInformation.Pan));
            Assert.True(string.IsNullOrEmpty(result.GeneralInformation.Pfi));
            Assert.True(string.IsNullOrEmpty(result.GeneralInformation.ViabilityIssue));
        }

        private static void AssertLatestOfstedJudgementCorrect(Academy result)
        {
            var latestOfstedJudgement = result.LatestOfstedJudgement;
            Assert.Equal("Fake Academy", latestOfstedJudgement.SchoolName);
            Assert.Equal("Outstanding", latestOfstedJudgement.OverallEffectiveness);
            Assert.Equal("01-01-2020", latestOfstedJudgement.InspectionEndDate);
            Assert.Equal("http://example.com", latestOfstedJudgement.OfstedReport);
            Assert.Equal("01-01-2020", latestOfstedJudgement.InspectionEndDate);
            Assert.Equal("Inadequate", latestOfstedJudgement.QualityOfEducation);
            Assert.Equal("Outstanding", latestOfstedJudgement.BehaviourAndAttitudes);
            Assert.Equal("Requires improvement", latestOfstedJudgement.PersonalDevelopment);
            Assert.Equal("Good", latestOfstedJudgement.EffectivenessOfLeadershipAndManagement);
            Assert.Equal("No data", latestOfstedJudgement.EarlyYearsProvision);
            Assert.Equal("Outstanding", latestOfstedJudgement.SixthFormProvision);
            Assert.Equal("01-01-2020", latestOfstedJudgement.DateOfLatestSection8Inspection);
        }

        private static void AssertGeneralInformationCorrect(Academy result, TramsEstablishment establishmentToMap)
        {
            var expectedAgeRange = $"{establishmentToMap.StatutoryLowAge} to {establishmentToMap.StatutoryHighAge}";
            var expectedPercentageFull = PercentageHelper.CalculatePercentageFromStrings(establishmentToMap.Census.NumberOfPupils, establishmentToMap.SchoolCapacity);
            var generalInformation = result.GeneralInformation;
            var viewAcademyConversion = establishmentToMap.ViewAcademyConversion;
            Assert.Equal(establishmentToMap.PhaseOfEducation.Name, generalInformation.SchoolPhase);
            Assert.Equal(expectedAgeRange, generalInformation.AgeRange);
            Assert.Equal(establishmentToMap.SchoolCapacity, generalInformation.Capacity);
            Assert.Equal(establishmentToMap.Census.NumberOfPupils, generalInformation.NumberOnRoll);
            Assert.Equal($"{establishmentToMap.Census.PercentageFsm}%", generalInformation.PercentageFsm);
            Assert.Equal(expectedPercentageFull, generalInformation.PercentageFull);
            Assert.Equal(establishmentToMap.EstablishmentType.Name, generalInformation.SchoolType);
            Assert.Equal(viewAcademyConversion.Deficit, generalInformation.Deficit);
            Assert.Equal(viewAcademyConversion.Pan, generalInformation.Pan);
            Assert.Equal(viewAcademyConversion.Pfi, generalInformation.Pfi);
            Assert.Equal(viewAcademyConversion.ViabilityIssue, generalInformation.ViabilityIssue);
        }
    }
}