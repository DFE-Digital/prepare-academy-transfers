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
        [BindProperty(SupportsGet = true)] public bool IsCompleted { get; set; }
        public bool ShowIsCompleted { get; private set; }

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
            IsCompleted = projectResult.Rationale.IsCompleted ?? false;
            ShowIsCompleted = !string.IsNullOrEmpty(projectResult.Rationale.Project) && 
                              !string.IsNullOrEmpty(projectResult.Rationale.Trust);
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            
            var projectResult = project.Result;
            
            projectResult.Rationale.IsCompleted = IsCompleted;

            await _projects.Update(projectResult);

            return RedirectToPage(ReturnToPreview ? Links.HeadteacherBoard.Preview.PageName : "/Projects/Index", new {Urn});
        }
    }
}