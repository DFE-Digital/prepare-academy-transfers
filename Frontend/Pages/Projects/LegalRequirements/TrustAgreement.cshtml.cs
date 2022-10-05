using Data;
using Frontend.Models.Forms;
using Frontend.Models.LegalRequirements;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            RadioButtonsYesNoNotApplicable = GetRadioButtons(project.Result.LegalRequirements.TrustAgreement.ToString());
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            if (!ModelState.IsValid)
            {
                RadioButtonsYesNoNotApplicable = GetRadioButtons(project.Result.LegalRequirements.TrustAgreement.ToString());
                return Page();
            }

            project.Result.LegalRequirements.TrustAgreement = TrustAgreementViewModel.TrustAgreement;
            await _projects.Update(project.Result);
            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }


            return RedirectToPage("/Projects/LegalRequirements/Index", new { Urn });
        }

        private IList<RadioButtonViewModel> GetRadioButtons(string valueSelected)
        {
            var list = new List<RadioButtonViewModel>
            {
                new RadioButtonViewModel
                {
                    DisplayName = "Yes",
                    Name = $"{nameof(TrustAgreementViewModel.TrustAgreement)}",
                    Value = "Yes",
                    Checked = valueSelected is "Yes"
                },
                new RadioButtonViewModel
                {
                    DisplayName = "No",
                    Name = $"{nameof(TrustAgreementViewModel.TrustAgreement)}",
                    Value = "No",
                    Checked = valueSelected is "No"
                },
                new RadioButtonViewModel
                {
                    DisplayName = "Not Applicable",
                    Name = $"{nameof(TrustAgreementViewModel.TrustAgreement)}",
                    Value = "NotApplicable",
                    Checked = valueSelected is "Not Applicable"
                }
            };

            var selectedRadio =
                list.FirstOrDefault(c => c.Value == TrustAgreementViewModel.TrustAgreement.ToString());
            if (selectedRadio != null)
            {
                selectedRadio.Checked = true;
            }

            return list;
        }
    }
}