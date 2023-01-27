using System.Linq;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.Data.TRAMS.Mappers.Response;
using Dfe.PrepareTransfers.Data.TRAMS.Models.EducationPerformance;
using Xunit;
using EducationPerformance = Dfe.PrepareTransfers.Data.TRAMS.Tests.TestFixtures.EducationPerformance;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests.Mappers.Response
{
    public class TramsEducationPerformanceMapperTests
    {
        private readonly TramsEducationPerformanceMapper _subject = new TramsEducationPerformanceMapper();

        [Fact]
        public void GivenTramsEducationPerformanceKeyStage2Results_MapsToInternalEducationPerformance()
        {
            var tramsEducationPerformanceToMap = EducationPerformance.GetSingleTramsEducationPerformance();

            var mappedInternalEducationPerformance = _subject.Map(tramsEducationPerformanceToMap);
            AssertKs2MappedCorrectly(mappedInternalEducationPerformance, tramsEducationPerformanceToMap);
            var mappedKs5Results = mappedInternalEducationPerformance.KeyStage5Performance;
            var tramsKs5Results = tramsEducationPerformanceToMap.KeyStage5;

            Assert.Equal(tramsKs5Results[0].Year, mappedKs5Results[0].Year);
            Assert.Equal(tramsKs5Results[0].AcademicQualificationAverage, mappedKs5Results[0].Academy.AcademicAverage);
            Assert.Equal(tramsKs5Results[0].AppliedGeneralQualificationAverage,
                mappedKs5Results[0].Academy.AppliedGeneralAverage);
            Assert.Equal(tramsKs5Results[0].NationalAcademicQualificationAverage,
                mappedKs5Results[0].National.AcademicAverage);
            Assert.Equal(tramsKs5Results[0].NationalAppliedGeneralQualificationAverage,
                mappedKs5Results[0].National.AppliedGeneralAverage);
            Assert.Equal(tramsKs5Results[1].Year, mappedKs5Results[1].Year);
            Assert.Equal(tramsKs5Results[1].AcademicQualificationAverage, mappedKs5Results[1].Academy.AcademicAverage);
            Assert.Equal(tramsKs5Results[1].AppliedGeneralQualificationAverage,
                mappedKs5Results[1].Academy.AppliedGeneralAverage);
            Assert.Equal(tramsKs5Results[1].NationalAcademicQualificationAverage,
                mappedKs5Results[1].National.AcademicAverage);
            Assert.Equal(tramsKs5Results[1].NationalAppliedGeneralQualificationAverage,
                mappedKs5Results[1].National.AppliedGeneralAverage);
        }

        private void AssertKs2MappedCorrectly(
            Dfe.PrepareTransfers.Data.Models.KeyStagePerformance.EducationPerformance mappedInternalEducationPerformance,
            TramsEducationPerformance tramsEducationPerformanceToMap)
        {
            var mappedKs2Result = mappedInternalEducationPerformance.KeyStage2Performance.First();
            var tramsKs2Result = tramsEducationPerformanceToMap.KeyStage2.First();

            Assert.Equal(tramsKs2Result.Year, mappedKs2Result.Year);
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.MathsProgressScore,
                mappedKs2Result.MathsProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.ReadingProgressScore,
                mappedKs2Result.ReadingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.WritingProgressScore,
                mappedKs2Result.WritingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAverageMathsProgressScore,
                mappedKs2Result.NationalAverageMathsProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAverageReadingProgressScore,
                mappedKs2Result.NationalAverageReadingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.LAAverageMathsProgressScore,
                mappedKs2Result.LAAverageMathsProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.LAAverageReadingProgressScore,
                mappedKs2Result.LAAverageReadingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.LAAverageWritingProgressScore,
                mappedKs2Result.LAAverageWritingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.LAAveragePercentageAchievingHigherStdInRWM,
                mappedKs2Result.LAAveragePercentageAchievingHigherStdInRWM));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.LAAveragePercentageMeetingExpectedStdInRWM,
                mappedKs2Result.LAAveragePercentageMeetingExpectedStdInRWM));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAverageMathsProgressScore,
                mappedKs2Result.NationalAverageMathsProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAverageReadingProgressScore,
                mappedKs2Result.NationalAverageReadingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAverageWritingProgressScore,
                mappedKs2Result.NationalAverageWritingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAveragePercentageAchievingHigherStdInRWM,
                mappedKs2Result.NationalAveragePercentageAchievingHigherStdInRWM));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAveragePercentageMeetingExpectedStdInRWM,
                mappedKs2Result.NationalAveragePercentageMeetingExpectedStdInRWM));
        }
        
        [Fact]
        public void GivenTramsEducationPerformanceKeyStage4Results_MapsToInternalEducationPerformance()
        {
            var tramsEducationPerformanceToMap = EducationPerformance.GetSingleTramsEducationPerformance();

            var mappedInternalEducationPerformance = _subject.Map(tramsEducationPerformanceToMap);
            var mappedKs4Result = mappedInternalEducationPerformance.KeyStage4Performance.First();
            var tramsKs4Result = tramsEducationPerformanceToMap.KeyStage4.First();

            Assert.Equal(tramsKs4Result.Year, mappedKs4Result.Year);
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.SipAttainment8score, mappedKs4Result.SipAttainment8score));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.SipAttainment8scoreebacc, mappedKs4Result.SipAttainment8scoreebacc));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.SipAttainment8scoreenglish, mappedKs4Result.SipAttainment8scoreenglish));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.SipAttainment8scoremaths, mappedKs4Result.SipAttainment8scoremaths));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.SipNumberofpupilsprogress8, mappedKs4Result.SipNumberofpupilsprogress8));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.SipProgress8ebacc, mappedKs4Result.SipProgress8ebacc));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.SipProgress8english, mappedKs4Result.SipProgress8english));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.SipProgress8maths, mappedKs4Result.SipProgress8maths));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.SipProgress8Score, mappedKs4Result.SipProgress8Score));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.NationalAverageA8English, mappedKs4Result.NationalAverageA8English));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.NationalAverageA8Maths, mappedKs4Result.NationalAverageA8Maths));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.NationalAverageA8Score, mappedKs4Result.NationalAverageA8Score));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.NationalAverageP8Ebacc, mappedKs4Result.NationalAverageP8Ebacc));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.NationalAverageP8English, mappedKs4Result.NationalAverageP8English));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.NationalAverageP8Maths, mappedKs4Result.NationalAverageP8Maths));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.NationalAverageP8Score, mappedKs4Result.NationalAverageP8Score));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.LAAverageA8English, mappedKs4Result.LAAverageA8English));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.LAAverageA8Maths, mappedKs4Result.LAAverageA8Maths));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.LAAverageA8Score, mappedKs4Result.LAAverageA8Score));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.LAAverageP8Ebacc, mappedKs4Result.LAAverageP8Ebacc));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.LAAverageP8English, mappedKs4Result.LAAverageP8English));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.LAAverageP8Maths, mappedKs4Result.LAAverageP8Maths));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.LAAverageP8Score, mappedKs4Result.LAAverageP8Score));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.NationalAverageA8EBacc, mappedKs4Result.NationalAverageA8EBacc));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.NationalAverageP8PupilsIncluded, mappedKs4Result.NationalAverageP8PupilsIncluded));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.LAAverageA8EBacc, mappedKs4Result.LAAverageA8EBacc));
            Assert.True(AssertPupilCategoryResponse(tramsKs4Result.LAAverageP8PupilsIncluded, mappedKs4Result.LAAverageP8PupilsIncluded));
            Assert.Equal(tramsKs4Result.SipProgress8lowerconfidence, mappedKs4Result.SipProgress8lowerconfidence);
            Assert.Equal(tramsKs4Result.SipProgress8upperconfidence, mappedKs4Result.SipProgress8upperconfidence);
            Assert.Equal(tramsKs4Result.NationalAverageP8LowerConfidence, mappedKs4Result.NationalAverageP8LowerConfidence);
            Assert.Equal(tramsKs4Result.NationalAverageP8UpperConfidence, mappedKs4Result.NationalAverageP8UpperConfidence);
            Assert.Equal(tramsKs4Result.LAAverageP8LowerConfidence, mappedKs4Result.LAAverageP8LowerConfidence);
            Assert.Equal(tramsKs4Result.LAAverageP8UpperConfidence, mappedKs4Result.LAAverageP8UpperConfidence);
            Assert.Equal(tramsKs4Result.Enteringebacc, mappedKs4Result.Enteringebacc);
            Assert.Equal(tramsKs4Result.LAEnteringEbacc, mappedKs4Result.LAEnteringEbacc);
            Assert.Equal(tramsKs4Result.NationalEnteringEbacc, mappedKs4Result.NationalEnteringEbacc);
        }

        private bool AssertPupilCategoryResponse(DisadvantagedPupilsResponse tramsResponse,
            DisadvantagedPupilsResult mappedResult) =>
            tramsResponse.Disadvantaged == mappedResult.Disadvantaged &&
            tramsResponse.NotDisadvantaged == mappedResult.NotDisadvantaged;
    }
}