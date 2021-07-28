using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{urn}/academy-and-trust-information")]
    public class AcademyAndTrustInformationController : Controller
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;

        public AcademyAndTrustInformationController(IProjects projectsRepository,
            IGetInformationForProject getInformationForProject)
        {
            _projectsRepository = projectsRepository;
            _getInformationForProject = getInformationForProject;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string urn)
        {
            var projectInformation = await _getInformationForProject.Execute(urn);

            if (!projectInformation.IsValid)
            {
                return View("ErrorPage", projectInformation.ResponseError.ErrorMessage);
            }

            var model = new AcademyAndTrustInformationViewModel
            {
                Project = projectInformation.Project,
                TransferringAcademy = projectInformation.OutgoingAcademy
            };

            return View(model);
        }

        [HttpGet("recommendation")]
        public async Task<IActionResult> Recommendation(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new AcademyAndTrustInformationViewModel
            {
                Project = project.Result,
                ReturnToPreview = returnToPreview
            };

            return View(model);
        }

        [HttpPost("recommendation")]
        public async Task<IActionResult> Recommendation(string urn,
            TransferAcademyAndTrustInformation.RecommendationResult recommendation, string author,
            bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new AcademyAndTrustInformationViewModel
            {
                Project = project.Result
            };

            model.Project.AcademyAndTrustInformation = new TransferAcademyAndTrustInformation
            {
                Recommendation = recommendation,
                Author = author
            };

            var result = await _projectsRepository.Update(model.Project);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            if (returnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = urn});
            }

            return RedirectToAction("Index", new {urn});
        }
    }
}