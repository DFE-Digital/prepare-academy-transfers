using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Frontend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Frontend.Pages.Home
{
    public class Login : CommonPageModel
    {
        private readonly IConfiguration _configuration;

        [BindProperty(SupportsGet = true)] public string ReturnUrl { get; set; }
        [BindProperty] public string Username { get; set; }
        [BindProperty] public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public Login(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var decodedUrl = "";

            if (Username != _configuration["username"] || Password != _configuration["password"])
            {
                ModelState.AddModelError("ErrorMessage", "Incorrect username and password");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Name")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authenticationProperties = new AuthenticationProperties();
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authenticationProperties);

            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                decodedUrl = WebUtility.UrlDecode(ReturnUrl);
            }

            if (Url.IsLocalUrl(decodedUrl))
            {
                return Redirect(ReturnUrl);
            }

            return RedirectToPage("/Home/Index");
        }
        
        public async Task<IActionResult> OnGetSignOut()
        {
            await HttpContext.SignOutAsync();
            TempData["Success.Message"] = "Successfully signed out";
            return RedirectToPage("/Home/Login");
        }
    }
}