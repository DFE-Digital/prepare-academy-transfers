using Frontend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;

namespace Frontend.Pages
{
    public class CookiePreferencesModel : PageModel
    {
        private const string ConsentCookieName = ".ManageAnAcademyTransfer.Consent";
        public bool? Consent { get; set; }
        public bool PreferencesSet { get; set; } = false;
        public string returnPath { get; set; }
        private readonly ILogger<CookiePreferencesModel> _logger;

        public CookiePreferencesModel(ILogger<CookiePreferencesModel> logger)
        {
            _logger = logger;
        }

        public ActionResult OnGet(bool? consent, string returnUrl)
        {
            returnPath = returnUrl;

            if (Request.Cookies.ContainsKey(ConsentCookieName))
            {
                Consent = bool.Parse(Request.Cookies[ConsentCookieName]);
            }

            if (consent.HasValue)
            {
                PreferencesSet = true;

                ApplyCookieConsent(consent);

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToPage(Links.CookiePreferences);
            }

            return Page();
        }

        public IActionResult OnPost(bool? consent, string returnUrl)
        {
            returnPath = returnUrl;

            if (Request.Cookies.ContainsKey(ConsentCookieName))
            {
                Consent = bool.Parse(Request.Cookies[ConsentCookieName]);
            }

            if (consent.HasValue)
            {
                Consent = consent;
                PreferencesSet = true;

                var cookieOptions = new CookieOptions { Expires = DateTime.Today.AddMonths(6), Secure = true };
                Response.Cookies.Append(ConsentCookieName, consent.Value.ToString(), cookieOptions);

                if (!consent.Value)
                {
                    ApplyCookieConsent(consent);
                }
                return Page();
            }

            return Page();
        }

        private void ApplyCookieConsent(bool? consent)
        {
            if (consent.HasValue)
            {
                var cookieOptions = new CookieOptions { Expires = DateTime.Today.AddMonths(6), Secure = true };
                Response.Cookies.Append(ConsentCookieName, consent.Value.ToString(), cookieOptions);
            }

            if (!consent.Value)
            {
                foreach (var cookie in Request.Cookies.Keys)
                {
                    if (cookie.StartsWith("_ga") || cookie.Equals("_gid"))
                    {
                        _logger.LogInformation($"deleting Google analytics cookie: {cookie}");
                        Response.Cookies.Delete(cookie);
                    }
                }
            }
        }
    }
}