using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Transfers
{
    public class IncomingTrustModel : PageModel
    {
        [BindProperty(Name = "query", SupportsGet = true)]
        public string SearchQuery { get; set; }

        public IActionResult OnGet(bool change = false)
        {
            ViewData["ChangeLink"] = change;

            // We redirect here with any error messages from the subsequent
            // steps.
            if (TempData.Peek("ErrorMessage") != null)
            {
                ModelState.AddModelError(nameof(SearchQuery), (string)TempData["ErrorMessage"]);
            }

            return Page();
        }
    }
}
