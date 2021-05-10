using System;
using System.Linq;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    [Authorize]
    [Route("project/{id}/headteacher-board")]
    public class HeadteacherBoardController : Controller
    {
        private readonly ICreateHtbDocument _createHtbDocument;
        private readonly IGetInformationForProject _getInformationForProject;

        public HeadteacherBoardController(ICreateHtbDocument createHtbDocument,
            IGetInformationForProject getInformationForProject)
        {
            _createHtbDocument = createHtbDocument;
            _getInformationForProject = getInformationForProject;
        }

        [Route("preview")]
        public async Task<IActionResult> Preview([FromRoute] string id)
        {
            var projectInformation = await _getInformationForProject.Execute(id);

            var model = new HeadTeacherBoardPreviewViewModel
            {
                Project = projectInformation.Project,
                OutgoingAcademy = projectInformation.OutgoingAcademy
            };

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