using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Frontend.Pages
{
    public class SessionTimedOut : PageModel
    {
        public SessionTimedOut(IConfiguration configuration)
        {
            MinutesTimeOut = configuration["AuthenticationExpirationInMinutes"];
        }
        [FromQuery]
        public string ReturnUrl { get; set; }
        public string MinutesTimeOut { get; }
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}