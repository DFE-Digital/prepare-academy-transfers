using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Models
{
    public abstract class CommonPageModel : PageModel
    {
        [BindProperty]
        public string Urn { get; set; }
        [BindProperty]
        public bool ReturnToPreview { get; set; }
        [BindProperty]
        public string OutgoingAcademyName { get; set; }
    }
}