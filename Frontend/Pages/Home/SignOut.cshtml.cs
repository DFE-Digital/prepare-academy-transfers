using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;

namespace Frontend.Pages.Home
{
    [AllowAnonymous]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class SignOut : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            // We redirect back to this handler once signed out of Azure AD. At that point
            // we want to show the user a signed out page.
            if (!User.Identity.IsAuthenticated)
            {
                return Page();
            }

            // SignOutAsync with a redirect overwrites the redirect to the OIDC endsession URL that SignOutAsync tries to issue.
            // The solution is to call it twice, once to delete the session cookie, then again to redirect.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignOutAsync(new AuthenticationProperties { RedirectUri = Url.Page("/Home/SignOut") });

            return null;
        }
    }
}