using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Models.Upstream.Enums;
using API.Models.Upstream.Response;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Frontend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IProjectsRepository _projectsRepository;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration,
            IProjectsRepository projectsRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _projectsRepository = projectsRepository;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var projects = await _projectsRepository.SearchProject("", ProjectStatusEnum.InProgress, false);
            var projectInformation = new Dictionary<string, SearchProjectsModel>() { };

            projects.Result.Projects.ForEach(project =>
                projectInformation.Add(project.ProjectId.ToString(), project));

            ViewData["ProjectInformation"] = projectInformation;

            return View();
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

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Name")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authenticationProperties = new AuthenticationProperties();
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authenticationProperties);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                decodedUrl = System.Net.WebUtility.UrlDecode(returnUrl);
            }

            if (Url.IsLocalUrl(decodedUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }
    }
}