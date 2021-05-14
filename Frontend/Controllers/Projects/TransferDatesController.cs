using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{urn}/transfer-dates")]
    public class TransferDatesController : Controller
    {
        private readonly IProjects _projectsRepository;

        public TransferDatesController(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> Index(string urn)
        {
            var model = await GetModel(urn);
            return View(model);
        }

        private async Task<TransferDatesViewModel> GetModel(string urn)
        {
            var project = await _projectsRepository.GetByUrn(urn);

            var model = new TransferDatesViewModel
            {
                Project = project.Result
            };
            
            return model;
        }
    }
}