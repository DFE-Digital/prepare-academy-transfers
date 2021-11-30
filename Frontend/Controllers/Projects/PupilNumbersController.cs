using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
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
            if (!projectInformation.IsValid)
            {
                return View("ErrorPage", projectInformation.ResponseError.ErrorMessage);
            }

            var model = new PupilNumbersViewModel
            {
                Urn = projectInformation.Project.Urn,
                ReturnToPreview = returnToPreview,
                GirlsOnRoll = projectInformation.OutgoingAcademy.PupilNumbers.GirlsOnRoll,
                BoysOnRoll = projectInformation.OutgoingAcademy.PupilNumbers.BoysOnRoll,
                WithStatementOfSEN = projectInformation.OutgoingAcademy.PupilNumbers.WithStatementOfSen,
                WithEAL = projectInformation.OutgoingAcademy.PupilNumbers.WhoseFirstLanguageIsNotEnglish,
                FreeSchoolMealsLast6Years = projectInformation.OutgoingAcademy.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years,
                OutgoingAcademyUrn = projectInformation.OutgoingAcademy.Urn,
                OutgoingAcademyName = projectInformation.OutgoingAcademy.Name,
                AdditionalInformationModel = new AdditionalInformationViewModel
                {
                    AdditionalInformation = projectInformation.Project.PupilNumbersAdditionalInformation,
                    HintText =
                        "If you add comments, they'll be included in the pupil numbers section of your project template.",
                    Urn = projectInformation.Project.Urn,
                    AddOrEditAdditionalInformation = addOrEditAdditionalInformation,
                    ReturnToPreview = returnToPreview
                }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string id, string additionalInformation, bool returnToPreview = false)
        {
            var model = await _projectsRepository.GetByUrn(id);
            if (!model.IsValid)
            {
                return View("ErrorPage", model.Error.ErrorMessage);
            }

            model.Result.PupilNumbersAdditionalInformation = additionalInformation;
            var result = await _projectsRepository.Update(model.Result);
            if (!result.IsValid)
            {
                return View("ErrorPage", model.Error.ErrorMessage);
            }

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