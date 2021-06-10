using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("/project/{id}/pupil-number")]
    public class PupilNumbersController : Controller
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;

        public PupilNumbersController(IGetInformationForProject getInformationForProject, IProjects projectsRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectsRepository = projectsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id, bool addOrEditAdditionalInformation = false)
        {
            var projectInformation = await _getInformationForProject.Execute(id);

            var model = new PupilNumbersViewModel
            {
                Project = projectInformation.Project,
                OutgoingAcademy = projectInformation.OutgoingAcademy,
                AdditionalInformationModel = new AdditionalInformationViewModel
                {
                    AdditionalInformation = projectInformation.Project.PupilNumbersAdditionalInformation,
                    HintText = "This information will populate into your HTB template under the school pupil forecasts section.",
                    Urn = projectInformation.Project.Urn,
                    AddOrEditAdditionalInformation = addOrEditAdditionalInformation
                }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string id, string additionalInformation)
        {
            var model = await _projectsRepository.GetByUrn(id);

            model.Result.PupilNumbersAdditionalInformation = additionalInformation;
            await _projectsRepository.Update(model.Result);

            return RedirectToAction(nameof(this.Index),
                "PupilNumbers",
                new { id },
                "additional-information-hint");
        }
    }
}