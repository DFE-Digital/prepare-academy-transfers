using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.ExtensionMethods;
using Frontend.Models;
using Frontend.Models.Features;
using Frontend.Models.Forms;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Projects.Features
{
    public class Type : CommonPageModel
    {
        private readonly IProjects _projects;
        [BindProperty] public FeaturesTypeViewModel FeaturesTypeViewModel { get; set; } = new FeaturesTypeViewModel();

        public Type(IProjects projects)
        {
            _projects = projects;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            if (!project.IsValid)
            {
                return this.View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            FeaturesTypeViewModel.TypeOfTransfer = projectResult.Features.TypeOfTransfer;
            FeaturesTypeViewModel.OtherType = projectResult.Features.OtherTypeOfTransfer;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            if (!project.IsValid)
            {
                return this.View("ErrorPage", project.Error.ErrorMessage);
            }
            
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var projectResult = project.Result;
            projectResult.Features.TypeOfTransfer = FeaturesTypeViewModel.TypeOfTransfer;
            projectResult.Features.OtherTypeOfTransfer = FeaturesTypeViewModel.OtherType;
            
            var result = await _projects.Update(projectResult);
            if (!result.IsValid)
            {
                return this.View("ErrorPage", result.Error.ErrorMessage);
            }
            
            return ReturnToPreview ? 
                RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { id = Urn }) :
                RedirectToPage("/Projects/Features/Index", new { Urn });
        }

        public List<RadioButtonViewModel> TypeOfTransferRadioButtons()
        {
            var values =
                EnumHelpers<TransferFeatures.TransferTypes>.GetDisplayableValues(TransferFeatures.TransferTypes
                    .Empty);

            var result = values.Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(),
                Name = $"{nameof(FeaturesTypeViewModel)}.{nameof(FeaturesTypeViewModel.TypeOfTransfer)}",
                DisplayName = EnumHelpers<TransferFeatures.TransferTypes>.GetDisplayValue(value),
                Checked = FeaturesTypeViewModel.TypeOfTransfer == value
            }).ToList();

            return result;
        }
    }
}