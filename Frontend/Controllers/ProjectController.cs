using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
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
            
            var viewModel = new ProjectTaskListViewModel
            {
                Project = project.Result
            };
            
            var educationPerformance =
                await _projectRepositoryEducationPerformance.GetByAcademyUrn(project.Result.OutgoingAcademyUrn);
            
            viewModel.EducationPerformance = educationPerformance.Result;
            
            
            ViewData["ProjectId"] = id;

            return View(viewModel);
        }
    }
}