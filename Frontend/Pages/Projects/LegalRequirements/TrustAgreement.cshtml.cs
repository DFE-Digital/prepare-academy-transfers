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

namespace Frontend.Pages.Projects.LegalRequirements
{
    public class TrustAgreementModel : CommonPageModel
    {
        private readonly IProjects _projects;

        public TrustAgreementModel(IProjects projects)
        {
            _projects = projects;
        }

        [BindProperty] public TrustAgreementViewModel TrustAgreementViewModel { get; set; } = new TrustAgreementViewModel();

        public IList<RadioButtonViewModel> RadioButtonsYesNoNotApplicable { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            IncomingTrustName = project.Result.IncomingTrustName;

            RadioButtonsYesNoNotApplicable = TrustAgreementViewModel.GetRadioButtons(project.Result.LegalRequirements.TrustAgreement.ToDescription(), TrustAgreementViewModel.TrustAgreement, nameof(TrustAgreementViewModel.TrustAgreement));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            if (!ModelState.IsValid)
            {
                RadioButtonsYesNoNotApplicable = TrustAgreementViewModel.GetRadioButtons(project.Result.LegalRequirements.TrustAgreement.ToDescription(), TrustAgreementViewModel.TrustAgreement, nameof(TrustAgreementViewModel.TrustAgreement));
                return Page();
            }

            project.Result.LegalRequirements.TrustAgreement = TrustAgreementViewModel.TrustAgreement;
            await _projects.Update(project.Result);
            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }


            return RedirectToPage(Links.LegalRequirements.Index.PageName, new { Urn });
        }
    }
}