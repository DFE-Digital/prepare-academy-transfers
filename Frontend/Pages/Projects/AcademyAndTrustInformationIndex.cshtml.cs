using Data.Models.Projects;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Projects
{
    public class AcademyAndTrustInformationIndex : PageModel
    {
        public string OutgoingAcademyUrn { get; set; }
        public TransferAcademyAndTrustInformation.RecommendationResult Recommendation { get; set; }
        public string Author { get; set; }
        public string ProjectName { get; set; }
        public string HtbDate { get; set; }
        public string IncomingTrustName { get; set; }
        public string TargetDate { get; set; }
        public string FirstDiscussedDate { get; set; }
        
        public void OnGet()
        {
            
        }
    }
}