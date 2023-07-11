using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Features;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.Features
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
            
            var projectResult = project.Result;
            FeaturesTypeViewModel.TypeOfTransfer = projectResult.Features.TypeOfTransfer;
            FeaturesTypeViewModel.OtherType = projectResult.Features.OtherTypeOfTransfer;
            IncomingTrustName = projectResult.IncomingTrustName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var projectResult = project.Result;
            projectResult.Features.TypeOfTransfer = FeaturesTypeViewModel.TypeOfTransfer;
            projectResult.Features.OtherTypeOfTransfer = FeaturesTypeViewModel.OtherType;
            
            await _projects.Update(projectResult);
            
            return ReturnToPreview ? 
                RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn }) :
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