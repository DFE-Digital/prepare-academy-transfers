using Data;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Models.LegalRequirements;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.TRAMS.ExtensionMethods;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.LegalRequirements
{
    public class IncomingTrustAgreementModel : CommonPageModel
    {
        private readonly IProjects _projects;

        public IncomingTrustAgreementModel(IProjects projects)
        {
            _projects = projects;
        }

        [BindProperty] public IncomingTrustAgreementViewModel IncomingTrustAgreementViewModel { get; set; } = new IncomingTrustAgreementViewModel();

        public IList<RadioButtonViewModel> RadioButtonsYesNoNotApplicable { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            IncomingTrustName = project.Result.IncomingTrustName;

            RadioButtonsYesNoNotApplicable = IncomingTrustAgreementViewModel.GetRadioButtons(project.Result.LegalRequirements.IncomingTrustAgreement.ToDescription(), IncomingTrustAgreementViewModel.IncomingTrustAgreement, nameof(IncomingTrustAgreementViewModel.IncomingTrustAgreement));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            if (!ModelState.IsValid)
            {
                RadioButtonsYesNoNotApplicable = IncomingTrustAgreementViewModel.GetRadioButtons(project.Result.LegalRequirements.IncomingTrustAgreement.ToDescription(), IncomingTrustAgreementViewModel.IncomingTrustAgreement, nameof(IncomingTrustAgreementViewModel.IncomingTrustAgreement));
                return Page();
            }

            project.Result.LegalRequirements.IncomingTrustAgreement = IncomingTrustAgreementViewModel.IncomingTrustAgreement;
            await _projects.Update(project.Result);
            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }


            return RedirectToPage(Links.LegalRequirements.Index.PageName, new { Urn });
        }
    }
}