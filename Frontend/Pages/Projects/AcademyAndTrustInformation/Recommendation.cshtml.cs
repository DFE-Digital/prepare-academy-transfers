using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.ExtensionMethods;
using Frontend.Models;
using Frontend.Models.AcademyAndTrustInformation;
using Frontend.Models.Forms;
using Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.AcademyAndTrustInformation
{
    public class Recommendation : CommonPageModel
    {
        private readonly IProjects _projectRepository;
        public TransferAcademyAndTrustInformation.RecommendationResult RecommendationResult { get; set; }
        public string Author { get; set; }

        public static List<RadioButtonViewModel> RecommendedRadioButtons(
            TransferAcademyAndTrustInformation.RecommendationResult selected)
        {
            var values =
                EnumHelpers<TransferAcademyAndTrustInformation.RecommendationResult>.GetDisplayableValues(
                    TransferAcademyAndTrustInformation.RecommendationResult
                        .Empty);

            var result = values.Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(),
                Name = "recommendation",
                DisplayName = value.ToString(),
                Checked = selected == value
            }).ToList();

            return result;
        }

        public Recommendation(IProjects projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IActionResult> OnGetAsync(string urn, bool returnToPreview = false)
        {
            var project = await _projectRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return this.View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;

            Urn = projectResult.Urn;
            OutgoingAcademyName = projectResult.OutgoingAcademyName;
            ReturnToPreview = returnToPreview;
            Author = projectResult.AcademyAndTrustInformation.Author;
            RecommendationResult = projectResult.AcademyAndTrustInformation.Recommendation;
            OutgoingAcademyUrn = projectResult.OutgoingAcademyUrn;
            
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(RecommendationViewModel vm)
        {
            var project = await _projectRepository.GetByUrn(vm.Urn);
            if (!project.IsValid)
            {
                return this.View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            projectResult.AcademyAndTrustInformation.Recommendation = vm.Recommendation;
            projectResult.AcademyAndTrustInformation.Author = vm.Author;

            var result = await _projectRepository.Update(projectResult);
            if (!result.IsValid)
            {
                return this.View("ErrorPage", result.Error.ErrorMessage);
            }

            if (vm.ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = vm.Urn});
            }

            return RedirectToPage("/Projects/AcademyAndTrustInformation/Index", new {vm.Urn});
        }
    }
}