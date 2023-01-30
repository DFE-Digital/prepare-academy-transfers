using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.BenefitsAndRisks
{
    public class OtherRisks : CommonPageModel
    {
        private readonly IProjects _projectsRepository;
        [BindProperty] public string Answer { get; set; }

        public string PreviousPage { get; set; }

        public OtherRisks(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);
            var projectResult = project.Result;
            IncomingTrustName = projectResult.IncomingTrustName;
            Answer = projectResult.Benefits.OtherFactors[TransferBenefits.OtherFactor.OtherRisks];
            PreviousPage =
                OtherFactors.GetPage(
                    new List<TransferBenefits.OtherFactor>
                    {
                        TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues,
                        TransferBenefits.OtherFactor.HighProfile,
                        TransferBenefits.OtherFactor.FinanceAndDebtConcerns
                    },
                    projectResult.Benefits.OtherFactors, true);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var projectResult = project.Result;
            projectResult.Benefits.OtherFactors[TransferBenefits.OtherFactor.OtherRisks] = Answer ?? string.Empty;
            await _projectsRepository.Update(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {Urn});
            }

            return RedirectToPage("/Projects/BenefitsAndRisks/Index", new {Urn});
        }
    }
}