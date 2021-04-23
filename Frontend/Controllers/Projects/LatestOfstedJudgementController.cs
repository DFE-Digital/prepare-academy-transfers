using System;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("/project/{id:guid}/latest-ofsted-judgement")]
    public class LatestOfstedJudgementController : Controller
    {
        private readonly IGetInformationForProject _getInformationForProject;

        public LatestOfstedJudgementController(IGetInformationForProject getInformationForProject)
        {
            _getInformationForProject = getInformationForProject;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var projectInformation = await _getInformationForProject.Execute(id);

            var model = new LatestOfstedJudgementViewModel
            {
                Project = projectInformation.Project,
                Academy = projectInformation.OutgoingAcademy
            };

            return View(model);
        }
    }
}