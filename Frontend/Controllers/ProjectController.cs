using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models.KeyStagePerformance;
using Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    [Authorize]
    [Route("project/{id}")]
    public class ProjectController : Controller
    {
        private readonly IProjects _projectRepository;
        private readonly IEducationPerformance _projectRepositoryEducationPerformance;

        public ProjectController(IProjects projectRepository, IEducationPerformance projectRepositoryEducationPerformance)
        {
            _projectRepository = projectRepository;
            _projectRepositoryEducationPerformance = projectRepositoryEducationPerformance;
        }
        
        public async Task<IActionResult> Index([FromRoute] string id)
        {
            var project = await _projectRepository.GetByUrn(id);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var viewModel = new ProjectTaskListViewModel
            {
                Project = project.Result
            };

            // TODO: Add error handling
            var educationPerformance =
                await _projectRepositoryEducationPerformance.GetByAcademyUrn(project.Result.OutgoingAcademyUrn);
            if (educationPerformance.IsValid)
            {
                viewModel.EducationPerformance = educationPerformance.Result;
            }
            
            ViewData["ProjectId"] = id;

            return View(viewModel);
        }
    }
}