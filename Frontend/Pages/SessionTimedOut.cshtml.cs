using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Frontend.Pages
{
    public class SessionTimedOut : PageModel
    {
        private readonly IConfiguration _configuration;

        public SessionTimedOut(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string ReturnUrl { get; set; }
        public string MinutesTimeOut { get; set; }
        public IActionResult OnGet()
        {
            Request.Query.TryGetValue("returnurl", out var value);
            ReturnUrl = value;
            MinutesTimeOut = _configuration["AuthenticationExpirationInMinutes"];
            return Page();
        }
    }
}