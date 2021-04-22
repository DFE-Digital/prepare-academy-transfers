using System;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using API.Repositories.Interfaces;
using Data;
using Frontend.Models;
using Frontend.Services;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    [Authorize]
    [Route("project/{id}/headteacher-board")]
    public class HeadteacherBoardController : Controller
    {
        private readonly IProjectsRepository _projectsRepository;
        private readonly ICreateHtbDocument _createHtbDocument;
        private readonly IAcademiesRepository _dynamicsAcademiesRepository;
        private readonly IAcademies _academiesRepository;

        public HeadteacherBoardController(IProjectsRepository projectsRepository, ICreateHtbDocument createHtbDocument,
            IAcademiesRepository dynamicsAcademiesRepository, IAcademies academiesRepository)
        {
            _projectsRepository = projectsRepository;
            _createHtbDocument = createHtbDocument;
            _dynamicsAcademiesRepository = dynamicsAcademiesRepository;
            _academiesRepository = academiesRepository;
        }

        [Route("preview")]
        public async Task<IActionResult> Preview([FromRoute] Guid id)
        {
            var projectResult = await _projectsRepository.GetProjectById(id);
            var projectAcademy = projectResult.Result.ProjectAcademies.First();
            var dynamicsAcademyResult = await _dynamicsAcademiesRepository.GetAcademyById(projectAcademy.AcademyId);
            var dynamicsAcademy = dynamicsAcademyResult.Result;
            var academyResult = await _academiesRepository.GetAcademyByUkprn(dynamicsAcademy.Ukprn);

            var model = new HeadTeacherBoardPreviewViewModel
            {
                Project = projectResult.Result,
                OutgoingAcademy = academyResult.Result
            };

            ViewData["ProjectId"] = id;
            ViewData["AcademyName"] = projectAcademy.AcademyName;

            return View(model);
        }

        [Route("download")]
        public IActionResult Download([FromRoute] Guid id)
        {
            ViewData["ProjectId"] = id;
            return View();
        }

        public async Task<IActionResult> GenerateDocument([FromRoute] Guid id)
        {
            var documentArray = await _createHtbDocument.Execute(id);

            return File(documentArray.ToArray(),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "Test.docx");
        }
    }
}