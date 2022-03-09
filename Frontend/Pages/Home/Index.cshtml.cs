using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Frontend.Pages.Home
{
    public class Index : PageModel
    {
        private readonly IProjects _projectsRepository;
        private readonly ILogger<Index> _logger;
        public List<ProjectSearchResult> Projects;
        public int StartingPage { get; private set; } = 1;
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => Projects.Count == 10;
        public int PreviousPage => CurrentPage - 1;
        public int NextPage => CurrentPage + 1;

        [BindProperty(SupportsGet = true)] public int CurrentPage { get; set; } = 1;

        public Index(IProjects projectsRepository, ILogger<Index> logger)
        {
            _projectsRepository = projectsRepository;
            _logger = logger;
        }

        
        public async Task<IActionResult> OnGetAsync()
        {
            var projects = await _projectsRepository.GetProjects(CurrentPage);

            _logger.LogInformation("Home page loaded");
            Projects = projects.Result;

            if (CurrentPage - 5 > 1)
            {
                StartingPage = CurrentPage - 5;
            }

            return Page();
        }
    }
}