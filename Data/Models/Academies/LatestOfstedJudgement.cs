using System.Collections.Generic;

namespace Data.Models.Academies
{
    public class LatestOfstedJudgement
    {
        public string SchoolName { get; set; }
        public string InspectionDate { get; set; }
        public string Effectiveness { get; set; }
        public string AchievementOfPupils { get; set; }
        public string QualityOfTeaching { get; set; }
        public string BehaviourAndSafetyOfPupils { get; set; }
        public string LeadershipAndManagement { get; set; }
        public string EarlyYearsProvision { get; set; }

        public IEnumerable<FormField> FieldsToDisplay()
        {
            return new List<FormField>
            {
                new FormField {Title = "School name", Value = SchoolName},
                new FormField {Title = "Ofsted inspection date", Value = InspectionDate},
                new FormField {Title = "Overall effectiveness", Value = Effectiveness},
                new FormField {Title = "Achievement of pupils", Value = AchievementOfPupils},
                new FormField {Title = "Quality of teaching", Value = QualityOfTeaching},
                new FormField {Title = "Behaviour and safety of pupils", Value = BehaviourAndSafetyOfPupils},
                new FormField {Title = "Leadership & Management", Value = LeadershipAndManagement},
                new FormField {Title = "Early years provision", Value = EarlyYearsProvision}
            };
        }
    }
}