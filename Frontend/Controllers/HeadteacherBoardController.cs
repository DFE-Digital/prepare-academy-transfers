using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    [Authorize]
    [Route("project/{id}/advisory-board")]
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

        [Route("download")]
        public async Task<IActionResult> Download([FromRoute] string id)
        {
            var projectInformation = await _getInformationForProject.Execute(id);

            var model = new DownloadViewModel()
            {
                Urn = projectInformation.Project.Urn,
                OutgoingAcademyName = projectInformation.OutgoingAcademy.Name
            };

            return View(model);
        }

        public async Task<IActionResult> GenerateDocument([FromRoute] string id)
        {
            var document = await _createHtbDocument.Execute(id);

            return File(document.Document.ToArray(),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                $"ProjectTemplateFor{id}_{System.DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentUICulture)}.docx");
        }
    }
}