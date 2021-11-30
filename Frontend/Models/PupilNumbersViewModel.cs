using System.Collections.Generic;
using Data.Models;
using Frontend.Models.Forms;

namespace Frontend.Models
{
    //todo: remove project academy models etc
    public class PupilNumbersViewModel
    {
        public Project Project { get; set; }
        public Academy OutgoingAcademy { get; set; }
        public AdditionalInformationViewModel AdditionalInformationModel { get; set; }
        public bool ReturnToPreview { get; set; }
        
        public IEnumerable<FormFieldViewModel> FieldsToDisplay()
        {
            return new List<FormFieldViewModel>
            {
                new FormFieldViewModel {Title = "Girls on roll", Value = OutgoingAcademy.PupilNumbers.GirlsOnRoll,},
                new FormFieldViewModel {Title = "Boys on roll", Value =  OutgoingAcademy.PupilNumbers.BoysOnRoll,},
                new FormFieldViewModel
                    {Title = "Pupils with a statement of special educational needs (SEN)", Value =  OutgoingAcademy.PupilNumbers.WithStatementOfSen},
                new FormFieldViewModel
                    {Title = "Pupils with English as an additional language (EAL)", Value =  OutgoingAcademy.PupilNumbers.WhoseFirstLanguageIsNotEnglish},
                new FormFieldViewModel
                {
                    Title = "Pupils eligible for free school meals during the past 6 years",
                    Value =  OutgoingAcademy.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years
                }
            };
        }
    }
}