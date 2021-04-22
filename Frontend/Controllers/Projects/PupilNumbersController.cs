using System;
using System.Threading.Tasks;
using API.Repositories;
using API.Repositories.Interfaces;
using Data;
using Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("/project/{id}/pupil-number")]
    public class PupilNumbersController : Controller
    {
        private readonly IProjectsRepository _dynamicsProjectsRepository;
        private readonly IAcademiesRepository _dynamicsAcademiesRepository;
        private readonly IAcademies _academiesRepository;

        public PupilNumbersController(IProjectsRepository dynamicsProjectsRepository, IAcademies academiesRepository,
            IAcademiesRepository dynamicsAcademiesRepository)
        {
            _dynamicsProjectsRepository = dynamicsProjectsRepository;
            _academiesRepository = academiesRepository;
            _dynamicsAcademiesRepository = dynamicsAcademiesRepository;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var projectResult = await _dynamicsProjectsRepository.GetProjectById(id);
            var dynamicsAcademiesResult =
                await _dynamicsAcademiesRepository.GetAcademyById(projectResult.Result.ProjectAcademies[0].AcademyId);
            var academyResult = await _academiesRepository.GetAcademyByUkprn(dynamicsAcademiesResult.Result.Ukprn);

            var model = new PupilNumbersViewModel()
            {
                Project = projectResult.Result,
                OutgoingAcademy = academyResult.Result
            };

            return View(model);
        }
    }
}