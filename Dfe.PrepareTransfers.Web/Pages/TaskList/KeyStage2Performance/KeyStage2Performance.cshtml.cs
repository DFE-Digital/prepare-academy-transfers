using System.Linq;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.TaskList.KeyStage2Performance
{
    public class KeyStage2Performance : CommonPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectRepository;

        [BindProperty]
        public AdditionalInformationViewModel AdditionalInformationViewModel { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public bool AddOrEditAdditionalInformation { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string AcademyUkprn { get; set; }
        public string AcademyName { get; set; }
        
        //todo: remove data models here
        #region remove
        public EducationPerformance EducationPerformance { get; set; }
        #endregion
        public KeyStage2Performance(IGetInformationForProject getInformationForProject, IProjects projectRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectRepository = projectRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await BuildPageModel();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projectRepository.GetByUrn(Urn);
            var academy = project.Result.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == AcademyUkprn);
            academy.KeyStage2PerformanceAdditionalInformation = AdditionalInformationViewModel.AdditionalInformation;
            await _projectRepository.Update(project.Result);

            if (ReturnToPreview)
            {
                return new RedirectToPageResult(
                    Links.HeadteacherBoard.Preview.PageName,
                    new {Urn}
                );
            }
            
            return new RedirectToPageResult(nameof(KeyStage2Performance),
                null,
                new {Urn},
                "additional-information-hint");
        }

        private async Task BuildPageModel()
        {
            var projectInformation = await _getInformationForProject.Execute(Urn);
            var academy = projectInformation.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn);
            AcademyName = academy.Name;
            OutgoingAcademyUrn = academy.Urn;
            EducationPerformance = academy.EducationPerformance;
            AdditionalInformationViewModel = new AdditionalInformationViewModel
            {
                AdditionalInformation = academy.EducationPerformance.KeyStage2AdditionalInformation,
                HintText =
                    "This information will go into your project template under the key stage performance section.",
                Urn = projectInformation.Project.Urn,
                AddOrEditAdditionalInformation = AddOrEditAdditionalInformation,
                ReturnToPreview = ReturnToPreview
            };
        }

    }
}