using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Models
{
    public abstract class CommonPageModel : PageModel
    {
        public string Urn { get; set; }
        public bool ReturnToPreview { get; set; }
        public string OutgoingAcademyName { get; set; }
    }
}