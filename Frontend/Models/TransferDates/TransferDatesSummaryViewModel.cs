using Frontend.Models.Forms;

namespace Frontend.Models.TransferDates
{
    public class TransferDatesSummaryViewModel : CommonViewModel
    {
        public string OutgoingAcademyUrn { get; set; }
        public string FirstDiscussedDate { get; set; } 
        public bool? HasFirstDiscussedDate { get; set; } 
        public string HtbDate { get; set; } 
        public bool? HasHtbDate { get; set; } 
        public string TargetDate { get; set; } 
        public bool? HasTargetDate { get; set; } 
    }
}