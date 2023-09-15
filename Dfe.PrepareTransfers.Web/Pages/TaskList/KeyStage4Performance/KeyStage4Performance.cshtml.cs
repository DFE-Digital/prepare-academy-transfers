using System.Linq;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Dfe.PrepareTransfers.Web.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.TaskList.KeyStage4Performance
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
        public string AcademyName { get; set; }

        #region remove
        public EducationPerformance EducationPerformance { get; set; }
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
            var academy = project.Result.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == AcademyUkprn);
            academy.KeyStage4PerformanceAdditionalInformation = AdditionalInformationViewModel.AdditionalInformation;
            await _projectRepository.UpdateAcademy(project.Result.Urn, academy);

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
            var academy = projectInformation.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn);
            EducationPerformance = academy.EducationPerformance;
            OutgoingAcademyUrn = academy.Urn;
            AcademyName = academy.Name;
            AdditionalInformationViewModel = new AdditionalInformationViewModel
            {
                AdditionalInformation = academy.EducationPerformance.KeyStage4AdditionalInformation,
                HintText =
                    "This information will go into your project template under the key stage performance section.",
                Urn = projectInformation.Project.Urn,
                AddOrEditAdditionalInformation = AddOrEditAdditionalInformation,
                ReturnToPreview = ReturnToPreview
            };
        }
    }
}