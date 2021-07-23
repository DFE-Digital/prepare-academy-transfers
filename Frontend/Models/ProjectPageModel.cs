using Data.Models;
using Data.Models.KeyStagePerformance;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Models
{
    public class ProjectPageModel : PageModel
    {
        public EducationPerformance EducationPerformance { get; set; }
        public Trust OutgoingTrust { get; set; }
        public Project Project { get; set; }
        public Academy TransferringAcademy { get; set; }
    }
}