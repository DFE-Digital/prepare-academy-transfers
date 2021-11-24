using System.Threading.Tasks;
using Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Frontend.Models;
using Frontend.Models.TransferDates;
using Frontend.Validators.TransferDates;
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

            var projectResult = project.Result;
            var vm = new TransferDatesSummaryViewModel
            {
                Urn = projectResult.Urn,
                OutgoingAcademyUrn = projectResult.OutgoingAcademyUrn,
                FirstDiscussedDate = projectResult.Dates?.FirstDiscussed,
                HasFirstDiscussedDate = projectResult.Dates?.HasFirstDiscussedDate,
                HtbDate = projectResult.Dates?.Htb,
                HasHtbDate = projectResult.Dates?.HasHtbDate,
                TargetDate = projectResult.Dates?.Target,
                HasTargetDate = projectResult.Dates?.HasTargetDateForTransfer
            };
            return View(vm);
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

            var projectResult = project.Result;

            var vm = new TargetDateViewModel
            {
                Urn = urn,
                ReturnToPreview = returnToPreview,
                TargetDate = new DateViewModel
                {
                    Date = DateViewModel.SplitDateIntoDayMonthYear(projectResult.Dates.Target),
                    UnknownDate = projectResult.Dates.HasTargetDateForTransfer is false
                }
            };

            return View(vm);
        }
        
        [HttpPost("target-date")]
        [ActionName("TargetDate")]
        public async Task<IActionResult> TargetDatePost([CustomizeValidator(Skip=true)] TargetDateViewModel vm)
        {
            var project = await _projectsRepository.GetByUrn(vm.Urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            
            var validationContext = new ValidationContext<TargetDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["HtbDate"] = projectResult.Dates.Htb
                }
            };
            var validator = new TargetDateValidator();
            var validationResult = await validator.ValidateAsync(validationContext);
            
            validationResult.AddToModelState(ModelState, null);

            if (!validationResult.IsValid)
            {
                return View(vm);
            }

            projectResult.Dates.Target = vm.TargetDate.DateInputAsString();
            projectResult.Dates.HasTargetDateForTransfer = !vm.TargetDate.UnknownDate;
            

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

        [HttpGet("htb-date")]
        public async Task<IActionResult> HtbDate(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }
            
            var projectResult = project.Result;
            var vm = new HtbDateViewModel
            {
                Urn = urn,
                ReturnToPreview = returnToPreview,
                HtbDate = new DateViewModel
                {
                    Date = DateViewModel.SplitDateIntoDayMonthYear(projectResult.Dates.Htb),
                    UnknownDate = projectResult.Dates.HasHtbDate is false
                }
            };
            
            return View(vm);
        }

        [HttpPost("htb-date")]
        [ActionName("HtbDate")]
        public async Task<IActionResult> HtbDatePost(HtbDateViewModel vm)
        {
            var project = await _projectsRepository.GetByUrn(vm.Urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }
            
            var projectResult = project.Result;

            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            
            var validationContext = new ValidationContext<HtbDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["TargetDate"] = projectResult.Dates.Target
                }
            };
            var validator = new HtbDateValidator();
            var validationResult = await validator.ValidateAsync(validationContext);
            
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState, null);
                return View(vm);
            }

            projectResult.Dates.Htb = vm.HtbDate.DateInputAsString();
            projectResult.Dates.HasHtbDate = !vm.HtbDate.UnknownDate;

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
    }
}