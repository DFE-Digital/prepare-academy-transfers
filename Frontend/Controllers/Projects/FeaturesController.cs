using System.Net;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
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

            var model = new FeaturesViewModel
            {
                Project = project.Result
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

            var model = new FeaturesViewModel
            {
                Project = project.Result,
                ReturnToPreview = returnToPreview
            };
            return View(model);
        }

        [Route("type")]
        [ActionName("Type")]
        [AcceptVerbs(WebRequestMethods.Http.Post)]
        public async Task<IActionResult> TypePost(string urn, TransferFeatures.TransferTypes typeOfTransfer,
            string otherType, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new FeaturesViewModel
            {
                Project = project.Result,
                ReturnToPreview = returnToPreview
            };

            model.Project.Features.TypeOfTransfer = typeOfTransfer;
            model.Project.Features.OtherTypeOfTransfer = otherType;

            if (typeOfTransfer == TransferFeatures.TransferTypes.Empty)
            {
                model.FormErrors.AddError(TransferFeatures.TransferTypes.SatClosure.ToString(), "typeOfTransfer",
                    "Select the type of transfer");
                return View(model);
            }

            if (typeOfTransfer == TransferFeatures.TransferTypes.Other && string.IsNullOrEmpty(otherType))
            {
                model.FormErrors.AddError("otherType", "otherType", "Enter the type of transfer");
                return View(model);
            }

            var result = await _projectsRepository.Update(model.Project);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            if (returnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { id = urn });
            }

            return RedirectToAction("Index", new { urn });
        }
    }
}