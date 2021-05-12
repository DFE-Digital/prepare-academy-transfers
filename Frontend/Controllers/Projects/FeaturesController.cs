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
            var model = await GetModel(urn);
            return View(model);
        }

        [AcceptVerbs(WebRequestMethods.Http.Get)]
        [Route("initiated")]
        public async Task<IActionResult> Initiated(string urn)
        {
            var model = await GetModel(urn);
            return View(model);
        }

        [ActionName("Initiated")]
        [Route("initiated")]
        [AcceptVerbs(WebRequestMethods.Http.Post)]
        public async Task<IActionResult> InitiatedPost(string urn, TransferFeatures.ProjectInitiators whoInitiated)
        {
            var model = await GetModel(urn);

            if (whoInitiated == TransferFeatures.ProjectInitiators.Empty)
            {
                model.FormErrors.AddError(TransferFeatures.ProjectInitiators.Dfe.ToString(), "whoInitiated",
                    "Please select who initiated the project");
                return View(model);
            }

            model.Project.Features.WhoInitiatedTheTransfer = whoInitiated;

            await _projectsRepository.Update(model.Project);

            return RedirectToAction("Index", new {urn});
        }

        [Route("reason")]
        [AcceptVerbs(WebRequestMethods.Http.Get)]
        public async Task<IActionResult> Reason(string urn)
        {
            var model = await GetModel(urn);
            return View(model);
        }

        [Route("reason")]
        [ActionName("Reason")]
        [AcceptVerbs(WebRequestMethods.Http.Post)]
        public async Task<IActionResult> ReasonPost(string urn, bool? isSubjectToIntervention, string moreDetail)
        {
            var model = await GetModel(urn);

            if (!isSubjectToIntervention.HasValue)
            {
                model.FormErrors.AddError("True", "isSubjectToIntervention",
                    "Select whether or not the transfer is subject to intervention");
                return View(model);
            }

            model.Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention = isSubjectToIntervention;
            model.Project.Features.ReasonForTransfer.InterventionDetails = moreDetail;
            await _projectsRepository.Update(model.Project);

            return RedirectToAction("Index", new {urn});
        }

        private async Task<FeaturesViewModel> GetModel(string urn)
        {
            var project = await _projectsRepository.GetByUrn(urn);

            var model = new FeaturesViewModel
            {
                Project = project.Result
            };
            return model;
        }
    }
}