using System;
using System.Threading.Tasks;
using Data;
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
        
        public async Task<IActionResult> Index([FromRoute] Guid id)
        {
            var project = await _projectRepository.GetByUrn(id.ToString());
            
            ViewData["OutgoingTrustName"] = project.Result.OutgoingTrustName;
            ViewData["ProjectName"] = project.Result.Name;
            ViewData["ProjectId"] = id;
            
            return View();
        }
    }
}