using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    
    [Authorize]
    [Route("project/{urn}/features")]
    public class FeaturesController : Controller
    {
        private readonly IProjects _projectsRepository;

        public FeaturesController(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }
        
        public async Task<IActionResult> Index(string urn)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            
            var model = new FeaturesViewModel
            {
                Project = project.Result
            };
            
            return View(model);
        }
    }
}