using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.TaskList.KeyStage5Performance
{
    public class KeyStage5Performance : ProjectPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projects;
        public string ProjectUrn => Project.Urn;
        public string OutgoingAcademyUrn => TransferringAcademy.Urn;
        public AdditionalInformationViewModel AdditionalInformation { get; set; }
        public bool ReturnToPreview { get; set; }

        public KeyStage5Performance(IGetInformationForProject getInformationForProject, IProjects projects)
        {
            _getInformationForProject = getInformationForProject;
            _projects = projects;
        }

        public async Task<IActionResult> OnGetAsync(string id, bool addOrEditAdditionalInformation = false,
            bool returnToPreview = false)
        {
            var projectInformation = await _getInformationForProject.Execute(id);
            
            PopulateModel(addOrEditAdditionalInformation, projectInformation, returnToPreview);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id, string additionalInformation, bool returnToPreview)
        {
            var project = await _projects.GetByUrn(id);
            
            project.Result.KeyStage5PerformanceAdditionalInformation = additionalInformation;
            await _projects.Update(project.Result);

            if (returnToPreview)
            {
                return new RedirectToPageResult(
                    Links.HeadteacherBoard.Preview.PageName,
                    new {id}
                );
            }

            return new RedirectToPageResult(nameof(KeyStage5Performance),
                "OnGetAsync",
                new {id},
                "additional-information-hint");
        }

        private void PopulateModel(bool addOrEditAdditionalInformation,
            GetInformationForProjectResponse projectInformation, bool returnToPreview)
        {
            Project = projectInformation.Project;
            TransferringAcademy = projectInformation.OutgoingAcademy;
            EducationPerformance = projectInformation.EducationPerformance;
            ReturnToPreview = returnToPreview;
            AdditionalInformation = new AdditionalInformationViewModel
            {
                AdditionalInformation = projectInformation.Project.KeyStage5PerformanceAdditionalInformation,
                HintText =
                    "If you add comments, they'll be included in the key stage 5 performance tables section of your project template.",
                Urn = projectInformation.Project.Urn,
                AddOrEditAdditionalInformation = addOrEditAdditionalInformation,
                ReturnToPreview = returnToPreview
            };
        }
    }
}