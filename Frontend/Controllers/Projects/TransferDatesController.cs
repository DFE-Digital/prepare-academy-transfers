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
        
        [HttpGet("target-date")]
        public async Task<IActionResult> TargetDate(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            
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
            
            var projectResult = project.Result;
            
            var validationContext = new ValidationContext<TargetDateViewModel>(vm)
            {
                RootContextData =
                {
                    ["AdvisoryBoardDate"] = projectResult.Dates.Htb
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
            

            await _projectsRepository.Update(projectResult);
            
            if (vm.ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = vm.Urn});
            }

            return RedirectToPage("/Projects/TransferDates/Index", new {vm.Urn});
        }
    }
}