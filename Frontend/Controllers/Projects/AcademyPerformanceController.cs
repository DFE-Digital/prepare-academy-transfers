using System;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{id}/academy-performance")]
    public class AcademyPerformanceController : Controller
    {
        private readonly IGetInformationForProject _getInformationForProject;

        public AcademyPerformanceController(IGetInformationForProject getInformationForProject)
        {
            _getInformationForProject = getInformationForProject;
        }

        public async Task<IActionResult> Index(string id)
        {
            var projectInformation = await _getInformationForProject.Execute(id);

            var model = new AcademyPerformanceViewModel
            {
                Project = projectInformation.Project,
                OutgoingAcademy = projectInformation.OutgoingAcademy
            };

            return View(model);
        }
    }
}