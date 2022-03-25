using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Benefits;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable UnusedMember.Global - properties bound using [BindProperty]

namespace Frontend.Pages.Projects.BenefitsAndRisks
{
    public class OtherFactors : CommonPageModel
    {
        private readonly IProjects _projects;

        public OtherFactors(IProjects projects)
        {
            _projects = projects;
        }

        [BindProperty] public OtherFactorsViewModel OtherFactorsViewModel { get; set; } = new OtherFactorsViewModel();

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            var projectResult = project.Result;
            IncomingTrustName = projectResult.IncomingTrustName;
            OtherFactorsViewModel.OtherFactorsVm = BuildOtherFactorsItemViewModel(projectResult.Benefits.OtherFactors);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var projectResult = project.Result;
            projectResult.Benefits.OtherFactors = OtherFactorsViewModel.OtherFactorsVm
                .Where(of => of.Checked)
                .ToDictionary(d => d.OtherFactor, x => x.Description);
            await _projects.Update(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = Urn});
            }

            var available = new List<TransferBenefits.OtherFactor>
            {
                TransferBenefits.OtherFactor.HighProfile,
                TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues,
                TransferBenefits.OtherFactor.FinanceAndDebtConcerns,
                TransferBenefits.OtherFactor.OtherRisks
            };
            return RedirectToPage(GetNextPage(available, projectResult.Benefits.OtherFactors), new {Urn});
        }

        public static string GetNextPage(List<TransferBenefits.OtherFactor> available,
            Dictionary<TransferBenefits.OtherFactor, string> otherFactors)
        {
            var foundOtherFactor =
                available.FirstOrDefault(otherFactor => otherFactors.Select(o => o.Key).Contains(otherFactor));

            switch (foundOtherFactor)
            {
                case TransferBenefits.OtherFactor.HighProfile:
                    return "/Projects/BenefitsAndRisks/HighProfileTransfer";
                case TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues:
                    return "/Projects/BenefitsAndRisks/ComplexLandAndBuilding";
                case TransferBenefits.OtherFactor.FinanceAndDebtConcerns:
                    return "/Projects/BenefitsAndRisks/FinanceAndDebt";
                case TransferBenefits.OtherFactor.OtherRisks:
                    return "/Projects/BenefitsAndRisks/OtherRisks";
            }

            return "/Projects/BenefitsAndRisks/Index";
        }


        public static List<OtherFactorsItemViewModel> BuildOtherFactorsItemViewModel(
            Dictionary<TransferBenefits.OtherFactor, string> otherFactorsToSet)
        {
            List<OtherFactorsItemViewModel> items = new List<OtherFactorsItemViewModel>();
            foreach (TransferBenefits.OtherFactor otherFactor in Enum.GetValues(typeof(TransferBenefits.OtherFactor)))
            {
                if (otherFactor != TransferBenefits.OtherFactor.Empty)
                {
                    var isChecked = otherFactorsToSet.ContainsKey(otherFactor);

                    items.Add(new OtherFactorsItemViewModel()
                    {
                        OtherFactor = otherFactor,
                        Checked = isChecked
                    });
                }
            }

            return items;
        }
    }
}