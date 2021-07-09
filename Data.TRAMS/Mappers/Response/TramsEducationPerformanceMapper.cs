using System.Linq;
using Data.Models.KeyStagePerformance;
using Data.TRAMS.Models;
using Data.TRAMS.Models.EducationPerformance;
using KeyStage2 = Data.Models.KeyStagePerformance.KeyStage2;
using Trams = Data.TRAMS.Models;

namespace Data.TRAMS.Mappers.Response
{
    public class TramsEducationPerformanceMapper : IMapper<TramsEducationPerformance, EducationPerformance>
    {
        public EducationPerformance Map(TramsEducationPerformance input)
        {
            return new EducationPerformance
            {
                KeyStage2Performance = input.KeyStage2.Select(
                    ks2Result => new KeyStage2
                    {
                        Year = ks2Result.Year,
                        PercentageMeetingExpectedStdInRWM = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.PercentageMeetingExpectedStdInRWM.NotDisadvantaged,
                            Disadvantaged = ks2Result.PercentageMeetingExpectedStdInRWM.Disadvantaged
                        },
                        PercentageAchievingHigherStdInRWM = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.PercentageAchievingHigherStdInRWM.NotDisadvantaged,
                            Disadvantaged = ks2Result.PercentageAchievingHigherStdInRWM.Disadvantaged
                        },
                        ReadingProgressScore = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.ReadingProgressScore.NotDisadvantaged,
                            Disadvantaged = ks2Result.ReadingProgressScore.Disadvantaged
                        },
                        WritingProgressScore = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.WritingProgressScore.NotDisadvantaged,
                            Disadvantaged = ks2Result.WritingProgressScore.Disadvantaged
                        },
                        MathsProgressScore = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.MathsProgressScore.NotDisadvantaged,
                            Disadvantaged = ks2Result.MathsProgressScore.Disadvantaged
                        },
                        NationalAveragePercentageMeetingExpectedStdInRWM = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.NationalAveragePercentageMeetingExpectedStdInRWM
                                .NotDisadvantaged,
                            Disadvantaged = ks2Result.NationalAveragePercentageMeetingExpectedStdInRWM.Disadvantaged
                        },
                        NationalAveragePercentageAchievingHigherStdInRWM = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.NationalAveragePercentageAchievingHigherStdInRWM
                                .NotDisadvantaged,
                            Disadvantaged = ks2Result.NationalAveragePercentageAchievingHigherStdInRWM.Disadvantaged
                        },
                        NationalAverageReadingProgressScore = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.NationalAverageReadingProgressScore.NotDisadvantaged,
                            Disadvantaged = ks2Result.NationalAverageReadingProgressScore.Disadvantaged
                        },
                        NationalAverageWritingProgressScore = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.NationalAverageWritingProgressScore.NotDisadvantaged,
                            Disadvantaged = ks2Result.NationalAverageWritingProgressScore.Disadvantaged
                        },
                        NationalAverageMathsProgressScore = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.NationalAverageMathsProgressScore.NotDisadvantaged,
                            Disadvantaged = ks2Result.NationalAverageMathsProgressScore.Disadvantaged
                        },
                        LAAveragePercentageMeetingExpectedStdInRWM = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.LAAveragePercentageMeetingExpectedStdInRWM.NotDisadvantaged,
                            Disadvantaged = ks2Result.LAAveragePercentageMeetingExpectedStdInRWM.Disadvantaged
                        },
                        LAAveragePercentageAchievingHigherStdInRWM = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.LAAveragePercentageAchievingHigherStdInRWM.NotDisadvantaged,
                            Disadvantaged = ks2Result.LAAveragePercentageAchievingHigherStdInRWM.Disadvantaged
                        },
                        LAAverageReadingProgressScore = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.LAAverageReadingProgressScore.NotDisadvantaged,
                            Disadvantaged = ks2Result.LAAverageReadingProgressScore.Disadvantaged
                        },
                        LAAverageWritingProgressScore = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.LAAverageWritingProgressScore.NotDisadvantaged,
                            Disadvantaged = ks2Result.LAAverageWritingProgressScore.Disadvantaged
                        },
                        LAAverageMathsProgressScore = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = ks2Result.LAAverageMathsProgressScore.NotDisadvantaged,
                            Disadvantaged = ks2Result.LAAverageMathsProgressScore.Disadvantaged
                        }
                    }).ToList()
            };
        }
    }
}