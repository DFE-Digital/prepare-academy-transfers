using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Benefits;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.BenefitsAndRisks
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

            var originalAnswer = project.Result.Benefits.AnyRisks;
            project.Result.Benefits.AnyRisks = RisksViewModel.RisksInvolved;
            if (RisksViewModel.RisksInvolved == false)
            {
                project.Result.Benefits.OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>();
            }

            await _projects.UpdateBenefits(project.Result);

            //Only go back to preview if No,or no change. Changing to Yes will take them through the options
            if (ReturnToPreview && RisksViewModel.RisksInvolved == false ||
                ReturnToPreview && RisksViewModel.RisksInvolved == true && originalAnswer == true)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {Urn});
            }

            return RisksViewModel.RisksInvolved == true
                ? RedirectToPage("/Projects/BenefitsAndRisks/OtherFactors", new {Urn})
                : RedirectToPage("/Projects/BenefitsAndRisks/Index", new {Urn});
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