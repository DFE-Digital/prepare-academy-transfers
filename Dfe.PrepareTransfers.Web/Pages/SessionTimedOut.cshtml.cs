using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Dfe.PrepareTransfers.Web.Pages
{
    public class SessionTimedOut : PageModel
    {
        public SessionTimedOut(IConfiguration configuration)
        {
            MinutesTimeOut = int.Parse(configuration["AuthenticationExpirationInMinutes"]);
        }
        [FromQuery]
        public string ReturnUrl { get; set; }
        public int MinutesTimeOut { get; }
        public IActionResult OnGet()
        {
            return Page();
        }
        
     
    }
}