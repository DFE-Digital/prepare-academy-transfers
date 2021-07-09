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

            var educationPerformance =
                await _projectRepositoryEducationPerformance.GetByAcademyUrn(project.Result.OutgoingAcademyUrn);
            if (educationPerformance.IsValid)
            {
                ViewData["HasKeyStage2PerformanceData"] =
                    HasKeyStage2PerformanceData(educationPerformance.Result?.KeyStage2Performance);
            }
            else
            {
                ViewData["HasKeyStage2PerformanceData"] = false;
            }

            var viewModel = new ProjectTaskListViewModel
            {
                Project = project.Result
            };

            ViewData["ProjectId"] = id;
            
            return View(viewModel);
        }

        private static bool HasKeyStage2PerformanceData(List<KeyStage2> keyStage2Performance)
        {
            return keyStage2Performance != null && keyStage2Performance.Any(result => HasValue(result.MathsProgressScore)
                || HasValue(result.ReadingProgressScore)
                || HasValue(result.WritingProgressScore)
                || HasValue(result.PercentageAchievingHigherStdInRWM)
                || HasValue(result.PercentageMeetingExpectedStdInRWM));
        }

        private static bool HasValue(DisadvantagedPupilsResult disadvantagedPupilResult)
        {
            return !string.IsNullOrEmpty(disadvantagedPupilResult.Disadvantaged) ||
                   !string.IsNullOrEmpty(disadvantagedPupilResult.NotDisadvantaged);
        }
    }
}