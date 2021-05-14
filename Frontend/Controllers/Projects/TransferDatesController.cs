using System.Threading.Tasks;
using Data;
using Frontend.Helpers;
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

        [HttpGet("first-discussed")]
        public async Task<IActionResult> FirstDiscussed(string urn)
        {
            var model = await GetModel(urn);
            return View(model);
        }

        [HttpPost("first-discussed")]
        [ActionName("FirstDiscussed")]
        public async Task<IActionResult> FirstDiscussedPost(string urn, string day, string month, string year)
        {
            var model = await GetModel(urn);
            model.Project.TransferDates.FirstDiscussed = DatesHelper.DayMonthYearToDateString(day, month, year);
            await _projectsRepository.Update(model.Project);
            return RedirectToAction("Index", new {urn});
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