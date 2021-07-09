using System.Collections.Generic;
using Data.TRAMS.Models.EducationPerformance;

namespace Data.TRAMS.Tests.TestFixtures
{
    public static class EducationPerformance
    {
        public static TramsEducationPerformance GetSingleTramsEducationPerformance()
        {
            return new TramsEducationPerformance
            {
                KeyStage2 = new List<KeyStage2>
                {
                    new KeyStage2
                    {
                        Year = "test Year",
                        PercentageMeetingExpectedStdInRWM = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        PercentageAchievingHigherStdInRWM = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        ReadingProgressScore = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        WritingProgressScore = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        MathsProgressScore = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        NationalAveragePercentageMeetingExpectedStdInRWM = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        NationalAveragePercentageAchievingHigherStdInRWM = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        NationalAverageReadingProgressScore = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        NationalAverageWritingProgressScore = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        NationalAverageMathsProgressScore = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        LAAveragePercentageMeetingExpectedStdInRWM = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        LAAveragePercentageAchievingHigherStdInRWM = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        LAAverageReadingProgressScore = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        LAAverageWritingProgressScore = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        },
                        LAAverageMathsProgressScore = new DisadvantagedPupilsResponse
                        {
                            NotDisadvantaged = "15.0",
                            Disadvantaged = "12.0"
                        }
                    }
                }
            };
        }
    }
}