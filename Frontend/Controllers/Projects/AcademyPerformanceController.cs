using System;
using System.Threading.Tasks;
using API.Models.Upstream.Response;
using API.Repositories;
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
        private readonly IAcademiesRepository _dynamicsAcademiesRepository;

        public AcademyPerformanceController(IProjectsRepository projectRepository, IAcademies academiesRepository,
            IAcademiesRepository dynamicsAcademiesRepository)
        {
            _projectRepository = projectRepository;
            _academiesRepository = academiesRepository;
            _dynamicsAcademiesRepository = dynamicsAcademiesRepository;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var getProjectResult = await _projectRepository.GetProjectById(id);
            var outgoingDynamicsAcademyResult =
                await _dynamicsAcademiesRepository.GetAcademyById(getProjectResult.Result.ProjectAcademies[0]
                    .AcademyId);
            var outgoingDynamicsAcademy = outgoingDynamicsAcademyResult.Result;

            var outgoingAcademyResult = await _academiesRepository.GetAcademyByUkprn(outgoingDynamicsAcademy.Ukprn);
            var outgoingAcademy = outgoingAcademyResult.Result;

            if (outgoingDynamicsAcademy.OfstedInspectionDate.HasValue)
            {
                outgoingAcademy.Performance.OfstedJudgementDate =
                    outgoingDynamicsAcademy.OfstedInspectionDate.Value.ToString("d MMMM yyyy");
            }

            if (!string.IsNullOrEmpty(outgoingDynamicsAcademy.EstablishmentType))
            {
                outgoingAcademy.Performance.SchoolType = outgoingDynamicsAcademy.EstablishmentType;
            }

            var model = new AcademyPerformanceViewModel
            {
                Project = getProjectResult.Result,
                OutgoingAcademy = outgoingAcademy
            };

            return View(model);
        }
    }
}