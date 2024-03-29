using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.BenefitsAndRisks
{
    public class ComplexLandAndBuilding : CommonPageModel
    {
        private readonly IProjects _projectsRepository;
        public string PreviousPage { get; set; }
        [BindProperty] public string Answer { get; set; }

        public ComplexLandAndBuilding(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);
            var projectResult = project.Result;
            Answer = projectResult.Benefits.OtherFactors[TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues];
            IncomingTrustName = projectResult.IncomingTrustName;
            PreviousPage =
                OtherFactors.GetPage(
                    new List<TransferBenefits.OtherFactor> {TransferBenefits.OtherFactor.HighProfile},
                    projectResult.Benefits.OtherFactors, true);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            var projectResult = project.Result;
            projectResult.Benefits.OtherFactors[TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues] = Answer ?? string.Empty;

            await _projectsRepository.UpdateBenefits(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {Urn});
            }
            
            var available = new List<TransferBenefits.OtherFactor>
            {
                TransferBenefits.OtherFactor.FinanceAndDebtConcerns,
                TransferBenefits.OtherFactor.OtherRisks
            };

            return RedirectToPage(OtherFactors.GetPage(available, projectResult.Benefits.OtherFactors), new {Urn});
        }
    }
}