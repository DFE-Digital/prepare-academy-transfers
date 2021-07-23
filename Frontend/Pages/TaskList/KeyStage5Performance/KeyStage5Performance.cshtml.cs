using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Models.KeyStagePerformance;
using Frontend.ExtensionMethods;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class KeyStage5Performance : PageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projects;
        public string ProjectUrn { get; private set; }
        public EducationPerformance EducationPerformance { get; set; }
        public string LocalAuthorityName { get; set; }
        public string OutgoingAcademyName { get; set; }
        public AdditionalInformationViewModel AdditionalInformation { get; set; }
        public string OutgoingAcademyUrn { get; set; }

        public KeyStage5Performance(IGetInformationForProject getInformationForProject, IProjects projects)
        {
            _getInformationForProject = getInformationForProject;
            _projects = projects;
        }

        public async Task<IActionResult> OnGetAsync(string id, bool addOrEditAdditionalInformation = false)
        {
            var projectInformation = await _getInformationForProject.Execute(id);

            if (!projectInformation.IsValid)
            {
                return this.View("ErrorPage", projectInformation.ResponseError.ErrorMessage);
            }

            PopulateModel(addOrEditAdditionalInformation, projectInformation);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id, string additionalInformation)
        {
            var project = await _projects.GetByUrn(id);

            if (!project.IsValid)
            {
                return this.View("ErrorPage", project.Error.ErrorMessage);
            }

            project.Result.KeyStage5PerformanceAdditionalInformation = additionalInformation;
            await _projects.Update(project.Result);

            return new RedirectToPageResult(nameof(KeyStage5Performance),
                "OnGetAsync",
                new {id},
                "additional-information-hint");
        }

        private void PopulateModel(bool addOrEditAdditionalInformation,
            GetInformationForProjectResponse projectInformation)
        {
            ProjectUrn = projectInformation.Project.Urn;
            OutgoingAcademyUrn = projectInformation.OutgoingAcademy.Urn;
            LocalAuthorityName = projectInformation.OutgoingAcademy.LocalAuthorityName;
            OutgoingAcademyName = projectInformation.OutgoingAcademy.Name;
            EducationPerformance = projectInformation.EducationPerformance;
            AdditionalInformation = new AdditionalInformationViewModel
            {
                AdditionalInformation = projectInformation.Project.KeyStage5PerformanceAdditionalInformation,
                HintText =
                    "This information will populate in your HTB template under the key stage performance tables section.",
                Urn = projectInformation.Project.Urn,
                AddOrEditAdditionalInformation = addOrEditAdditionalInformation
            };
        }
    }
}