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
    [Route("/project/{id}/latest-ofsted-judgement")]
    public class LatestOfstedJudgementController : Controller
    {
        private readonly IProjectsRepository _dynamicsProjectsRepository;
        private readonly IAcademiesRepository _dynamicsAcademiesRepository;
        private readonly IAcademies _academiesRepository;

        public LatestOfstedJudgementController(IProjectsRepository dynamicsProjectsRepository,
            IAcademiesRepository dynamicsAcademiesRepository, IAcademies academiesRepository)
        {
            _dynamicsProjectsRepository = dynamicsProjectsRepository;
            _dynamicsAcademiesRepository = dynamicsAcademiesRepository;
            _academiesRepository = academiesRepository;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var projectResult = await _dynamicsProjectsRepository.GetProjectById(id);
            var dynamicsAcademiesResult =
                await _dynamicsAcademiesRepository.GetAcademyById(projectResult.Result.ProjectAcademies[0].AcademyId);
            var academyResult = await _academiesRepository.GetAcademyByUkprn(dynamicsAcademiesResult.Result.Ukprn);

            var model = new LatestOfstedJudgementViewModel()
            {
                Project = projectResult.Result,
                Academy = academyResult.Result
            };

            return View(model);
        }
    }
}