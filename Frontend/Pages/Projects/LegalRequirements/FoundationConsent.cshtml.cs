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

            RadioButtonsYesNoNotApplicable = GetRadioButtons(project.Result.LegalRequirements.FoundationConsent.ToString());
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            if (!ModelState.IsValid)
            {
                RadioButtonsYesNoNotApplicable = GetRadioButtons(project.Result.LegalRequirements.FoundationConsent.ToString());
                return Page();
            }

            project.Result.LegalRequirements.FoundationConsent = FoundationConsentViewModel.FoundationConsent;
            await _projects.Update(project.Result);

            return RedirectToPage("/Projects/LegalRequirements/Index", new { Urn });
        }

        private IList<RadioButtonViewModel> GetRadioButtons(string valueSelected)
        {
            var list = new List<RadioButtonViewModel>
            {
                new RadioButtonViewModel
                {
                    DisplayName = "Yes",
                    Name = $"{nameof(FoundationConsentViewModel.FoundationConsent)}",
                    Value = "Yes",
                    Checked = valueSelected is "Yes"
                },
                new RadioButtonViewModel
                {
                    DisplayName = "No",
                    Name = $"{nameof(FoundationConsentViewModel.FoundationConsent)}",
                    Value = "No",
                    Checked = valueSelected is "No"
                },
                new RadioButtonViewModel
                {
                    DisplayName = "Not Applicable",
                    Name = $"{nameof(FoundationConsentViewModel.FoundationConsent)}",
                    Value = "Not Applicable",
                    Checked = valueSelected is "Not Applicable"
                }
            };

            var selectedRadio =
                list.FirstOrDefault(c => c.Value == FoundationConsentViewModel.FoundationConsent.ToString());
            if (selectedRadio != null)
            {
                selectedRadio.Checked = true;
            }

            return list;
        }
    }
}