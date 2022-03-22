using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.Benefits;
using Frontend.Models.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.BenefitsAndRisks
{
    public class Risks : CommonPageModel
    {
        private readonly IProjects _projects;

        public Risks(IProjects projects)
        {
            _projects = projects;
        }

        [BindProperty] public RisksViewModel RisksViewModel { get; set; } = new RisksViewModel();

        public IList<RadioButtonViewModel> RadioButtonsYesNo { get; set; }


        public IActionResult OnGet()
        {
            RadioButtonsYesNo = GetRadioButtons();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            if (!ModelState.IsValid)
            {
                RadioButtonsYesNo = GetRadioButtons();
                return Page();
            }

            if (RisksViewModel.RisksInvolved == "Yes")
            {
                
            }
        }

        private IList<RadioButtonViewModel> GetRadioButtons()
        {
            var list = new List<RadioButtonViewModel>
            {
                new RadioButtonViewModel
                {
                    DisplayName = "Yes",
                    Name = $"{nameof(RisksViewModel.RisksInvolved)}",
                    Value = "Yes"
                },
                new RadioButtonViewModel
                {
                    DisplayName = "No",
                    Name = $"{nameof(RisksViewModel.RisksInvolved)}",
                    Value = "No"
                }
            };

            var selectedRadio =
                list.FirstOrDefault(c => c.Value == RisksViewModel.RisksInvolved);
            if (selectedRadio != null)
            {
                selectedRadio.Checked = true;
            }

            return list;
        }
    }
}