using Data;
using Frontend.Models;
using Frontend.Models.Benefits;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.BenefitsAndRisks
{
    public class Risks : CommonPageModel
    {
        private readonly IProjects _projects;

        public Risks(IProjects projects)
        {
            _projects = projects;
        }
        
        [BindProperty]
        public RisksViewModel RisksViewModel => new RisksViewModel();

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost(IProjects projects)
        {
            
        }
    }
}