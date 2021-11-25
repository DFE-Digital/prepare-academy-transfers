using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.AcademyAndTrustInformation;
using Frontend.Services.Interfaces;
using Helpers;
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

            var vm = new AcademyAndTrustInformationSummaryViewModel
            {
                OutgoingAcademyName = projectInformation.OutgoingAcademy?.Name,
                Recommendation = projectInformation.Project.AcademyAndTrustInformation.Recommendation,
                Author = projectInformation.Project.AcademyAndTrustInformation.Author,
                HtbDate = projectInformation.Project.Dates?.Htb,
                ProjectName = projectInformation.Project.Name,
                IncomingTrustName = projectInformation.Project.IncomingTrustName,
                TargetDate = projectInformation.Project.Dates?.Target,
                FirstDiscussedDate = projectInformation.Project.Dates?.FirstDiscussed,
                OutgoingAcademyUrn = projectInformation.Project.OutgoingAcademyUrn,
                Urn = projectInformation.Project.Urn
            };

            return View(vm);
        }

        [HttpGet("recommendation")]
        public async Task<IActionResult> Recommendation(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            var vm = new RecommendationViewModel
            {
                Urn = projectResult.Urn,
                OutgoingAcademyName = projectResult.OutgoingAcademyName,
                ReturnToPreview = returnToPreview,
                Author = projectResult.AcademyAndTrustInformation.Author,
                Recommendation = projectResult.AcademyAndTrustInformation.Recommendation,
                OutgoingAcademyUrn = projectResult.OutgoingAcademyUrn
            };

            return View(vm);
        }

        [HttpPost("recommendation")]
        public async Task<IActionResult> Recommendation(RecommendationViewModel vm)
        {
            var project = await _projectsRepository.GetByUrn(vm.Urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            projectResult.AcademyAndTrustInformation.Recommendation = vm.Recommendation;
            projectResult.AcademyAndTrustInformation.Author = vm.Author;

            var result = await _projectsRepository.Update(projectResult);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            if (vm.ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = vm.Urn});
            }

            return RedirectToAction("Index", new {vm.Urn});
        }
    }
}