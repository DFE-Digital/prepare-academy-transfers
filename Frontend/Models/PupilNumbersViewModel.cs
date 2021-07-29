using Data.Models;
using Frontend.Models.Forms;

namespace Frontend.Models
{
    public class PupilNumbersViewModel
    {
        public Project Project { get; set; }
        public Academy OutgoingAcademy { get; set; }
        public AdditionalInformationViewModel AdditionalInformationModel { get; set; }
        public bool ReturnToPreview { get; set; }
    }
}