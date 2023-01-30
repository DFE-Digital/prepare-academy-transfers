using System.Linq;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.Data.TRAMS.Models.EducationPerformance;
using KeyStage2 = Dfe.PrepareTransfers.Data.Models.KeyStagePerformance.KeyStage2;
using KeyStage4 = Dfe.PrepareTransfers.Data.Models.KeyStagePerformance.KeyStage4;
using KeyStage5 = Dfe.PrepareTransfers.Data.Models.KeyStagePerformance.KeyStage5;
using Trams = Dfe.PrepareTransfers.Data.TRAMS.Models;

namespace Dfe.PrepareTransfers.Data.TRAMS.Mappers.Response
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
                    }).ToList(),
                KeyStage4Performance = input.KeyStage4.Select(
                    ks4Result => new KeyStage4
                    {
                        Year = ks4Result.Year,
                 SipAttainment8score = new DisadvantagedPupilsResult
                 {
                     NotDisadvantaged = ks4Result.SipAttainment8score.NotDisadvantaged,
                     Disadvantaged = ks4Result.SipAttainment8score.Disadvantaged
                 },
                 SipAttainment8scoreenglish = new DisadvantagedPupilsResult
                 {
                     NotDisadvantaged = ks4Result.SipAttainment8scoreenglish.NotDisadvantaged,
                     Disadvantaged = ks4Result.SipAttainment8scoreenglish.Disadvantaged
                 },
                 SipAttainment8scoremaths = new DisadvantagedPupilsResult
                 {
                     NotDisadvantaged = ks4Result.SipAttainment8scoremaths.NotDisadvantaged,
                     Disadvantaged = ks4Result.SipAttainment8scoremaths.Disadvantaged
                 },
                 SipAttainment8scoreebacc = new DisadvantagedPupilsResult
                 {
                     NotDisadvantaged = ks4Result.SipAttainment8scoreebacc.NotDisadvantaged,
                     Disadvantaged = ks4Result.SipAttainment8scoreebacc.Disadvantaged
                 },
                 SipNumberofpupilsprogress8 = new DisadvantagedPupilsResult
                 {
                     NotDisadvantaged = ks4Result.SipNumberofpupilsprogress8.NotDisadvantaged,
                     Disadvantaged = ks4Result.SipNumberofpupilsprogress8.Disadvantaged
                 },
                 SipProgress8upperconfidence = ks4Result.SipProgress8upperconfidence,
                 SipProgress8lowerconfidence = ks4Result.SipProgress8lowerconfidence,
                 SipProgress8english = new DisadvantagedPupilsResult
                 {
                     NotDisadvantaged = ks4Result.SipProgress8english.NotDisadvantaged,
                     Disadvantaged = 
                         ks4Result.SipProgress8english.Disadvantaged
                 },
                 SipProgress8maths = new DisadvantagedPupilsResult
                 {
                     NotDisadvantaged = ks4Result.SipProgress8maths.NotDisadvantaged,
                     Disadvantaged = ks4Result.SipProgress8maths.Disadvantaged
                 },
                 SipProgress8ebacc = new DisadvantagedPupilsResult
                 {
                     NotDisadvantaged = ks4Result.SipProgress8ebacc.NotDisadvantaged,
                     Disadvantaged = ks4Result.SipProgress8ebacc.Disadvantaged
                 },
                 SipProgress8Score = new DisadvantagedPupilsResult
                 {
                     NotDisadvantaged = ks4Result.SipProgress8Score.NotDisadvantaged,
                     Disadvantaged = ks4Result.SipProgress8Score.Disadvantaged
                 },
                 NationalAverageA8Score = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.NationalAverageA8Score.NotDisadvantaged,
                    Disadvantaged = ks4Result.NationalAverageA8Score.Disadvantaged
                },
                NationalAverageA8English = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.NationalAverageA8English.NotDisadvantaged,
                    Disadvantaged = ks4Result.NationalAverageA8English.Disadvantaged
                },
                NationalAverageA8Maths = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.NationalAverageA8Maths.NotDisadvantaged,
                    Disadvantaged = ks4Result.NationalAverageA8Maths.Disadvantaged
                },
                NationalAverageA8EBacc = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.NationalAverageA8EBacc.NotDisadvantaged,
                    Disadvantaged = ks4Result.NationalAverageA8EBacc.Disadvantaged
                },
                NationalAverageP8PupilsIncluded = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.NationalAverageP8PupilsIncluded.NotDisadvantaged,
                    Disadvantaged = ks4Result.NationalAverageP8PupilsIncluded.Disadvantaged
                },
                NationalAverageP8Score = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.NationalAverageP8Score.NotDisadvantaged,
                    Disadvantaged = ks4Result.NationalAverageP8Score.Disadvantaged
                },
                NationalAverageP8English = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.NationalAverageP8English.NotDisadvantaged,
                    Disadvantaged = ks4Result.NationalAverageP8English.Disadvantaged
                },
                NationalAverageP8Maths = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.NationalAverageP8Maths.NotDisadvantaged,
                    Disadvantaged = ks4Result.NationalAverageP8Maths.Disadvantaged
                },
                NationalAverageP8Ebacc = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.NationalAverageP8Ebacc.NotDisadvantaged,
                    Disadvantaged = ks4Result.NationalAverageP8Ebacc.Disadvantaged
                },
                NationalAverageP8LowerConfidence = ks4Result.NationalAverageP8LowerConfidence,
                NationalAverageP8UpperConfidence = ks4Result.NationalAverageP8UpperConfidence,
                LAAverageA8Score = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.LAAverageA8Score.NotDisadvantaged,
                    Disadvantaged = ks4Result.LAAverageA8Score.Disadvantaged
                },
                LAAverageA8English = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.LAAverageA8English.NotDisadvantaged,
                    Disadvantaged = ks4Result.LAAverageA8English.Disadvantaged
                },
                LAAverageA8Maths = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.LAAverageA8Maths.NotDisadvantaged,
                    Disadvantaged = ks4Result.LAAverageA8Maths.Disadvantaged
                },
                LAAverageA8EBacc = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.LAAverageA8EBacc.NotDisadvantaged,
                    Disadvantaged = ks4Result.LAAverageA8EBacc.Disadvantaged
                },
                LAAverageP8PupilsIncluded = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.LAAverageP8PupilsIncluded.NotDisadvantaged,
                    Disadvantaged = ks4Result.LAAverageP8PupilsIncluded.Disadvantaged
                },
                LAAverageP8Score = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.LAAverageP8Score.NotDisadvantaged,
                    Disadvantaged = ks4Result.LAAverageP8Score.Disadvantaged
                },
                LAAverageP8English = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.LAAverageP8English.NotDisadvantaged,
                    Disadvantaged = ks4Result.LAAverageP8English.Disadvantaged
                },
                LAAverageP8Maths = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.LAAverageP8Maths.NotDisadvantaged,
                    Disadvantaged = ks4Result.LAAverageP8Maths.Disadvantaged
                },
                LAAverageP8Ebacc = new DisadvantagedPupilsResult
                {
                    NotDisadvantaged = ks4Result.LAAverageP8Ebacc.NotDisadvantaged,
                    Disadvantaged = ks4Result.LAAverageP8Ebacc.Disadvantaged
                },
                LAAverageP8LowerConfidence = ks4Result.LAAverageP8LowerConfidence,
                LAAverageP8UpperConfidence = ks4Result.LAAverageP8UpperConfidence,
                Enteringebacc = ks4Result.Enteringebacc,
                LAEnteringEbacc = ks4Result.LAEnteringEbacc,
                NationalEnteringEbacc = ks4Result.NationalEnteringEbacc
                    }).ToList() ,
                KeyStage5Performance = input.KeyStage5.Select(ks5Result =>
                    new KeyStage5
                    {
                        Year = ks5Result.Year,
                        Academy = new KeyStage5Result
                        {
                            AcademicAverage = ks5Result.AcademicQualificationAverage,
                            AppliedGeneralAverage = ks5Result.AppliedGeneralQualificationAverage,
                        },
                        National = new KeyStage5Result
                        {
                            AcademicAverage = ks5Result.NationalAcademicQualificationAverage,
                            AppliedGeneralAverage = ks5Result.NationalAppliedGeneralQualificationAverage
                        }
                    }
                ).ToList()
            };
        }
    }
}