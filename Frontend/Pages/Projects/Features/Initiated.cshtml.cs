using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Features;
using Frontend.Models.Forms;
using Helpers;
using Microsoft.AspNetCore.Mvc;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global (Set on model binding)

namespace Frontend.Pages.Projects.Features
{
    public class Initiated : CommonPageModel
    {
        private readonly IProjects _projects;
        [BindProperty]
        public FeaturesInitiatedViewModel FeaturesInitiatedViewModel { get; set; } = new FeaturesInitiatedViewModel();

        public Initiated(IProjects projects)
        {
            _projects = projects;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            
            var projectResult = project.Result;
            OutgoingAcademyName = projectResult.OutgoingAcademyName;
            FeaturesInitiatedViewModel.WhoInitiated = projectResult.Features.WhoInitiatedTheTransfer;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            project.Result.Features.WhoInitiatedTheTransfer = FeaturesInitiatedViewModel.WhoInitiated;
            
            await _projects.Update(project.Result);
            
            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { id = Urn });
            }
            
            return RedirectToPage("/Projects/Features/Index", new { Urn });
            
        }

        public static List<RadioButtonViewModel> InitiatedRadioButtons(TransferFeatures.ProjectInitiators selected)
        {
            var values =
                EnumHelpers<TransferFeatures.ProjectInitiators>.GetDisplayableValues(TransferFeatures.ProjectInitiators
                    .Empty);

            var result = values.Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(),
                Name = "WhoInitiated",
                DisplayName = EnumHelpers<TransferFeatures.ProjectInitiators>.GetDisplayValue(value),
                Checked = selected == value
            }).ToList();

            return result;
        }
    }
}