using System.Collections.Generic;
using API.Models.Upstream.Response;
using Data.Models;

namespace Frontend.Models.AcademyPerformance
{
    public class AcademyPerformanceViewModel
    {
        public GetProjectsResponseModel Project { get; set; }
        public Academy OutgoingAcademy { get; set; }

        public IEnumerable<FormField> FieldsToDisplay()
        {
            var performance = OutgoingAcademy.Performance;

            return new List<FormField>
            {
                new FormField {Title = "School phase", Value = performance.SchoolPhase},
                new FormField {Title = "Age range", Value = performance.AgeRange},
                new FormField {Title = "Capacity", Value = performance.Capacity},
                new FormField {Title = "NOR (% full)", Value = performance.Nor},
                new FormField {Title = "PAN", Value = performance.Pan},
                new FormField {Title = "PFI", Value = performance.Pfi},
                new FormField {Title = "Viability issue", Value = performance.ViabilityIssue},
                new FormField {Title = "Deficit", Value = performance.Deficit},
                new FormField {Title = "School type", Value = performance.SchoolType},
                new FormField {Title = "Dioceses % G or O", Value = performance.DiocesesPercent},
                new FormField {Title = "Distance to sponsor HQ", Value = performance.DistanceToSponsorHq},
                new FormField {Title = "MP (Party)", Value = performance.MpAndParty},
                new FormField {Title = "Ofsted judgement date", Value = performance.OfstedJudgementDate},
                new FormField {Title = "Current framework", Value = performance.CurrentFramework},
                new FormField {Title = "Achievement of pupil", Value = performance.AchievementOfPupil},
                new FormField {Title = "Quality of teaching", Value = performance.QualityOfTeaching},
                new FormField {Title = "Behaviour and safety of pupil", Value = performance.BehaviourAndSafetyOfPupil},
                new FormField {Title = "Leadership and management", Value = performance.LeadershipAndManagement},
            };
        }
    }
}