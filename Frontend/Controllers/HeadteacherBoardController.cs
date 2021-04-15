using System;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories.Interfaces;
using Frontend.Services;
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

        public HeadteacherBoardController(IProjectsRepository projectsRepository, ICreateHtbDocument createHtbDocument)
        {
            _projectsRepository = projectsRepository;
            _createHtbDocument = createHtbDocument;
        }

        [Route("preview")]
        public async Task<IActionResult> Preview([FromRoute] Guid id)
        {
            var projectResult = await _projectsRepository.GetProjectById(id);
            var projectAcademy = projectResult.Result.ProjectAcademies.First();

            ViewData["ProjectId"] = id;
            ViewData["AcademyName"] = projectAcademy.AcademyName;

            return View();
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