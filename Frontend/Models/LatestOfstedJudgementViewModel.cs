using Data.Models;
using Frontend.Models.Forms;

namespace Frontend.Models
{
    public class LatestOfstedJudgementViewModel
    {
        public Project Project { get; set; }
        public Academy Academy { get; set; }
        public AdditionalInformationViewModel AdditionalInformationModel { get; set; }
        public bool ReturnToPreview { get; set; }
    }
}