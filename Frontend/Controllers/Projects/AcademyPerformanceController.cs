using System;
using System.Threading.Tasks;
using API.Models.Upstream.Response;
using API.Repositories.Interfaces;
using Data;
using Data.Models;
using Frontend.Models.AcademyPerformance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{id}/academy-performance")]
    public class AcademyPerformanceController : Controller
    {
        private readonly IProjectsRepository _projectRepository;
        private readonly IAcademies _academiesRepository;

        public AcademyPerformanceController(IProjectsRepository projectRepository, IAcademies academiesRepository)
        {
            _projectRepository = projectRepository;
            _academiesRepository = academiesRepository;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var getProjectResult = await _projectRepository.GetProjectById(id);
            var outgoingAcademy = await _academiesRepository.GetAcademyByUkprn("ukprn");

            var model = new IndexViewModel
            {
                Project = getProjectResult.Result,
                OutgoingAcademy = outgoingAcademy
            };

            return View(model);
        }
    }
}