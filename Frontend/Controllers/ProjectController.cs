using System;
using System.Threading.Tasks;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    [Authorize]
    [Route("project/{id}")]
    public class ProjectController : Controller
    {
        private readonly IProjectsRepository _projectsRepository;

        public ProjectController(IProjectsRepository projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }
        
        public async Task<IActionResult> Index([FromRoute] Guid id)
        {
            var project = await _projectsRepository.GetProjectById(id);
            
            ViewData["OutgoingTrustName"] = project.Result.ProjectTrusts[0].TrustName;
            ViewData["ProjectName"] = project.Result.ProjectName;
            ViewData["ProjectId"] = id;
            
            return View();
        }
    }
}