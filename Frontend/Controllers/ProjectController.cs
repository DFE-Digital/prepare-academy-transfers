using System;
using System.Threading.Tasks;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    [Route("project/{id}")]
    public class ProjectController : Controller
    {
        private readonly IProjectsRepository _projectsRepository;

        public ProjectController(IProjectsRepository projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }
        
        public async Task<IActionResult> Index([FromRoute] string id)
        {
            var project = await _projectsRepository.GetProjectById(Guid.Parse(id));
            
            ViewData["OutgoingTrustName"] = project.Result.ProjectTrusts[0].TrustName;
            return View();
        }
    }
}