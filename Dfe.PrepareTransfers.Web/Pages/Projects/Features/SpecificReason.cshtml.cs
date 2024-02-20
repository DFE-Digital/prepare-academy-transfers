using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.Projects;
using FluentValidation.AspNetCore;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Validators.Features;
using Dfe.PrepareTransfers.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.Features
{
    public class SpecificReason : CommonPageModel
    {
        private readonly IProjects _projects;

        public SpecificReason(IProjects projects)
        {
            _projects = projects;
        }

        [BindProperty]
        public TransferFeatures.SpecificReasonForTheTransferTypes SpecificReasonForTheTransfer { get; set; }
        [BindProperty]
        public TransferFeatures.ReasonForTheTransferTypes ReasonForTheTransfer { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            var projectResult = project.Result;
            IncomingTrustName = projectResult.IncomingTrustName;
            ReasonForTheTransfer = projectResult.Features.ReasonForTheTransfer;
            SpecificReasonForTheTransfer = projectResult.Features.SpecificReasonForTheTransfer;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            var validator = new FeaturesSpecificReasonValidator();
            var validationResults = await validator.ValidateAsync(this);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            project.Result.Features.SpecificReasonForTheTransfer = SpecificReasonForTheTransfer;

            await _projects.UpdateFeatures(project.Result);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }

            return RedirectToPage("/Projects/Features/Index", new { Urn });

        }

        public static List<RadioButtonViewModel> SpecificReasonRadioButtons(TransferFeatures.SpecificReasonForTheTransferTypes selected, TransferFeatures.ReasonForTheTransferTypes parentReason)
        {
            var values =
                EnumHelpers<TransferFeatures.SpecificReasonForTheTransferTypes>.GetDisplayableValues(TransferFeatures.SpecificReasonForTheTransferTypes
                    .Empty);

            var result = values.Where(x => AttributeHelpers.GetAttribute<ReasonParentAttribute>(x).Parent == parentReason).Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(),
                Name = nameof(SpecificReasonForTheTransfer),
                DisplayName = EnumHelpers<TransferFeatures.SpecificReasonForTheTransferTypes>.GetDisplayValue(value),
                Checked = selected == value
            }).ToList();

            return result;
        }
    }
}