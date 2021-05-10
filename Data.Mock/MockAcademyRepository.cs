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
                Performance = new AcademyPerformance
                {
                    SchoolPhase = "Placeholder",
                    AgeRange = "Placeholder",
                    Capacity = "Placeholder",
                    Nor = "Placeholder",
                    Pan = "Placeholder",
                    Pfi = "Placeholder",
                    ViabilityIssue = "Placeholder",
                    Deficit = "Placeholder",
                    SchoolType = "Placeholder",
                    DiocesesPercent = "Placeholder",
                    DistanceToSponsorHq = "Placeholder",
                    MpAndParty = "Placeholder",
                    OfstedJudgementDate = "Placeholder",
                    CurrentFramework = "Placeholder",
                    AchievementOfPupil = "Placeholder",
                    QualityOfTeaching = "Placeholder",
                    BehaviourAndSafetyOfPupil = "Placeholder",
                    LeadershipAndManagement = "Placeholder"
                },

                PupilNumbers = new PupilNumbers
                {
                    GirlsOnRoll = "Placeholder",
                    BoysOnRoll = "Placeholder",
                    WithStatementOfSen = "Placeholder",
                    WhoseFirstLanguageIsNotEnglish = "Placeholder",
                    EligibleForFreeSchoolMeals = "Placeholder"
                },

                LatestOfstedJudgement = new LatestOfstedJudgement
                {
                    AchievementOfPupils = "Placeholder",
                    BehaviourAndSafetyOfPupils = "Placeholder",
                    EarlyYearsProvision = "Placeholder",
                    OverallEffectiveness = "Placeholder",
                    InspectionDate = "Placeholder",
                    LeadershipAndManagement = "Placeholder",
                    QualityOfTeaching = "Placeholder",
                    SchoolName = "Placeholder"
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