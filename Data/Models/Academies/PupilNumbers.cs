using System.Collections.Generic;

namespace Data.Models.Academies
{
    public class PupilNumbers
    {
        public string GirlsOnRoll { get; set; }
        public string BoysOnRoll { get; set; }
        public string WithStatementOfSen { get; set; }
        public string WhoseFirstLanguageIsNotEnglish { get; set; }
        public string PercentageEligibleForFreeSchoolMealsDuringLast6Years { get; set; }

        public IEnumerable<FormField> FieldsToDisplay()
        {
            return new List<FormField>
            {
                new FormField {Title = "Girls on roll", Value = GirlsOnRoll,},
                new FormField {Title = "Boys on roll", Value = BoysOnRoll,},
                new FormField
                    {Title = "Pupil with a statement of special educational needs (SEN)", Value = WithStatementOfSen},
                new FormField
                    {Title = "Pupil whose first language is not English", Value = WhoseFirstLanguageIsNotEnglish},
                new FormField
                {
                    Title = "Pupil eligible for free school meal during the past 6 years",
                    Value = PercentageEligibleForFreeSchoolMealsDuringLast6Years
                }
            };
        }
    }
}