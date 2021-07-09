using System.Threading.Tasks;
using Data.Models.KeyStagePerformance;
using Frontend.ExtensionMethods;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class KeyStage2Performance : PageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        public string ProjectUrn { get; private set; }
        public string OutgoingAcademyUrn { get; private set; }
        public string OutgoingAcademyName { get; private set; }
        public string LocalAuthorityName { get; private set; }
        public AdditionalInformationViewModel AdditionalInformation { get; private set; }
        public EducationPerformance EducationPerformance { get; private set; }

        public KeyStage2Performance(IGetInformationForProject getInformationForProject)
        {
            _getInformationForProject = getInformationForProject;
        }
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var projectInformation = await _getInformationForProject.Execute(id);

            if (!projectInformation.IsValid)
            {
                return this.View("ErrorPage", projectInformation.ResponseError.ErrorMessage);
            }

            BuildPageModel(projectInformation);

            return Page();
        }
        
        // TODO: Add post method to add additional information

        private void BuildPageModel(GetInformationForProjectResponse projectInformation)
        {
            ProjectUrn = projectInformation.Project.Urn;
            OutgoingAcademyUrn = projectInformation.OutgoingAcademy.Urn;
            LocalAuthorityName = projectInformation.OutgoingAcademy.LocalAuthorityName;
            OutgoingAcademyName = projectInformation.OutgoingAcademy.Name;
            EducationPerformance = projectInformation.EducationPerformance;
            AdditionalInformation = new AdditionalInformationViewModel
            {
                AdditionalInformation = projectInformation.Project.KeyStage2PerformanceAdditionalInformation,
                HintText =
                    "This information will populate in your HTB template under the key stage performance tables section.",
                Urn = projectInformation.Project.Urn,
                AddOrEditAdditionalInformation = false
            };
        }
    }
}