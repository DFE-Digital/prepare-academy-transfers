using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Benefits;
using Frontend.Models.LegalRequirements;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.LegalRequirements
{
    public class Index : CommonPageModel
    {
        private readonly IProjects _projects;
        public LegalRequirementsViewModel LegalRequirementsViewModel;
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
            LegalRequirementsViewModel = new LegalRequirementsViewModel(
                projectResult.LegalRequirements.TrustAgreement,
                projectResult.LegalRequirements.DiocesanConsent,
                projectResult.Urn
            );
            MarkSectionCompletedViewModel = new MarkSectionCompletedViewModel
            {
                IsCompleted = projectResult.LegalRequirements.IsCompleted ?? false,
                ShowIsCompleted = LegalRequirementsSectionDataIsPopulated(projectResult)
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            var projectResult = project.Result;

            projectResult.LegalRequirements.IsCompleted = MarkSectionCompletedViewModel.IsCompleted;

            await _projects.Update(projectResult);

            return RedirectToPage(ReturnToPreview ? Links.HeadteacherBoard.Preview.PageName : Links.Project.Index.PageName,
                new { Urn });
        }

        private bool LegalRequirementsSectionDataIsPopulated(Project project) =>
            project.LegalRequirements.DiocesanConsent != null
            && project.LegalRequirements.TrustAgreement != null;


    }
}