using System.Threading.Tasks;
using Data;
using Frontend.Helpers;
using Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{urn}/transfer-dates")]
    public class TransferDatesController : Controller
    {
        private readonly IProjects _projectsRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TransferDatesController(IProjects projectsRepository, IDateTimeProvider dateTimeProvider)
        {
            _projectsRepository = projectsRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IActionResult> Index(string urn)
        {
            var model = await GetModel(urn);
            return View(model);
        }

        [HttpGet("first-discussed")]
        public async Task<IActionResult> FirstDiscussed(string urn)
        {
            var model = await GetModel(urn);
            return View(model);
        }

        [HttpPost("first-discussed")]
        [ActionName("FirstDiscussed")]
        public async Task<IActionResult> FirstDiscussedPost(string urn, string day, string month, string year)
        {
            var model = await GetModel(urn);
            var dateString = DatesHelper.DayMonthYearToDateString(day, month, year);
            model.Project.Dates.FirstDiscussed = dateString;

            if (string.IsNullOrEmpty(day) || string.IsNullOrEmpty(month) || string.IsNullOrEmpty(year))
            {
                model.FormErrors.AddError("day", "day", "Please enter the date the transfer was first discussed");
                return View(model);
            }

            if (!DatesHelper.IsValidDate(dateString))
            {
                model.FormErrors.AddError("day", "day", "Please enter a valid date");
                return View(model);
            }

            await _projectsRepository.Update(model.Project);
            return RedirectToAction("Index", new {urn});
        }

        [HttpGet("target-date")]
        public async Task<IActionResult> TargetDate(string urn)
        {
            var model = await GetModel(urn);
            return View(model);
        }

        [HttpPost("target-date")]
        [ActionName("TargetDate")]
        public async Task<IActionResult> TargetDatePost(string urn, string day, string month, string year)
        {
            var model = await GetModel(urn);
            var dateString = DatesHelper.DayMonthYearToDateString(day, month, year);
            model.Project.Dates.Target = dateString;

            if (string.IsNullOrEmpty(day) || string.IsNullOrEmpty(month) || string.IsNullOrEmpty(year))
            {
                model.FormErrors.AddError("day", "day", "Please enter the target date for the transfer");
                return View(model);
            }

            if (!DatesHelper.IsValidDate(dateString))
            {
                model.FormErrors.AddError("day", "day", "Please enter a valid date");
                return View(model);
            }

            await _projectsRepository.Update(model.Project);
            return RedirectToAction("Index", new {urn});
        }

        [HttpGet("htb-date")]
        public async Task<IActionResult> HtbDate(string urn)
        {
            var model = await GetModel(urn);
            return View(model);
        }

        [HttpPost("htb-date")]
        [ActionName("HtbDate")]
        public async Task<IActionResult> HtbDatePost(string urn, string htbDate)
        {
            var model = await GetModel(urn);
            model.Project.Dates.Htb = htbDate;

            if (string.IsNullOrEmpty(htbDate))
            {
                var startDateString = DatesHelper.DateTimeToDateString(_dateTimeProvider.Today());
                var htbDates = DatesHelper.GetFirstWorkingDaysOfTheTheMonthForTheNextYear(startDateString);
                var errorId = DatesHelper.DateTimeToDateString(htbDates[0]);
                model.FormErrors.AddError(errorId, "htbDate", "Please select an HTB date");
                return View(model);
            }

            await _projectsRepository.Update(model.Project);
            return RedirectToAction("Index", new {urn});
        }

        private async Task<TransferDatesViewModel> GetModel(string urn)
        {
            var project = await _projectsRepository.GetByUrn(urn);

            var model = new TransferDatesViewModel
            {
                Project = project.Result
            };

            return model;
        }
    }
}