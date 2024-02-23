using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.PrepareTransfers.Web.Models
{
    public abstract class CommonPageModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Urn { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ReturnToPreview { get; set; }
        public string OutgoingAcademyUrn { get; set; }
        [BindProperty]
        public string ProjectReference { get; set; }
        [BindProperty]
        public string IncomingTrustName { get; set; }
        public bool IsFormAMAT { get; set; } = true;
    }
}