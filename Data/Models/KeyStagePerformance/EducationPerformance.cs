using System.Collections.Generic;

namespace Data.Models.KeyStagePerformance
{
    public class EducationPerformance
    {
        public List<KeyStage2> KeyStage2Performance { get; set; }
        public List<KeyStage4> KeyStage4Performance { get; set; }
        public List<KeyStage5> KeyStage5Performance { get; set; }
    }
}