using Frontend.Models;
using Frontend.Models.Benefits;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.BenefitsAndRisks
{
    public class Risks : CommonPageModel
    {
        public RisksViewModel RisksViewModel => new RisksViewModel();

        public IActionResult OnGet()
        {
            return Page();
        }
    }
}