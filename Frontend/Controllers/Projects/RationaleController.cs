using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.Rationale;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{urn}/rationale")]
    public class RationaleController : Controller
    {
        private readonly IProjects _projectsRepository;

        public RationaleController(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> Index(string urn)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new RationaleViewModel
            {
                Project = project.Result
            };

            return View(model);
        }

        [HttpGet("rationale")]
        public async Task<IActionResult> Project(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            var vm = new RationaleProjectViewModel
            {
                Urn = projectResult.Urn,
                OutgoingAcademyName = projectResult.OutgoingAcademyName,
                ReturnToPreview = returnToPreview,
                ProjectRationale = projectResult.Rationale.Project
            };
            
            return View(vm);
        }

        [ActionName("Project")]
        [HttpPost("rationale")]
        public async Task<IActionResult> ProjectPost(RationaleProjectViewModel vm)
        {
            var project = await _projectsRepository.GetByUrn(vm.Urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }
            
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            
            var projectResult = project.Result;
            projectResult.Rationale.Project = vm.ProjectRationale;

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

        [HttpGet("trust-or-sponsor")]
        public async Task<IActionResult> TrustOrSponsor(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            var vm = new RationaleTrustOrSponsorViewModel
            {
                Urn = projectResult.Urn,
                OutgoingAcademyName = projectResult.OutgoingAcademyName,
                ReturnToPreview = returnToPreview,
                TrustOrSponsorRationale = projectResult.Rationale.Trust
            };

            return View(vm);
        }

        [ActionName("TrustOrSponsor")]
        [HttpPost("trust-or-sponsor")]
        public async Task<IActionResult> TrustOrSponsorPost(RationaleTrustOrSponsorViewModel vm)
        {
            var project = await _projectsRepository.GetByUrn(vm.Urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            
            var projectResult = project.Result;
            projectResult.Rationale.Trust = vm.TrustOrSponsorRationale;

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