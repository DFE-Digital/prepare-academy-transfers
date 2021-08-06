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
    [Route("/project/{id}/latest-ofsted-judgement")]
    public class LatestOfstedJudgementController : Controller
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;

        public LatestOfstedJudgementController(IGetInformationForProject getInformationForProject,
            IProjects projectsRepository)
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

            var model = new LatestOfstedJudgementViewModel
            {
                Project = projectInformation.Project,
                Academy = projectInformation.OutgoingAcademy,
                ReturnToPreview = returnToPreview,
                AdditionalInformationModel = new AdditionalInformationViewModel
                {
                    AdditionalInformation = projectInformation.Project.LatestOfstedJudgementAdditionalInformation,
                    HintText =
                        "This information will populate into your HTB template under the latest ofsted judgement section.",
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

            model.Result.LatestOfstedJudgementAdditionalInformation = additionalInformation;
            var result = await _projectsRepository.Update(model.Result);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            if (returnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id});
            }

            return RedirectToAction(nameof(this.Index),
                "LatestOfstedJudgement",
                new {id},
                "additional-information-hint");
        }
    }
}