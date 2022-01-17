using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.TaskList.KeyStage4Performance
{
    public class KeyStage4Performance : ProjectPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectRepository;
        public string ProjectUrn => Project.Urn;
        public string OutgoingAcademyUrn => TransferringAcademy.Urn;
        public AdditionalInformationViewModel AdditionalInformation { get; private set; }
        public bool ReturnToPreview { get; private set; }

        public KeyStage4Performance(IGetInformationForProject getInformationForProject, IProjects projectRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectRepository = projectRepository;
        }

        public async Task<IActionResult> OnGetAsync(string id, bool addOrEditAdditionalInformation = false,
            bool returnToPreview = false)
        {
            var projectInformation = await _getInformationForProject.Execute(id);
            
            BuildPageModel(projectInformation, addOrEditAdditionalInformation, returnToPreview);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id, string additionalInformation, bool returnToPreview)
        {
            var project = await _projectRepository.GetByUrn(id);
            
            project.Result.KeyStage4PerformanceAdditionalInformation = additionalInformation;
            await _projectRepository.Update(project.Result);
            
            if (returnToPreview)
            {
                return new RedirectToPageResult(
                    Links.HeadteacherBoard.Preview.PageName,
                    new {id}
                );
            }

            return new RedirectToPageResult(nameof(KeyStage4Performance),
                "OnGetAsync",
                new {id},
                "additional-information-hint");
        }

        private void BuildPageModel(GetInformationForProjectResponse projectInformation,
            bool addOrEditAdditionalInformation, bool returnToPreview)
        {
            Project = projectInformation.Project;
            TransferringAcademy = projectInformation.OutgoingAcademy;
            EducationPerformance = projectInformation.EducationPerformance;
            ReturnToPreview = returnToPreview;
            AdditionalInformation = new AdditionalInformationViewModel
            {
                AdditionalInformation = projectInformation.Project.KeyStage4PerformanceAdditionalInformation,
                HintText =
                    "If you add comments, they'll be included in the key stage 4 performance tables section of your project template.",
                Urn = projectInformation.Project.Urn,
                AddOrEditAdditionalInformation = addOrEditAdditionalInformation,
                ReturnToPreview = returnToPreview
            };
        }
    }
}