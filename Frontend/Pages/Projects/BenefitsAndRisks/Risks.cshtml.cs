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


        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            RadioButtonsYesNo = GetRadioButtons(project.Result.Benefits.AnyRisks);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            if (!ModelState.IsValid)
            {
                RadioButtonsYesNo = GetRadioButtons(project.Result.Benefits.AnyRisks);
                return Page();
            }
            
            project.Result.Benefits.AnyRisks = RisksViewModel.RisksInvolved;
            await _projects.Update(project.Result);
              
            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = Urn});
            }
            
            return RedirectToPage("/Projects/BenefitsAndRisks/Index", new {Urn});
        }

        private IList<RadioButtonViewModel> GetRadioButtons(bool? valueSelected)
        {
            var list = new List<RadioButtonViewModel>
            {
                new RadioButtonViewModel
                {
                    DisplayName = "Yes",
                    Name = $"{nameof(RisksViewModel.RisksInvolved)}",
                    Value = "true",
                    Checked = valueSelected is true
                },
                new RadioButtonViewModel
                {
                    DisplayName = "No",
                    Name = $"{nameof(RisksViewModel.RisksInvolved)}",
                    Value = "false",
                    Checked = valueSelected is false
                }
            };

            var selectedRadio =
                list.FirstOrDefault(c => c.Value == RisksViewModel.RisksInvolved.ToString());
            if (selectedRadio != null)
            {
                selectedRadio.Checked = true;
            }

            return list;
        }
    }
}