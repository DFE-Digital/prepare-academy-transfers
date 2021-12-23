using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.Rationale;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{urn}/rationale")]
    public class RationaleController : Controller
    {
        private readonly IProjects _projectsRepository;

        public RationaleController(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> Index(string urn)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;

            var vm = new RationaleSummaryViewModel
            {
                Urn = projectResult.Urn,
                OutgoingAcademyUrn = projectResult.OutgoingAcademyUrn,
                ProjectRationale = projectResult.Rationale?.Project,
                TrustRationale = projectResult.Rationale?.Trust
            };

            return View(vm);
        }
    }
}