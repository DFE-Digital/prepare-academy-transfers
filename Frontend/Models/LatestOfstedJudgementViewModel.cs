using System.ComponentModel;
using Data.Models;
using Frontend.Models.Forms;

namespace Frontend.Models
{
    public class LatestOfstedJudgementViewModel : CommonViewModel
    {
        public string SchoolName { get; set; }
        public string InspectionDate { get; set; }
        public string OverallEffectiveness { get; set; }
        public string OfstedReport { get; set; }
        public AdditionalInformationViewModel AdditionalInformation { get; set; }
        public string OutgoingAcademyUrn { get; set; }
        public bool IsPreview { get; set; }
    }
}