using System.Collections.Generic;
using System.Linq;
using Data.Models.Projects;
using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models.AcademyAndTrustInformation
{
    public class AcademyAndTrustInformationSummaryViewModel : CommonViewModel
    {
        public string OutgoingAcademyUrn { get; set; }
        public TransferAcademyAndTrustInformation.RecommendationResult Recommendation { get; set; }
        public string Author { get; set; }
        public string ProjectName { get; set; }
        public string HtbDate { get; set; }
        public string IncomingTrustName { get; set; }
        public string TargetDate { get; set; }
        public string FirstDiscussedDate { get; set; }
    }
}