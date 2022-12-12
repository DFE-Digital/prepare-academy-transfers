using Data;
using Frontend.Models.Benefits;
using Frontend.Models.Forms;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Data.TRAMS.ExtensionMethods;
using Frontend.ExtensionMethods;
using Frontend.Models.LegalRequirements;

namespace Frontend.Pages.Projects.LegalRequirements
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
            await _projects.Update(project.Result);
            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }
            return RedirectToPage(Links.LegalRequirements.Index.PageName, new { Urn });
        }
    }
}