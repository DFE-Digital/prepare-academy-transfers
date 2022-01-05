using Frontend.Models;
using Frontend.Models.Forms;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Projects.LatestOfstedJudgement
{
    public class Index : CommonPageModel
    {
        public string SchoolName { get; set; }
        public string InspectionDate { get; set; }
        public string OverallEffectiveness { get; set; }
        public string OfstedReport { get; set; }
        public AdditionalInformationViewModel AdditionalInformation { get; set; }
        public bool IsPreview { get; set; }
        
        public void OnGet()
        {
            
        }
    }
}