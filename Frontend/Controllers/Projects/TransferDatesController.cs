using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{urn}/transfer-dates")]
    public class TransferDatesController : Controller
    {
        private readonly IProjects _projectsRepository;

        public TransferDatesController(IProjects projectsRepository)
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

            var model = new TransferDatesViewModel
            {
                Project = project.Result
            };
            return View(model);
        }

        [HttpGet("first-discussed")]
        public async Task<IActionResult> FirstDiscussed(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new TransferDatesViewModel {Project = project.Result, ReturnToPreview = returnToPreview};
            return View(model);
        }

        [HttpPost("first-discussed")]
        [ActionName("FirstDiscussed")]
        public async Task<IActionResult> FirstDiscussedPost(string urn, string day, string month, string year, bool dateUnknown = false,
            bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new TransferDatesViewModel {Project = project.Result, ReturnToPreview = returnToPreview};

            var dateString = DatesHelper.DayMonthYearToDateString(day, month, year);

            if (!string.IsNullOrEmpty(dateString) && !DatesHelper.IsValidDate(dateString))
            {
                model.FormErrors.AddError("day", "day", "Enter a valid date");
                return View(model);
            }
            
            if (string.IsNullOrEmpty(dateString) && !dateUnknown)
            {
                model.FormErrors.AddError("day", "day", "You must enter the date or confirm that you don't know it");
                return View(model);
            }

            model.Project.Dates.FirstDiscussed = dateUnknown ? null : dateString;
            model.Project.Dates.HasFirstDiscussedDate = !dateUnknown;
            
            var result = await _projectsRepository.Update(model.Project);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            if (returnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = urn});
            }

            return RedirectToAction("Index", new {urn});
        }

        [HttpGet("target-date")]
        public async Task<IActionResult> TargetDate(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new TransferDatesViewModel {Project = project.Result, ReturnToPreview = returnToPreview};

            return View(model);
        }

        [HttpPost("target-date")]
        [ActionName("TargetDate")]
        public async Task<IActionResult> TargetDatePost(string urn, string day, string month, string year,
            bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new TransferDatesViewModel {Project = project.Result, ReturnToPreview = returnToPreview};

            var dateString = DatesHelper.DayMonthYearToDateString(day, month, year);
            model.Project.Dates.Target = dateString;

            if (!string.IsNullOrEmpty(dateString) && !DatesHelper.IsValidDate(dateString))
            {
                model.FormErrors.AddError("day", "day", "Enter a valid date");
                return View(model);
            }
            
            if (!string.IsNullOrEmpty(model.Project.Dates.Target))
            {
                if (DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(model.Project.Dates.Htb,
                    model.Project.Dates.Target) == true)
                {
                    model.FormErrors.AddError("day", "day", 
                        $"The target transfer date must be on or after the Advisory Board date");
                    return View(model);
                }
            }

            var result = await _projectsRepository.Update(model.Project);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            if (returnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = urn});
            }

            return RedirectToAction("Index", new {urn});
        }

        [HttpGet("htb-date")]
        public async Task<IActionResult> HtbDate(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new TransferDatesViewModel {Project = project.Result, ReturnToPreview = returnToPreview};
            return View(model);
        }

        [HttpPost("htb-date")]
        [ActionName("HtbDate")]
        public async Task<IActionResult> HtbDatePost(string urn, string day, string month, string year,
            bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new TransferDatesViewModel {Project = project.Result, ReturnToPreview = returnToPreview};

            var dateString = DatesHelper.DayMonthYearToDateString(day, month, year);
            model.Project.Dates.Htb = dateString;

            if (!string.IsNullOrEmpty(dateString) && !DatesHelper.IsValidDate(dateString))
            {
                model.FormErrors.AddError("day", "day", "Enter a valid date");
                return View(model);
            }
            
            if (!string.IsNullOrEmpty(model.Project.Dates.Htb))
            {
                if (DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(model.Project.Dates.Htb,
                    model.Project.Dates.Target) == true)
                {
                    model.FormErrors.AddError("day", "day", 
                        $"The Advisory Board date must be on or before the target date for the transfer");
                    return View(model);
                }
            }

            var result = await _projectsRepository.Update(model.Project);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            if (returnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = urn});
            }

            return RedirectToAction("Index", new {urn});
        }
    }
}