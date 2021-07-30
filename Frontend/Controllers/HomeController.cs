using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Views.Home;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IProjects _projectsRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, IProjects projectsRepository,
            ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _projectsRepository = projectsRepository;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Index(int page = 1)
        {
            var projects = await _projectsRepository.GetProjects(page);
            if (!projects.IsValid)
            {
                return View("ErrorPage", projects.Error.ErrorMessage);
            }

            _logger.LogInformation("Home page loaded");
            var model = new Index {Projects = projects.Result, Page = page};
            return View(model);
        }

        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public async Task<IActionResult> SubmitLogin(string username, string password, string returnUrl)
        {
            var decodedUrl = "";

            if (username != _configuration["username"] || password != _configuration["password"])
            {
                TempData["Error.Message"] = "Incorrect username and password";
                return RedirectToAction("Login", new {returnUrl});
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Name")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authenticationProperties = new AuthenticationProperties();
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authenticationProperties);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                decodedUrl = WebUtility.UrlDecode(returnUrl);
            }

            if (Url.IsLocalUrl(decodedUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }
    }
}