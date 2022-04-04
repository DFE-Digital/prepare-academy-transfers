using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.BenefitsAndRisks
{
    public class HighProfileTransfer : CommonPageModel
    {
        private readonly IProjects _projectsRepository;

        [BindProperty] public string Answer { get; set; }

        public HighProfileTransfer(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            var projectResult = project.Result;
            IncomingTrustName = projectResult.IncomingTrustName;
            Answer = projectResult.Benefits.OtherFactors[TransferBenefits.OtherFactor.HighProfile];
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            var projectResult = project.Result;
            projectResult.Benefits.OtherFactors[TransferBenefits.OtherFactor.HighProfile] = Answer ?? string.Empty;
            await _projectsRepository.Update(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {Urn});
            }

            var available = new List<TransferBenefits.OtherFactor>
            {
                TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues,
                TransferBenefits.OtherFactor.FinanceAndDebtConcerns,
                TransferBenefits.OtherFactor.OtherRisks
            };
            return RedirectToPage(OtherFactors.GetPage(available, projectResult.Benefits.OtherFactors), new {Urn});
        }
    }
}