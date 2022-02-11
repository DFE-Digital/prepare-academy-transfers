using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.TaskList.KeyStage4Performance
{
    public class KeyStage4Performance : CommonPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectRepository;
        
        [BindProperty]
        public AdditionalInformationViewModel AdditionalInformationViewModel { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public bool AddOrEditAdditionalInformation { get; set; }
        [BindProperty(SupportsGet = true)]
        public string AcademyUkprn { get; set; }
        
        //todo: remove data models here
        #region remove
        public EducationPerformance EducationPerformance { get; set; }
        public Academy TransferringAcademy { get; set; }
        #endregion
        
        public KeyStage4Performance(IGetInformationForProject getInformationForProject, IProjects projectRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectRepository = projectRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var projectInformation = await _getInformationForProject.Execute(Urn);

            BuildPageModel(projectInformation);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projectRepository.GetByUrn(Urn);
            
            project.Result.KeyStage4PerformanceAdditionalInformation = AdditionalInformationViewModel.AdditionalInformation;
            await _projectRepository.Update(project.Result);
            
            if (ReturnToPreview)
            {
                return new RedirectToPageResult(
                    Links.HeadteacherBoard.Preview.PageName,
                    new {Urn}
                );
            }

            return new RedirectToPageResult(nameof(KeyStage4Performance),
                null,
                new {Urn},
                "additional-information-hint");
        }

        private void BuildPageModel(GetInformationForProjectResponse projectInformation)
        {
            TransferringAcademy = projectInformation.OutgoingAcademy;
            EducationPerformance = projectInformation.EducationPerformance;
            OutgoingAcademyUrn = projectInformation.OutgoingAcademy.Urn;
            AdditionalInformationViewModel = new AdditionalInformationViewModel
            {
                AdditionalInformation = projectInformation.Project.KeyStage4PerformanceAdditionalInformation,
                HintText =
                    "If you add comments, they'll be included in the key stage 4 performance tables section of your project template.",
                Urn = projectInformation.Project.Urn,
                AddOrEditAdditionalInformation = AddOrEditAdditionalInformation,
                ReturnToPreview = ReturnToPreview
            };
        }
        
    }
}