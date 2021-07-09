using System.ComponentModel.DataAnnotations;

namespace Data.Models.KeyStagePerformance
{
    public class KeyStage2
    {
        public string Year { get; set; }
        public DisadvantagedPupilsResult PercentageMeetingExpectedStdInRWM { get; set; }
        public DisadvantagedPupilsResult PercentageAchievingHigherStdInRWM { get; set; }
        public DisadvantagedPupilsResult ReadingProgressScore { get; set; }
        public DisadvantagedPupilsResult WritingProgressScore { get; set; }
        public DisadvantagedPupilsResult MathsProgressScore { get; set; }
        public DisadvantagedPupilsResult NationalAveragePercentageMeetingExpectedStdInRWM { get; set; }
        public DisadvantagedPupilsResult NationalAveragePercentageAchievingHigherStdInRWM { get; set; }
        public DisadvantagedPupilsResult NationalAverageReadingProgressScore { get; set; }
        public DisadvantagedPupilsResult NationalAverageWritingProgressScore { get; set; }
        public DisadvantagedPupilsResult NationalAverageMathsProgressScore { get; set; }
        public DisadvantagedPupilsResult LAAveragePercentageMeetingExpectedStdInRWM { get; set; }
        public DisadvantagedPupilsResult LAAveragePercentageAchievingHigherStdInRWM { get; set; }
        public DisadvantagedPupilsResult LAAverageReadingProgressScore { get; set; }
        public DisadvantagedPupilsResult LAAverageWritingProgressScore { get; set; }
        public DisadvantagedPupilsResult LAAverageMathsProgressScore { get; set; }
    }
}