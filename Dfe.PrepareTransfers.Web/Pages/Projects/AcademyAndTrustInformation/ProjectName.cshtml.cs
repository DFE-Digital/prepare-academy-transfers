using Dfe.PrepareTransfers.Data;
using FluentValidation.AspNetCore;
using Dfe.PrepareTransfers.Web.Validators.Transfers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Models;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.AcademyAndTrustInformation
{
    public class ProjectNameModel : CommonPageModel
    {
        private readonly IProjects _projectRepository;

        public ProjectNameModel(IProjects projectRepository)
        {
            _projectRepository = projectRepository;
        }

        [BindProperty]
        public string ProjectName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var validator = new ProjectNameValidator();
            var validationResults = await validator.ValidateAsync(this);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            await _projectRepository.UpdateProjectName(Urn, ProjectName);

            return RedirectToPage("/Projects/AcademyAndTrustInformation/Index", new { Urn });
        }
    }
}
