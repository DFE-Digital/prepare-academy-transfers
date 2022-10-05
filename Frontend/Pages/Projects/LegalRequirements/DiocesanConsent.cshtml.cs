using Data;
using Frontend.Models.Benefits;
using Frontend.Models.Forms;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.ExtensionMethods;
using Frontend.Models.LegalRequirements;

namespace Frontend.Pages.Projects.LegalRequirements
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

            RadioButtonsYesNoNotApplicable = GetRadioButtons(project.Result.LegalRequirements.DiocesanConsent.ToDescription());
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            if (ModelState.IsValid is false)
            {
                RadioButtonsYesNoNotApplicable = GetRadioButtons(project.Result.LegalRequirements.DiocesanConsent.ToDescription());
                return Page();
            }


            project.Result.LegalRequirements.DiocesanConsent = DiocesanConsentViewModel.DiocesanConsent;
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
                    Name = $"{nameof(DiocesanConsentViewModel.DiocesanConsent)}",
                    Value = "Yes",
                    Checked = valueSelected is "Yes"
                },
                new RadioButtonViewModel
                {
                    DisplayName = "No",
                    Name = $"{nameof(DiocesanConsentViewModel.DiocesanConsent)}",
                    Value = "No",
                    Checked = valueSelected is "No"
                },
                new RadioButtonViewModel
                {
                    DisplayName = "Not Applicable",
                    Name = $"{nameof(DiocesanConsentViewModel.DiocesanConsent)}",
                    Value = "NotApplicable",
                    Checked = valueSelected is "Not Applicable"
                }
            };

            var selectedRadio =
                list.FirstOrDefault(c => c.Value == DiocesanConsentViewModel.DiocesanConsent.ToString());
            if (selectedRadio != null)
            {
                selectedRadio.Checked = true;
            }

            return list;
        }
    }
}