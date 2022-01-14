using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Frontend.Models;
using Frontend.Models.TransferDates;
using Frontend.Validators;
using Frontend.Validators.TransferDates;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Configuration;

namespace Frontend.Pages.Home
{
    public class Login : CommonPageModel
    {
        private readonly IConfiguration _configuration;

        [BindProperty(SupportsGet = true)] 
        public string ReturnUrl { get; set; }
        [BindProperty]
        
        [CustomizeValidator(Skip = true)]
        public LoginViewModel LoginViewModel { get; set; }

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
            
            var validationResult = await ValidateUsernameAndPassword();

            if (!validationResult.IsValid)
            {
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

        private async Task<ValidationResult> ValidateUsernameAndPassword()
        {
            var validationContext = new ValidationContext<LoginViewModel>(LoginViewModel)
            {
                RootContextData =
                {
                    ["ConfigUsername"] = _configuration["username"],
                    ["ConfigPassword"] = _configuration["password"]
                }
            };
            var validator = new LoginValidator();
            var validationResult = await validator.ValidateAsync(validationContext);
            validationResult.AddToModelState(ModelState, null);
            return validationResult;
        }

        public async Task<IActionResult> OnGetSignOut()
        {
            await HttpContext.SignOutAsync();
            TempData["Success.Message"] = "Successfully signed out";
            return RedirectToPage("/Home/Login");
        }
    }
}