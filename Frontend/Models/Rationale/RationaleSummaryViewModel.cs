using Frontend.Models.Forms;

namespace Frontend.Models.Rationale
{
    public class RationaleSummaryViewModel : CommonViewModel
    {
        public string OutgoingAcademyUrn { get; set; }
        public string ProjectRationale { get; set; }
        public string TrustRationale { get; set; }
    }
}