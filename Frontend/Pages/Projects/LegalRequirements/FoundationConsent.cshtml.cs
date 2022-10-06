using Data;
using Frontend.Models.Forms;
using Frontend.Models.LegalRequirements;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.TRAMS.ExtensionMethods;
using Frontend.ExtensionMethods;

namespace Frontend.Pages.Projects.LegalRequirements
{
    public class FoundationConsentModel : CommonPageModel
    {
        private readonly IProjects _projects;

        public FoundationConsentModel(IProjects projects)
        {
            _projects = projects;
        }

        [BindProperty] public FoundationConsentViewModel FoundationConsentViewModel { get; set; } = new FoundationConsentViewModel();

        public IList<RadioButtonViewModel> RadioButtonsYesNoNotApplicable { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            IncomingTrustName = project.Result.IncomingTrustName;

            RadioButtonsYesNoNotApplicable = FoundationConsentViewModel.GetRadioButtons(project.Result.LegalRequirements.FoundationConsent.ToDescription(), FoundationConsentViewModel.FoundationConsent, nameof(FoundationConsentViewModel.FoundationConsent));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            if (ModelState.IsValid is false)
            {
                RadioButtonsYesNoNotApplicable = FoundationConsentViewModel.GetRadioButtons(project.Result.LegalRequirements.FoundationConsent.ToDescription(), FoundationConsentViewModel.FoundationConsent, nameof(FoundationConsentViewModel.FoundationConsent));
                return Page();
            }
            project.Result.LegalRequirements.FoundationConsent = FoundationConsentViewModel.FoundationConsent;
            await _projects.Update(project.Result);
            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }

            return RedirectToPage(Links.LegalRequirements.Index.PageName, new { Urn });
        }
    }
}