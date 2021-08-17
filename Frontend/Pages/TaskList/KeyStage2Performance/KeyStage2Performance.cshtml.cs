using System.Threading.Tasks;
using Data;
using Frontend.ExtensionMethods;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.TaskList.KeyStage2Performance
{
    public class KeyStage2Performance : ProjectPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectRepository;
        public string ProjectUrn => Project.Urn;
        public string OutgoingAcademyUrn => TransferringAcademy.Urn;
        public AdditionalInformationViewModel AdditionalInformation { get; private set; }
        public bool ReturnToPreview { get; private set; }

        public KeyStage2Performance(IGetInformationForProject getInformationForProject, IProjects projectRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectRepository = projectRepository;
        }

        public async Task<IActionResult> OnGetAsync(string id, bool addOrEditAdditionalInformation = false,
            bool returnToPreview = false)
        {
            var projectInformation = await _getInformationForProject.Execute(id);

            if (!projectInformation.IsValid)
            {
                return this.View("ErrorPage", projectInformation.ResponseError.ErrorMessage);
            }

            BuildPageModel(projectInformation, addOrEditAdditionalInformation, returnToPreview);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id, string additionalInformation, bool returnToPreview)
        {
            var project = await _projectRepository.GetByUrn(id);

            if (!project.IsValid)
            {
                return this.View("ErrorPage", project.Error.ErrorMessage);
            }

            project.Result.KeyStage2PerformanceAdditionalInformation = additionalInformation;
            await _projectRepository.Update(project.Result);

            if (returnToPreview)
            {
                return new RedirectToPageResult(
                    Links.HeadteacherBoard.Preview.PageName,
                    new {id}
                );
            }
            
            return new RedirectToPageResult(nameof(KeyStage2Performance),
                "OnGetAsync",
                new {id},
                "additional-information-hint");
        }

        private void BuildPageModel(GetInformationForProjectResponse projectInformation,
            bool addOrEditAdditionalInformation,
            bool returnToPreview)
        {
            Project = projectInformation.Project;
            TransferringAcademy = projectInformation.OutgoingAcademy;
            EducationPerformance = projectInformation.EducationPerformance;
            ReturnToPreview = returnToPreview;
            AdditionalInformation = new AdditionalInformationViewModel
            {
                AdditionalInformation = projectInformation.Project.KeyStage2PerformanceAdditionalInformation,
                HintText =
                    "This information will populate in your HTB template under the key stage 2 performance tables section.",
                Urn = projectInformation.Project.Urn,
                AddOrEditAdditionalInformation = addOrEditAdditionalInformation,
                ReturnToPreview = returnToPreview
            };
        }
    }
}