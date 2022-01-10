using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Frontend.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Frontend.Pages.Home
{
    public class Index : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IProjects _projectsRepository;
        private readonly ILogger<Index> _logger;
        public List<ProjectSearchResult> Projects;
        public int Page { get; set; }

        public Index(IConfiguration configuration, IProjects projectsRepository,
            ILogger<Index> logger)
        {
            _configuration = configuration;
            _projectsRepository = projectsRepository;
            _logger = logger;
        }
        
        public async Task<IActionResult> OnGet()
        {
            var projects = await _projectsRepository.GetProjects(Page);
            if (!projects.IsValid)
            {
                return this.View("ErrorPage", projects.Error.ErrorMessage);
            }

            _logger.LogInformation("Home page loaded");
            Projects = projects.Result;

            return Page();
        }
    }
}