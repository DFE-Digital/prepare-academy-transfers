using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.BenefitsAndRisks
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
            projectResult.Benefits.OtherFactors[TransferBenefits.OtherFactor.OtherRisks] = Answer ?? string.Empty;;
            await _projectsRepository.Update(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {Urn});
            }

            return RedirectToPage("/Projects/BenefitsAndRisks/Index", new {Urn});
        }
    }
}