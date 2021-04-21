using System.Collections.Generic;
using API.Models.Upstream.Response;
using Data.Models;

namespace Frontend.Models
{
    public class LatestOfstedJudgementViewModel
    {
        public GetProjectsResponseModel Project { get; set; }
        public Academy Academy { get; set; }

        public IEnumerable<FormField> FieldsToDisplay()
        {
            var judgement = Academy.LatestOfstedJudgement;

            return new List<FormField>
            {
                new FormField {Title = "School name", Value = judgement.SchoolName},
                new FormField {Title = "Ofsted inspection date", Value = judgement.InspectionDate},
                new FormField {Title = "Overall effectiveness", Value = judgement.Effectiveness},
                new FormField {Title = "Achievement of pupils", Value = judgement.AchievementOfPupils},
                new FormField {Title = "Quality of teaching", Value = judgement.QualityOfTeaching},
                new FormField {Title = "Behaviour and safety of pupils", Value = judgement.BehaviourAndSafetyOfPupils},
                new FormField {Title = "Leadership & Management", Value = judgement.LeadershipAndManagement},
                new FormField {Title = "Early years provision", Value = judgement.EarlyYearsProvision}
            };
        }
    }
}