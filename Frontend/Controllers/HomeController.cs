using System.Diagnostics;
using Data;
using Frontend.Models;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}