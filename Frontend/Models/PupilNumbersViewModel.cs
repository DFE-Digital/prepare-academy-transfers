using System.Collections.Generic;
using API.Models.Upstream.Response;
using Data.Models;

namespace Frontend.Models
{
    public class PupilNumbersViewModel
    {
        public GetProjectsResponseModel Project { get; set; }
        public Academy OutgoingAcademy { get; set; }

        public IEnumerable<FormField> FieldsToDisplay()
        {
            var pupilNumbers = OutgoingAcademy.PupilNumbers;

            return new List<FormField>
            {
                new FormField {Title = "Girls on roll", Value = pupilNumbers.GirlsOnRoll,},
                new FormField {Title = "Boys on roll", Value = pupilNumbers.BoysOnRoll,},
                new FormField
                {
                    Title = "Pupil with a statement of special educational needs (SEN)",
                    Value = pupilNumbers.WithStatementOfSen
                },
                new FormField
                {
                    Title = "Pupil whose first language is not English",
                    Value = pupilNumbers.WhoseFirstLanguageIsNotEnglish
                },
                new FormField
                {
                    Title = "Pupil eligible for free school meal during the past 6 years",
                    Value = pupilNumbers.EligibleForFreeSchoolMeals
                }
            };
        }
    }
}