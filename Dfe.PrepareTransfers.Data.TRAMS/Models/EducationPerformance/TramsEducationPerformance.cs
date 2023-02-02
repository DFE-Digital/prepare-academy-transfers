using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Data.TRAMS.Models.EducationPerformance
{
    public class TramsEducationPerformance
    {
        public List<KeyStage2> KeyStage2 { get; set; }
        public List<KeyStage4> KeyStage4 { get; set; }
        public List<KeyStage5> KeyStage5 { get; set; }
    }
}