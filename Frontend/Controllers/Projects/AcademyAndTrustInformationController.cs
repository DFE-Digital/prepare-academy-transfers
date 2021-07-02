using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{urn}/academy-and-trust-information")]
    public class AcademyAndTrustInformationController : Controller
    {
        private readonly IProjects _projectsRepository;

        public AcademyAndTrustInformationController(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string urn)
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

            return View(model);
        }

        [HttpGet("recommendation")]
        public async Task<IActionResult> Recommendation(string urn)
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

            return View(model);
        }

        [HttpPost("recommendation")]
        public async Task<IActionResult> Recommendation(string urn, TransferAcademyAndTrustInformation.RecommendationResult recommendation, string author)
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

            return RedirectToAction("Index", new {urn});
        }
    }
}