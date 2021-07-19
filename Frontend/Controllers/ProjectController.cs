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

            var educationPerformance =
                await _projectRepositoryEducationPerformance.GetByAcademyUrn(project.Result.OutgoingAcademyUrn);
            if (educationPerformance.IsValid)
            {
                viewModel.EducationPerformance = educationPerformance.Result;

                ViewData["HasKeyStage2PerformanceData"] =
                    HasKeyStage2PerformanceData(educationPerformance.Result?.KeyStage2Performance);
                ViewData["HasKeyStage4PerformanceData"] =
                    HasKeyStage4PerformanceData(educationPerformance.Result?.KeyStage4Performance);
            }
            else
            {
                ViewData["HasKeyStage2PerformanceData"] = false;
                ViewData["HasKeyStage4PerformanceData"] = false;
            }

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
        
        private static bool HasKeyStage4PerformanceData(List<KeyStage4> keyStage2Performance)
        {
            return keyStage2Performance != null && keyStage2Performance.Any(result => HasValue(result.SipAttainment8score)
                || HasValue(result.SipAttainment8scoreebacc)
                || HasValue(result.SipAttainment8scoreenglish)
                || HasValue(result.SipAttainment8scoremaths)
                || HasValue(result.SipAttainment8score)
                || HasValue(result.SipProgress8ebacc)
                || HasValue(result.SipProgress8english)
                || HasValue(result.SipProgress8maths)
                || HasValue(result.SipProgress8Score)
                || HasValue(result.SipNumberofpupilsprogress8));
        }

        private static bool HasValue(DisadvantagedPupilsResult disadvantagedPupilResult)
        {
            return !string.IsNullOrEmpty(disadvantagedPupilResult.Disadvantaged) ||
                   !string.IsNullOrEmpty(disadvantagedPupilResult.NotDisadvantaged);
        }
    }
}