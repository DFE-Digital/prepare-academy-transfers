using System.Threading.Tasks;
using Data;
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

        public ProjectController(IProjects projectRepository)
        {
            _projectRepository = projectRepository;
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

            ViewData["ProjectId"] = id;
            
            return View(viewModel);
        }
    }
}