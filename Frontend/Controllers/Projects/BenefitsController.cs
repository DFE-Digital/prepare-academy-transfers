using System.Threading.Tasks;
using Data;
using Frontend.Helpers;
using Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{urn}/benefits")]
    public class BenefitsController : Controller
    {
        private readonly IProjects _projectsRepository;

        public BenefitsController(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> Index(string urn)
        {
            var model = await GetModel(urn);
            return View(model);
        }

        private async Task<BenefitsViewModel> GetModel(string urn)
        {
            var project = await _projectsRepository.GetByUrn(urn);

            var model = new BenefitsViewModel()
            {
                Project = project.Result
            };

            return model;
        }
    }
}