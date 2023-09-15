using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.TRAMS.ExtensionMethods;
using Dfe.PrepareTransfers.Web.Models.LegalRequirements;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.LegalRequirements
{
    public class OutgoingTrustConsentModel : CommonPageModel
    {
        private readonly IProjects _projects;

        public OutgoingTrustConsentModel(IProjects projects)
        {
            _projects = projects;
        }

        [BindProperty] public OutgoingTrustConsentViewModel OutgoingTrustConsentViewModel { get; set; } = new OutgoingTrustConsentViewModel();

        public IList<RadioButtonViewModel> RadioButtonsYesNoNotApplicable { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            IncomingTrustName = project.Result.IncomingTrustName;

            RadioButtonsYesNoNotApplicable = OutgoingTrustConsentViewModel.GetRadioButtons(project.Result.LegalRequirements.OutgoingTrustConsent.ToDescription(), OutgoingTrustConsentViewModel.OutgoingTrustConsent, nameof(OutgoingTrustConsentViewModel.OutgoingTrustConsent));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            if (ModelState.IsValid is false)
            {
                RadioButtonsYesNoNotApplicable = OutgoingTrustConsentViewModel.GetRadioButtons(project.Result.LegalRequirements.OutgoingTrustConsent.ToDescription(), OutgoingTrustConsentViewModel.OutgoingTrustConsent, nameof(OutgoingTrustConsentViewModel.OutgoingTrustConsent));
                return Page();
            }
            project.Result.LegalRequirements.OutgoingTrustConsent = OutgoingTrustConsentViewModel.OutgoingTrustConsent;
            await _projects.UpdateLegalRequirements(project.Result);
            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }
            return RedirectToPage(Links.LegalRequirements.Index.PageName, new { Urn });
        }
    }
}