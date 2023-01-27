using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Benefits;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.BenefitsAndRisks
{
    public class IntendedBenefits : CommonPageModel
    {
        private readonly IProjects _projects;

        [BindProperty]
        public IntendedBenefitsViewModel IntendedBenefitsViewModel { get; set; } = new IntendedBenefitsViewModel();

        public IList<CheckboxViewModel> Checkboxes { get; set; }

        public IntendedBenefits(IProjects projects)
        {
            _projects = projects;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            var projectResult = project.Result;

            IncomingTrustName = projectResult.IncomingTrustName;
            IntendedBenefitsViewModel.SelectedIntendedBenefits = projectResult.Benefits.IntendedBenefits;
            IntendedBenefitsViewModel.OtherBenefit =
                projectResult.Benefits.IntendedBenefits.Contains(TransferBenefits.IntendedBenefit.Other)
                    ? projectResult.Benefits.OtherIntendedBenefit
                    : null;
            Checkboxes = GetIntendedBenefitsCheckboxes();
            return Page();
        }

        private IList<CheckboxViewModel> GetIntendedBenefitsCheckboxes()
        {
            IList<CheckboxViewModel> items = new List<CheckboxViewModel>();
            foreach (var intendedBenefit in EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayableValues(
                         TransferBenefits.IntendedBenefit.Empty))
            {
                items.Add(new CheckboxViewModel
                {
                    DisplayName = EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue(intendedBenefit),
                    Name =
                        $"{nameof(IntendedBenefitsViewModel)}.{nameof(IntendedBenefitsViewModel.SelectedIntendedBenefits)}",
                    Value = intendedBenefit.ToString(),
                    Checked = IntendedBenefitsViewModel.SelectedIntendedBenefits.Contains(intendedBenefit)
                });
            }

            return items;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            if (!ModelState.IsValid)
            {
                Checkboxes = GetIntendedBenefitsCheckboxes();
                return Page();
            }

            var projectResult = project.Result;
            projectResult.Benefits.IntendedBenefits =
                new List<TransferBenefits.IntendedBenefit>(IntendedBenefitsViewModel.SelectedIntendedBenefits);
            projectResult.Benefits.OtherIntendedBenefit = IntendedBenefitsViewModel.OtherBenefit;

            await _projects.Update(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {Urn});
            }

            return RedirectToPage("/Projects/BenefitsAndRisks/Index", new {Urn});
        }
    }
}