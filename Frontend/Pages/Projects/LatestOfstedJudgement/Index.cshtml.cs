using System.Linq;
using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.LatestOfstedJudgement
{
    public class Index : CommonPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;

        public Data.Models.Academies.LatestOfstedJudgement LatestOfstedJudgement { get; set; }

        [BindProperty]
        public AdditionalInformationViewModel AdditionalInformationViewModel { get; set; }
        public bool IsPreview { get; set; }

        [BindProperty(SupportsGet = true)]
        public string AcademyUkprn { get; set; }

        public Index(IGetInformationForProject getInformationForProject,
            IProjects projectsRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync(bool addOrEditAdditionalInformation = false)
        {
            var projectInformation = await _getInformationForProject.Execute(Urn);
            var academy = projectInformation.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn);
            LatestOfstedJudgement = academy.LatestOfstedJudgement;

            OutgoingAcademyUrn = projectInformation.Project.OutgoingAcademyUrn;
            ProjectReference = projectInformation.Project.Reference;

            AdditionalInformationViewModel = new AdditionalInformationViewModel
            {
                AdditionalInformation = LatestOfstedJudgement.AdditionalInformation,
                HintText =
                        "If you add comments, they'll be included in the latest Ofsted judgement section of your project template.",
                Urn = projectInformation.Project.Urn,
                AddOrEditAdditionalInformation = addOrEditAdditionalInformation,
                ReturnToPreview = ReturnToPreview
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var model = await _projectsRepository.GetByUrn(Urn);
            var academy = model.Result.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == AcademyUkprn);
            academy.LatestOfstedReportAdditionalInformation = AdditionalInformationViewModel?.AdditionalInformation;
            await _projectsRepository.Update(model.Result);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }

            return RedirectToPage("/Projects/LatestOfstedJudgement/Index", null, new { Urn }, "additional-information-hint");
        }


    }
}