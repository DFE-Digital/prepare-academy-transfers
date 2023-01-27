using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.BenefitsAndRisks
{
    public class FinanceAndDebt : CommonPageModel
    {
        private readonly IProjects _projectsRepository;

        [BindProperty] public string Answer { get; set; }
        public string PreviousPage { get; set; }

        public FinanceAndDebt(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            var projectResult = project.Result;
            Answer = projectResult.Benefits.OtherFactors[TransferBenefits.OtherFactor.FinanceAndDebtConcerns];
            IncomingTrustName = projectResult.IncomingTrustName;
            PreviousPage =
                OtherFactors.GetPage(
                    new List<TransferBenefits.OtherFactor>
                    {
                        TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues,
                        TransferBenefits.OtherFactor.HighProfile,
                    },
                    projectResult.Benefits.OtherFactors, true);
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);
            var projectResult = project.Result;
            projectResult.Benefits.OtherFactors[TransferBenefits.OtherFactor.FinanceAndDebtConcerns] = Answer ?? string.Empty;;
            await _projectsRepository.Update(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {Urn});
            }

            var available = new List<TransferBenefits.OtherFactor>
            {
                TransferBenefits.OtherFactor.OtherRisks
            };
            return RedirectToPage(OtherFactors.GetPage(available, projectResult.Benefits.OtherFactors), new {Urn});
        }
    }
}