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
       [BindProperty(SupportsGet = true)] public string ReturnUrl { get; set; }

       public Login(IConfiguration configuration)
        {
         
        }

        public IActionResult OnGet()
        {
            var decodedUrl = "";
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
            return RedirectToPage("/sessiontimedout");
        }
    }
}