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
    public class DiocesanConsentModel : CommonPageModel
    {
        private readonly IProjects _projects;

        public DiocesanConsentModel(IProjects projects)
        {
            _projects = projects;
        }

        [BindProperty] public DiocesanConsentViewModel DiocesanConsentViewModel { get; set; } = new DiocesanConsentViewModel();

        public IList<RadioButtonViewModel> RadioButtonsYesNoNotApplicable { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            IncomingTrustName = project.Result.IncomingTrustName;

            RadioButtonsYesNoNotApplicable = DiocesanConsentViewModel.GetRadioButtons(project.Result.LegalRequirements.DiocesanConsent.ToDescription(), DiocesanConsentViewModel.DiocesanConsent, nameof(DiocesanConsentViewModel.DiocesanConsent));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            if (ModelState.IsValid is false)
            {
                RadioButtonsYesNoNotApplicable = DiocesanConsentViewModel.GetRadioButtons(project.Result.LegalRequirements.DiocesanConsent.ToDescription(), DiocesanConsentViewModel.DiocesanConsent, nameof(DiocesanConsentViewModel.DiocesanConsent));
                return Page();
            }
            project.Result.LegalRequirements.DiocesanConsent = DiocesanConsentViewModel.DiocesanConsent;
            await _projects.Update(project.Result);
            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }
            return RedirectToPage(Links.LegalRequirements.Index.PageName, new { Urn });
        }
    }
}