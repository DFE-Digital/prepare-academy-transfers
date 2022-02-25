using System.Collections.Generic;
using System.Dynamic;

namespace Data.Models.KeyStagePerformance
{
    public class EducationPerformance
    {
        //todo: remove this when view models/view component
        #region remove
        public string ProjectUrn { get;set; }
        public string AcademyName { get; set; }
        public string AcademyUkprn { get; set; }
        #endregion
        public List<KeyStage2> KeyStage2Performance { get; set; }
        public string KeyStage2AdditionalInformation { get; set; }
        public List<KeyStage4> KeyStage4Performance { get; set; }
        public string KeyStage4AdditionalInformation { get; set; }
        public List<KeyStage5> KeyStage5Performance { get; set; }
        public string KeyStage5AdditionalInformation { get; set; }
        public string LocalAuthorityName { get; set; }
    }
}