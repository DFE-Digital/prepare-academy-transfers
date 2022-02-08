using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Models
{
    public abstract class CommonPageModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Urn { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ReturnToPreview { get; set; }
        [BindProperty]
        public string OutgoingAcademyName { get; set; }
        public string OutgoingAcademyUrn { get; set; }
        [BindProperty]
        public string ProjectReference { get; set; }
        [BindProperty]
        public string IncomingTrustName { get; set; }
    }
}