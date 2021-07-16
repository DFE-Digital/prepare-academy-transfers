using System;
using System.Collections.Generic;
using Data.TRAMS.Models.EducationPerformance;

namespace Data.TRAMS.Tests.TestFixtures
{
    public static class EducationPerformance
    {
        private static readonly Random RandomGenerator = new Random();
        
        public static TramsEducationPerformance GetSingleTramsEducationPerformance()
        {
            return new TramsEducationPerformance
            {
                KeyStage2 = new List<KeyStage2>
                {
                    new KeyStage2
                    {
                        Year = "test ks3 Year",
                        PercentageMeetingExpectedStdInRWM = GetTestResult(),
                        PercentageAchievingHigherStdInRWM = GetTestResult(),
                        ReadingProgressScore = GetTestResult(),
                        WritingProgressScore = GetTestResult(),
                        MathsProgressScore = GetTestResult(),
                        NationalAveragePercentageMeetingExpectedStdInRWM = GetTestResult(),
                        NationalAveragePercentageAchievingHigherStdInRWM = GetTestResult(),
                        NationalAverageReadingProgressScore = GetTestResult(),
                        NationalAverageWritingProgressScore = GetTestResult(),
                        NationalAverageMathsProgressScore = GetTestResult(),
                        LAAveragePercentageMeetingExpectedStdInRWM = GetTestResult(),
                        LAAveragePercentageAchievingHigherStdInRWM = GetTestResult(),
                        LAAverageReadingProgressScore = GetTestResult(),
                        LAAverageWritingProgressScore = GetTestResult(),
                        LAAverageMathsProgressScore = GetTestResult(),
                    }
                },
                KeyStage4 = new List<KeyStage4>
                {
                    new KeyStage4
                    {
                        Year = "test ks4 year",
                        SipAttainment8score = GetTestResult(),
                        SipAttainment8scoreenglish = GetTestResult(),
                        SipAttainment8scoremaths = GetTestResult(),
                        SipAttainment8scoreebacc = GetTestResult(),
                        SipNumberofpupilsprogress8 = GetTestResult(),
                        SipProgress8upperconfidence = new decimal(RandomGenerator.NextDouble()),
                        SipProgress8lowerconfidence = new decimal(RandomGenerator.NextDouble()),
                        SipProgress8english = GetTestResult(),
                        SipProgress8maths = GetTestResult(),
                        SipProgress8ebacc = GetTestResult(),
                        SipProgress8Score = GetTestResult(),
                        NationalAverageA8Score = GetTestResult(),
                        NationalAverageA8English = GetTestResult(),
                        NationalAverageA8Maths = GetTestResult(),
                        NationalAverageA8EBacc = GetTestResult(),
                        NationalAverageP8PupilsIncluded =
                            GetTestResult(),
                        NationalAverageP8Score = GetTestResult(),
                        NationalAverageP8LowerConfidence = new decimal(RandomGenerator.NextDouble()),
                        NationalAverageP8UpperConfidence = new decimal(RandomGenerator.NextDouble()),
                        NationalAverageP8English = GetTestResult(),
                        NationalAverageP8Maths = GetTestResult(),
                        NationalAverageP8Ebacc = GetTestResult(),
                        LAAverageA8Score = GetTestResult(),
                        LAAverageA8English = GetTestResult(),
                        LAAverageA8Maths = GetTestResult(),
                        LAAverageA8EBacc = GetTestResult(),
                        LAAverageP8PupilsIncluded = GetTestResult(),
                        LAAverageP8Score = GetTestResult(),
                        LAAverageP8LowerConfidence = new decimal(RandomGenerator.NextDouble()),
                        LAAverageP8UpperConfidence = new decimal(RandomGenerator.NextDouble()),
                        LAAverageP8English = GetTestResult(),
                        LAAverageP8Maths = GetTestResult(),
                        LAAverageP8Ebacc = GetTestResult(),
                    }
                }
            };
        }

        private static DisadvantagedPupilsResponse GetTestResult()
        {
            return new DisadvantagedPupilsResponse
            {
                NotDisadvantaged = RandomGenerator.NextDouble().ToString(),
                Disadvantaged = RandomGenerator.NextDouble().ToString()
            };
        }
    }
}