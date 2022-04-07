using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.Rationale
{
    public class Index : CommonPageModel
    {
        public string ProjectRationale { get; set; }
        public string TrustRationale { get; set; }

        private readonly IProjects _projects;
        [BindProperty]
        public MarkSectionCompletedViewModel MarkSectionCompletedViewModel { get; set; }

        public Index(IProjects projects)
        {
            _projects = projects;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            var projectResult = project.Result;

            ProjectReference = projectResult.Reference;
            ProjectRationale = projectResult.Rationale.Project;
            TrustRationale = projectResult.Rationale.Trust;
            OutgoingAcademyUrn = projectResult.OutgoingAcademyUrn;
            MarkSectionCompletedViewModel = new MarkSectionCompletedViewModel
            {
                IsCompleted = projectResult.Rationale.IsCompleted ?? false,
                ShowIsCompleted = !string.IsNullOrEmpty(projectResult.Rationale.Project) &&
                                  !string.IsNullOrEmpty(projectResult.Rationale.Trust)
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            var projectResult = project.Result;

            projectResult.Rationale.IsCompleted = MarkSectionCompletedViewModel.IsCompleted;

            await _projects.Update(projectResult);

            return RedirectToPage(ReturnToPreview ? Links.HeadteacherBoard.Preview.PageName : "/Projects/Index",
                new {Urn});
        }
    }
}