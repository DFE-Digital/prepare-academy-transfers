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
                    {Title = "Pupils with a statement of special educational needs (SEN)", Value = WithStatementOfSen},
                new FormField
                    {Title = "Pupils with English as an additional language (EAL)", Value = WhoseFirstLanguageIsNotEnglish},
                new FormField
                {
                    Title = "Pupils eligible for free school meals during the past 6 years",
                    Value = PercentageEligibleForFreeSchoolMealsDuringLast6Years
                }
            };
        }
    }
}