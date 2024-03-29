using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Rationale;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.Rationale
{
    public class Project : CommonPageModel
    {
        private readonly IProjects _projectsRepository;

        public Project(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        [BindProperty] public RationaleProjectViewModel ViewModel { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            var projectResult = project.Result;

            IncomingTrustName = projectResult.IncomingTrustName;
            ViewModel = new RationaleProjectViewModel
            {
                ProjectRationale = projectResult.Rationale.Project
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var projectResult = project.Result;
            projectResult.Rationale.Project = ViewModel.ProjectRationale;

            await _projectsRepository.UpdateRationale(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {Urn});
            }

            return RedirectToPage("/Projects/Rationale/Index", new {Urn});
        }
    }
}