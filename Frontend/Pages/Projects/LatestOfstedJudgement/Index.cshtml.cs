using System.Threading.Tasks;
using Data;
using Frontend.ExtensionMethods;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.LatestOfstedJudgement
{
    public class Index : CommonPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;
        
        public string SchoolName { get; set; }
        public string InspectionDate { get; set; }
        public string OverallEffectiveness { get; set; }
        public string OfstedReport { get; set; }
        
        [BindProperty]
        public AdditionalInformationViewModel AdditionalInformationViewModel { get; set; }
        public bool IsPreview { get; set; }

        public Index(IGetInformationForProject getInformationForProject,
            IProjects projectsRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectsRepository = projectsRepository;
        }
        
        public async Task<IActionResult> OnGetAsync(bool addOrEditAdditionalInformation = false)
        {
            var projectInformation = await _getInformationForProject.Execute(Urn);
            if (!projectInformation.IsValid)
            {
                return this.View("ErrorPage", projectInformation.ResponseError.ErrorMessage);
            }

            Urn = projectInformation.Project.Urn;
            OutgoingAcademyUrn = projectInformation.Project.OutgoingAcademyUrn;
            OutgoingAcademyName = projectInformation.Project.OutgoingAcademyName;
            SchoolName = projectInformation.OutgoingAcademy.LatestOfstedJudgement.SchoolName;
            InspectionDate = projectInformation.OutgoingAcademy.LatestOfstedJudgement.InspectionDate;
            OverallEffectiveness = projectInformation.OutgoingAcademy.LatestOfstedJudgement.OverallEffectiveness;
            OfstedReport = projectInformation.OutgoingAcademy.LatestOfstedJudgement.OfstedReport;
            AdditionalInformationViewModel = new AdditionalInformationViewModel
            {
                AdditionalInformation = projectInformation.Project.LatestOfstedJudgementAdditionalInformation,
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
            if (!model.IsValid)
            {
                return this.View("ErrorPage", model.Error.ErrorMessage);
            }

            model.Result.LatestOfstedJudgementAdditionalInformation = AdditionalInformationViewModel?.AdditionalInformation;
            var result = await _projectsRepository.Update(model.Result);
            if (!result.IsValid)
            {
                return this.View("ErrorPage", result.Error.ErrorMessage);
            }

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = Urn});
            }

            return RedirectToPage("/Projects/LatestOfstedJudgement/Index", nameof(this.OnGetAsync), new { Urn }, "additional-information-hint");
        }
    }
}