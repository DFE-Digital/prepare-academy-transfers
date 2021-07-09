using System.Collections.Generic;
using System.Linq;
using Data.Models.KeyStagePerformance;
using Data.TRAMS.Mappers.Response;
using Data.TRAMS.Models.EducationPerformance;
using Data.TRAMS.Tests.TestFixtures;
using Xunit;
using EducationPerformance = Data.TRAMS.Tests.TestFixtures.EducationPerformance;
using KeyStage2 = Data.TRAMS.Models.EducationPerformance.KeyStage2;

namespace Data.TRAMS.Tests.Mappers.Response
{
    public class TramsEducationPerformanceMapperTests
    {
        private readonly TramsEducationPerformanceMapper _subject = new TramsEducationPerformanceMapper();

        [Fact]
        public void GivenTramsEducationPerformance_MapsToInternalEducationPerformanceSuccessfully()
        {
            var tramsEducationPerformanceToMap = EducationPerformance.GetSingleTramsEducationPerformance();

            var mappedInternalEducationPerformance = _subject.Map(tramsEducationPerformanceToMap);
            var mappedKs2Result = mappedInternalEducationPerformance.KeyStage2Performance.First();
            var tramsKs2Result = tramsEducationPerformanceToMap.KeyStage2.First();
            
            Assert.Equal(tramsKs2Result.Year, mappedKs2Result.Year);
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.MathsProgressScore, mappedKs2Result.MathsProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.ReadingProgressScore, mappedKs2Result.ReadingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.WritingProgressScore, mappedKs2Result.WritingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAverageMathsProgressScore, mappedKs2Result.NationalAverageMathsProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAverageReadingProgressScore, mappedKs2Result.NationalAverageReadingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.LAAverageMathsProgressScore, mappedKs2Result.LAAverageMathsProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.LAAverageReadingProgressScore, mappedKs2Result.LAAverageReadingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.LAAverageWritingProgressScore, mappedKs2Result.LAAverageWritingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.LAAveragePercentageAchievingHigherStdInRWM, mappedKs2Result.LAAveragePercentageAchievingHigherStdInRWM));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.LAAveragePercentageMeetingExpectedStdInRWM, mappedKs2Result.LAAveragePercentageMeetingExpectedStdInRWM));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAverageMathsProgressScore, mappedKs2Result.NationalAverageMathsProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAverageReadingProgressScore, mappedKs2Result.NationalAverageReadingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAverageWritingProgressScore, mappedKs2Result.NationalAverageWritingProgressScore));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAveragePercentageAchievingHigherStdInRWM, mappedKs2Result.NationalAveragePercentageAchievingHigherStdInRWM));
            Assert.True(AssertPupilCategoryResponse(tramsKs2Result.NationalAveragePercentageMeetingExpectedStdInRWM, mappedKs2Result.NationalAveragePercentageMeetingExpectedStdInRWM));
        }

        private bool AssertPupilCategoryResponse(DisadvantagedPupilsResponse tramsResponse,
            DisadvantagedPupilsResult mappedResult) =>
            tramsResponse.Disadvantaged == mappedResult.Disadvantaged &&
            tramsResponse.NotDisadvantaged == mappedResult.NotDisadvantaged;
    }
}