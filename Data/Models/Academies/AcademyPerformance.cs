using System.Collections.Generic;

namespace Data.Models.Academies
{
    public class AcademyPerformance
    {
        public string SchoolPhase { get; set; }
        public string AgeRange { get; set; }
        public string Capacity { get; set; }
        public string Nor { get; set; }
        public string Pan { get; set; }
        public string Pfi { get; set; }
        public string ViabilityIssue { get; set; }
        public string Deficit { get; set; }
        public string SchoolType { get; set; }
        public string DiocesesPercent { get; set; }
        public string DistanceToSponsorHq { get; set; }
        public string MpAndParty { get; set; }
        public string OfstedJudgementDate { get; set; }
        public string AchievementOfPupil { get; set; }
        public string QualityOfTeaching { get; set; }
        public string BehaviourAndSafetyOfPupil { get; set; }
        public string LeadershipAndManagement { get; set; }

        public IEnumerable<FormField> FieldsToDisplay()
        {
            return new List<FormField>
            {
                new FormField {Title = "School phase", Value = SchoolPhase},
                new FormField {Title = "Age range", Value = AgeRange},
                new FormField {Title = "Capacity", Value = Capacity},
                new FormField {Title = "NOR (% full)", Value = Nor},
                new FormField {Title = "PAN", Value = Pan},
                new FormField {Title = "PFI", Value = Pfi},
                new FormField {Title = "Viability issue", Value = ViabilityIssue},
                new FormField {Title = "Deficit", Value = Deficit},
                new FormField {Title = "School type", Value = SchoolType},
                new FormField {Title = "Dioceses % G or O", Value = DiocesesPercent},
                new FormField {Title = "Distance to sponsor HQ", Value = DistanceToSponsorHq},
                new FormField {Title = "MP (Party)", Value = MpAndParty},
                new FormField {Title = "Ofsted judgement date", Value = OfstedJudgementDate},
                new FormField {Title = "Achievement of pupil", Value = AchievementOfPupil},
                new FormField {Title = "Quality of teaching", Value = QualityOfTeaching},
                new FormField {Title = "Behaviour and safety of pupil", Value = BehaviourAndSafetyOfPupil},
                new FormField {Title = "Leadership and management", Value = LeadershipAndManagement},
            };
        }
    }
}