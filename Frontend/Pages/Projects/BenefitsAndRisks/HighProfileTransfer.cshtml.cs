using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Benefits;
using Frontend.Models.Rationale;
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
            //projectResult.Rationale.Project = ViewModel.ProjectRationale;

            await _projectsRepository.Update(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = Urn});
            }

            var available = new List<TransferBenefits.OtherFactor>
            {
                TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues,
                TransferBenefits.OtherFactor.FinanceAndDebtConcerns,
                TransferBenefits.OtherFactor.OtherRisks
            };
            return RedirectToPage(OtherFactors.GetNextPage(available, projectResult.Benefits.OtherFactors), new {Urn});
        }

       
    }
}