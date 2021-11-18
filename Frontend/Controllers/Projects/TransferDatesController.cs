using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.TransferDates;
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

            var projectResult = project.Result;
            var vm = new FirstDiscussedViewModel
            {
                Urn = urn,
                ReturnToPreview = returnToPreview,
                FirstDiscussed = new DateViewModel
                {
                    Date = DateViewModel.SplitDateIntoDayMonthYear(projectResult.Dates.FirstDiscussed),
                    UnknownDate = projectResult.Dates.HasFirstDiscussedDate is false
                }
            };
                
            return View(vm);
        }

        [HttpPost("first-discussed")]
        [ActionName("FirstDiscussed")]
        public async Task<IActionResult> FirstDiscussedPost(FirstDiscussedViewModel vm)
        {
            var project = await _projectsRepository.GetByUrn(vm.Urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }
            
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var projectResult = project.Result;

            projectResult.Dates.FirstDiscussed =  vm.FirstDiscussed.DateInputAsString();
            projectResult.Dates.HasFirstDiscussedDate = !vm.FirstDiscussed.UnknownDate;

            var result = await _projectsRepository.Update(projectResult);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            if (vm.ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = vm.Urn});
            }

            return RedirectToAction("Index", new {vm.Urn});
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
            bool returnToPreview = false, bool dateUnknown = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new TransferDatesViewModel {Project = project.Result, ReturnToPreview = returnToPreview};
            
            var dateString = DatesHelper.DayMonthYearToDateString(day, month, year);

            model.Project.Dates.Target = dateString;
            model.Project.Dates.HasTargetDateForTransfer = !dateUnknown;

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

            if (string.IsNullOrEmpty(dateString) && !dateUnknown)
            {
                model.FormErrors.AddError("day", "day", "You must enter the date or confirm that you don't know it");
                return View(model);
            }

            if (DatesHelper.IsValidDate(dateString) && dateUnknown)
            {
                model.FormErrors.AddError("day", "day", "You must either enter the date or select 'I do not know this'");
                return View(model);
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
            bool returnToPreview = false, bool dateUnknown = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new TransferDatesViewModel {Project = project.Result, ReturnToPreview = returnToPreview};

            var dateString = DatesHelper.DayMonthYearToDateString(day, month, year);
            
            model.Project.Dates.Htb = dateString;
            model.Project.Dates.HasHtbDate = !dateUnknown;

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
            
            if (string.IsNullOrEmpty(dateString) && !dateUnknown)
            {
                model.FormErrors.AddError("day", "day", "You must enter the date or confirm that you don't know it");
                return View(model);
            }

            if (DatesHelper.IsValidDate(dateString) && dateUnknown)
            {
                model.FormErrors.AddError("day", "day", "You must either enter the date or select 'I do not know this'");
                return View(model);
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