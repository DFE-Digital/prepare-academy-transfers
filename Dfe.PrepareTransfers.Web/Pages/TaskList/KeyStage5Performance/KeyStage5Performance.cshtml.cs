using System.Linq;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Dfe.PrepareTransfers.Web.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.TaskList.KeyStage5Performance
{
    public class KeyStage5Performance : CommonPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projects;
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

        public KeyStage5Performance(IGetInformationForProject getInformationForProject, IProjects projects)
        {
            _getInformationForProject = getInformationForProject;
            _projects = projects;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var projectInformation = await _getInformationForProject.Execute(Urn);

            PopulateModel(projectInformation);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            var academy = project.Result.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == AcademyUkprn);
            academy.KeyStage5PerformanceAdditionalInformation = AdditionalInformationViewModel.AdditionalInformation;
            await _projects.UpdateAcademy(project.Result.Urn, academy);

            if (ReturnToPreview)
            {
                return new RedirectToPageResult(
                    Links.HeadteacherBoard.Preview.PageName,
                    new {Urn}
                );
            }

            return new RedirectToPageResult(nameof(KeyStage5Performance),
                null,
                new {Urn},
                "additional-information-hint");
        }

        private void PopulateModel(GetInformationForProjectResponse projectInformation)
        {
            var academy = projectInformation.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn);
            EducationPerformance = academy.EducationPerformance;
            OutgoingAcademyUrn = academy.Urn;
            AcademyName = academy.Name;
            AdditionalInformationViewModel = new AdditionalInformationViewModel
            {
                AdditionalInformation = academy.EducationPerformance.KeyStage5AdditionalInformation,
                HintText =
                    "This information will go into your project template under the key stage performance section.",
                Urn = projectInformation.Project.Urn,
                AddOrEditAdditionalInformation = AddOrEditAdditionalInformation,
                ReturnToPreview = ReturnToPreview
            };
        }
    }
}