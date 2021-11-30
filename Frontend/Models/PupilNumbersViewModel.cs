using System.Collections.Generic;
using Data.Models;
using Frontend.Models.Forms;

namespace Frontend.Models
{
    //todo: remove project academy models etc
    public class PupilNumbersViewModel : CommonViewModel
    {
        public string GirlsOnRoll { get; set; }
        public string BoysOnRoll { get; set; }
        public string WithStatementOfSEN { get; set; }
        public string WithEAL { get; set; }
        public string FreeSchoolMealsLast6Years { get; set; }
        public AdditionalInformationViewModel AdditionalInformation { get; set; }
        public string OutgoingAcademyUrn { get; set; }
    }
}