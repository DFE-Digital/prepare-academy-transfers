using System;
using System.Collections.Generic;
using Data.Models;
using Frontend.Models.Forms;

namespace Frontend.Models
{
    public class GeneralInformationViewModel : CommonViewModel
    {
        public IEnumerable<FormFieldViewModel> NameValues { get; set; }
        public AdditionalInformationViewModel AdditionalInformationModel { get; set; }
        public object OutgoingAcademyUrn { get; set; }
    }
}