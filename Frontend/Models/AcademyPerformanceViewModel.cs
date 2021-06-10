using Data.Models;
using Frontend.Models.Forms;

namespace Frontend.Models
{
    public class AcademyPerformanceViewModel
    {
        public Project Project { get; set; }
        public Academy OutgoingAcademy { get; set; }
        public AdditionalInformationViewModel AdditionalInformationModel { get; set; }
    }
}