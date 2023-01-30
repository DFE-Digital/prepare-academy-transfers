using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Dfe.PrepareTransfers.Web.Pages
{
    public class CookiePreferencesModel : PageModel
    {
        private const string ConsentCookieName = ".ManageAnAcademyTransfer.Consent";
        public bool? Consent { get; set; }
        public bool PreferencesSet { get; set; } = false;
        public string returnPath { get; set; }
        private readonly ILogger<CookiePreferencesModel> _logger;
        private readonly IOptions<ServiceLinkOptions> _options;

        public CookiePreferencesModel(ILogger<CookiePreferencesModel> logger, IOptions<ServiceLinkOptions> options)
        {
            _logger = logger;
            _options = options;
        }

        public string ConversionsCookieUrl { get; set; }

        public ActionResult OnGet(bool? consent, string returnUrl)
        {
            returnPath = returnUrl;
            ConversionsCookieUrl = $"{_options.Value.ConversionsUrl}/public/cookie-preferences?returnUrl=%2Fhome";

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

                return RedirectToPage(Links.Global.CookiePreferences);
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

                var cookieOptions = new CookieOptions { Expires = DateTime.Today.AddMonths(6), Secure = true, HttpOnly = true };
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
                var cookieOptions = new CookieOptions { Expires = DateTime.Today.AddMonths(6), Secure = true, HttpOnly = true };
                Response.Cookies.Append(ConsentCookieName, consent.Value.ToString(), cookieOptions);
            }

            if (!consent.Value)
            {
                foreach (var cookie in Request.Cookies.Keys)
                {
                    if (cookie.StartsWith("_ga") || cookie.Equals("_gid"))
                    {
                        _logger.LogInformation("Expiring Google analytics cookie: {cookie}", cookie);
                        Response.Cookies.Append(cookie, string.Empty, new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(-1),
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            HttpOnly = true
                        });
                    }
                }
            }
        }
    }
}
