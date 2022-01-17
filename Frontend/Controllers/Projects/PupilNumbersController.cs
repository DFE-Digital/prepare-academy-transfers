using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("/project/{id}/pupil-number")]
    public class PupilNumbersController : Controller
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;

        public PupilNumbersController(IGetInformationForProject getInformationForProject, IProjects projectsRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectsRepository = projectsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id, bool addOrEditAdditionalInformation = false,
            bool returnToPreview = false)
        {
            var projectInformation = await _getInformationForProject.Execute(id);

            var model = BuildViewModel(projectInformation, returnToPreview, false, addOrEditAdditionalInformation);

            return View(model);
        }

        public static PupilNumbersViewModel BuildViewModel(GetInformationForProjectResponse projectInformation,
            bool returnToPreview, bool isPreview = false, bool addOrEditAdditionalInformation = false)
        {
            var model = new PupilNumbersViewModel
            {
                Urn = projectInformation.Project.Urn,
                ReturnToPreview = returnToPreview,
                IsPreview = isPreview,
                GirlsOnRoll = projectInformation.OutgoingAcademy.PupilNumbers.GirlsOnRoll,
                BoysOnRoll = projectInformation.OutgoingAcademy.PupilNumbers.BoysOnRoll,
                WithStatementOfSEN = projectInformation.OutgoingAcademy.PupilNumbers.WithStatementOfSen,
                WithEAL = projectInformation.OutgoingAcademy.PupilNumbers.WhoseFirstLanguageIsNotEnglish,
                FreeSchoolMealsLast6Years = projectInformation.OutgoingAcademy.PupilNumbers
                    .PercentageEligibleForFreeSchoolMealsDuringLast6Years,
                OutgoingAcademyUrn = projectInformation.OutgoingAcademy.Urn,
                OutgoingAcademyName = projectInformation.OutgoingAcademy.Name,
                AdditionalInformation = new AdditionalInformationViewModel
                {
                    AdditionalInformation = projectInformation.Project.PupilNumbersAdditionalInformation,
                    HintText =
                        "If you add comments, they'll be included in the pupil numbers section of your project template.",
                    Urn = projectInformation.Project.Urn,
                    AddOrEditAdditionalInformation = addOrEditAdditionalInformation,
                    ReturnToPreview = returnToPreview
                }
            };
            return model;
        }

        [HttpPost]
        public async Task<IActionResult> Index(string id, string additionalInformation, bool returnToPreview = false)
        {
            var model = await _projectsRepository.GetByUrn(id);

            model.Result.PupilNumbersAdditionalInformation = additionalInformation;
            await _projectsRepository.Update(model.Result);
            
            if (returnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id});
            }

            return RedirectToAction(nameof(this.Index),
                "PupilNumbers",
                new {id},
                "additional-information-hint");
        }
    }
}