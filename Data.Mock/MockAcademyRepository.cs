using System.Threading.Tasks;
using Data.Models;
using Data.Models.Academies;

namespace Data.Mock
{
    public class MockAcademyRepository : IAcademies
    {
        public Task<RepositoryResult<Academy>> GetAcademyByUkprn(string ukprn)
        {
            var academy = new Academy
            {
                Ukprn = ukprn,
                Name = "Placeholder",
                GeneralInformation = new GeneralInformation
                {
                    SchoolPhase = "Placeholder",
                    AgeRange = "Placeholder",
                    Capacity = "Placeholder",
                    NumberOnRoll = "Placeholder",
                    Pan = "Placeholder",
                    Pfi = "Placeholder",
                    ViabilityIssue = "Placeholder",
                    Deficit = "Placeholder",
                    SchoolType = "Placeholder",
                    DiocesesPercent = "Placeholder",
                    DistanceToSponsorHq = "Placeholder",
                    MpAndParty = "Placeholder"
                },

                PupilNumbers = new PupilNumbers
                {
                    GirlsOnRoll = "Placeholder",
                    BoysOnRoll = "Placeholder",
                    WithStatementOfSen = "Placeholder",
                    WhoseFirstLanguageIsNotEnglish = "Placeholder",
                    PercentageEligibleForFreeSchoolMealsDuringLast6Years = "Placeholder"
                },

                LatestOfstedJudgement = new LatestOfstedJudgement
                {
                    OverallEffectiveness = "Placeholder",
                    InspectionEndDate = "Placeholder",
                    SchoolName = "Placeholder",
                    OfstedReport = "Placeholder"
                }
            };

            var result = new RepositoryResult<Academy>()
            {
                Result = academy
            };

            return Task.FromResult(result);
        }
    }
}