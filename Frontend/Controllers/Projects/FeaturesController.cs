using System.Net;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Initiated(string urn)
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

        [ActionName("Initiated")]
        [Route("initiated")]
        [AcceptVerbs(WebRequestMethods.Http.Post)]
        public async Task<IActionResult> InitiatedPost(string urn, TransferFeatures.ProjectInitiators whoInitiated)
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

            if (whoInitiated == TransferFeatures.ProjectInitiators.Empty)
            {
                model.FormErrors.AddError(TransferFeatures.ProjectInitiators.Dfe.ToString(), "whoInitiated",
                    "Please select who initiated the project");
                return View(model);
            }

            model.Project.Features.WhoInitiatedTheTransfer = whoInitiated;

            var result = await _projectsRepository.Update(model.Project);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            return RedirectToAction("Index", new {urn});
        }

        [Route("reason")]
        [AcceptVerbs(WebRequestMethods.Http.Get)]
        public async Task<IActionResult> Reason(string urn)
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

        [Route("reason")]
        [ActionName("Reason")]
        [AcceptVerbs(WebRequestMethods.Http.Post)]
        public async Task<IActionResult> ReasonPost(string urn, bool? isSubjectToIntervention, string moreDetail)
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

            if (!isSubjectToIntervention.HasValue)
            {
                model.FormErrors.AddError("True", "isSubjectToIntervention",
                    "Select whether or not the transfer is subject to intervention");
                return View(model);
            }

            model.Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention = isSubjectToIntervention;
            model.Project.Features.ReasonForTransfer.InterventionDetails = moreDetail;

            var result = await _projectsRepository.Update(model.Project);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            return RedirectToAction("Index", new {urn});
        }

        [Route("type")]
        [AcceptVerbs(WebRequestMethods.Http.Get)]
        public async Task<IActionResult> Type(string urn)
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

        [Route("type")]
        [ActionName("Type")]
        [AcceptVerbs(WebRequestMethods.Http.Post)]
        public async Task<IActionResult> TypePost(string urn, TransferFeatures.TransferTypes typeOfTransfer,
            string otherType)
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

            model.Project.Features.TypeOfTransfer = typeOfTransfer;
            model.Project.Features.OtherTypeOfTransfer = otherType;

            if (typeOfTransfer == TransferFeatures.TransferTypes.Empty)
            {
                model.FormErrors.AddError(TransferFeatures.TransferTypes.SatClosure.ToString(), "typeOfTransfer",
                    "Please select the type of transfer");
                return View(model);
            }

            if (typeOfTransfer == TransferFeatures.TransferTypes.Other && string.IsNullOrEmpty(otherType))
            {
                model.FormErrors.AddError("otherType", "otherType", "Please enter the type of transfer");
                return View(model);
            }

            var result = await _projectsRepository.Update(model.Project);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            return RedirectToAction("Index", new { urn });
        }
    }
}