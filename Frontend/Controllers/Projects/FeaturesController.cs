using System.Net;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Helpers;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{urn}/features")]
    public class FeaturesController : Controller
    {
        private readonly IProjects _projectsRepository;

        public FeaturesController(IProjects projectsRepository)
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

            var projectResult = project.Result;
            var model = new FeaturesViewModel
            {
                Urn = projectResult.Urn,
                IsSubjectToRddOrEsfaIntervention = projectResult.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention,
                TypeOfTransfer = projectResult.Features.TypeOfTransfer,
                OtherTypeOfTransfer = projectResult.Features.OtherTypeOfTransfer,
                OutgoingAcademyUrn = projectResult.OutgoingAcademyUrn,
                WhoInitiatedTheTransfer = projectResult.Features.WhoInitiatedTheTransfer,
                InterventionDetails = projectResult.Features.ReasonForTransfer.InterventionDetails

            };
            return View(model);
        }

        [AcceptVerbs(WebRequestMethods.Http.Get)]
        [Route("initiated")]
        public async Task<IActionResult> Initiated(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            var model = new FeaturesInitiatedViewModel
            {
                Urn = projectResult.Urn,
                OutgoingAcademyName = projectResult.OutgoingAcademyName,
                WhoInitiated  = projectResult.Features.WhoInitiatedTheTransfer,
                ReturnToPreview = returnToPreview,                
            };

            return View(model);
        }

        [ActionName("Initiated")]
        [Route("initiated")]
        [AcceptVerbs(WebRequestMethods.Http.Post)]
        public async Task<IActionResult> InitiatedPost(FeaturesInitiatedViewModel vm)
        {
            var urn = vm.Urn;
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            project.Result.Features.WhoInitiatedTheTransfer = vm.WhoInitiated;

            var result = await _projectsRepository.Update(project.Result);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            if (vm.ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { id = urn });
            }

            return RedirectToAction("Index", new { urn });
        }

        [Route("reason")]
        [AcceptVerbs(WebRequestMethods.Http.Get)]
        public async Task<IActionResult> Reason(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            var vm = new FeaturesReasonViewModel
            {
                Urn = projectResult.Urn,
                OutgoingAcademyName = projectResult.OutgoingAcademyName,
                IsSubjectToIntervention = projectResult.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention,
                ReturnToPreview = returnToPreview,
                MoreDetail = projectResult.Features.ReasonForTransfer.InterventionDetails
            };
            return View(vm);
        }

        [Route("reason")]
        [ActionName("Reason")]
        [AcceptVerbs(WebRequestMethods.Http.Post)]
        public async Task<IActionResult> ReasonPost(FeaturesReasonViewModel vm)
        {
            var urn = vm.Urn;
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            project.Result.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention = vm.IsSubjectToIntervention;
            project.Result.Features.ReasonForTransfer.InterventionDetails = vm.MoreDetail ?? string.Empty;

            var result = await _projectsRepository.Update(project.Result);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            if (vm.ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { id = urn });
            }

            return RedirectToAction("Index", new { urn });
        }

        [Route("type")]
        [AcceptVerbs(WebRequestMethods.Http.Get)]
        public async Task<IActionResult> Type(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            var vm = new FeaturesTypeViewModel
            {
                Urn = projectResult.Urn,
                OutgoingAcademyName = projectResult.OutgoingAcademyName,
                ReturnToPreview = returnToPreview,
                TypeOfTransfer = projectResult.Features.TypeOfTransfer,
                OtherType = projectResult.Features.OtherTypeOfTransfer
            };
            return View(vm);
        }

        [Route("type")]
        [ActionName("Type")]
        [AcceptVerbs(WebRequestMethods.Http.Post)]
        public async Task<IActionResult> TypePost(FeaturesTypeViewModel vm)
        {
            var urn = vm.Urn;
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
            projectResult.Features.TypeOfTransfer = vm.TypeOfTransfer;
            projectResult.Features.OtherTypeOfTransfer = vm.OtherType;

            var result = await _projectsRepository.Update(projectResult);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            if (vm.ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { id = urn });
            }

            return RedirectToAction("Index", new { urn });
        }
    }
}