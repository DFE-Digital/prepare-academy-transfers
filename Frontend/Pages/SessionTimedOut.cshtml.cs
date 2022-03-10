using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class SessionTimedOut : PageModel
    {
        public string ReturnUrl { get; set; }

        public IActionResult OnGet()
        {
            Request.Query.TryGetValue("returnurl", out var value);
            ReturnUrl = value;
            return Page();
        }
    }
}