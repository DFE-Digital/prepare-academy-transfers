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

            model.Project.Features.WhoInitiatedTheTransfer = whoInitiated;

            await _projectsRepository.Update(model.Project);

            return View(model);
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