using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.Features;
using Frontend.Models.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.Features
{
    public class Reason : CommonPageModel
    {
        private readonly IProjects _projects;

        [BindProperty] public FeaturesReasonViewModel FeaturesReasonViewModel { get; set; } = new FeaturesReasonViewModel();

        public Reason(IProjects projects)
        {
            _projects = projects;
        }

        public async Task<IActionResult> OnGetAsync()
        {

            var project = await _projects.GetByUrn(Urn);
            
            var projectResult = project.Result;
            OutgoingAcademyName = projectResult.OutgoingAcademyName;
            FeaturesReasonViewModel.IsSubjectToIntervention =
                projectResult.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention;
            FeaturesReasonViewModel.MoreDetail = projectResult.Features.ReasonForTransfer.InterventionDetails;

            return Page();
        }

        public List<RadioButtonViewModel> ReasonRadioButtons()
        {
            var result = new[] {true, false}.Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(),
                Name = nameof(FeaturesReasonViewModel.IsSubjectToIntervention),
                DisplayName = value ? "Yes" : "No",
                Checked = FeaturesReasonViewModel.IsSubjectToIntervention == value
            }).ToList();

            return result;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            project.Result.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention = FeaturesReasonViewModel.IsSubjectToIntervention;
            project.Result.Features.ReasonForTransfer.InterventionDetails = FeaturesReasonViewModel.MoreDetail ?? string.Empty;
            
            await _projects.Update(project.Result);
            
            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { id = Urn });
            }
            
            return RedirectToPage("/Projects/Features/Index", new { Urn });
        }
    }
}