using System.Threading.Tasks;
using Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using Dfe.PrepareTransfers.Web.Validators.TransferDates;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.TransferDates
{
    public class Target : CommonPageModel
    {
        private readonly IProjects _projectsRepository;

        [BindProperty] 
        [CustomizeValidator(Skip = true)]
        public TargetDateViewModel TargetDateViewModel { get; set; }

        public Target(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            var projectResult = project.Result;

            TargetDateViewModel = new TargetDateViewModel
            {
                TargetDate = new DateViewModel
                {
                    Date = DateViewModel.SplitDateIntoDayMonthYear(projectResult.Dates.Target),
                    UnknownDate = projectResult.Dates.HasTargetDateForTransfer is false
                }
            };
            IncomingTrustName = projectResult.IncomingTrustName;
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (TargetDateViewModel.TargetDate.Date.Month != null || TargetDateViewModel.TargetDate.Date.Year != null)
            {
                // Transfers always happen on the 1st of the month.
                TargetDateViewModel.TargetDate.Date.Day = "01";
            }
            
            var project = await _projectsRepository.GetByUrn(Urn);

            var projectResult = project.Result;

            var validationContext = new ValidationContext<TargetDateViewModel>(TargetDateViewModel)
            {
                RootContextData =
                {
                    ["AdvisoryBoardDate"] = projectResult.Dates.Htb
                }
            };
            var validator = new TargetDateValidator();
            var validationResult = await validator.ValidateAsync(validationContext);
            
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState, nameof(TargetDateViewModel));
                return Page();
            }
            
            projectResult.Dates.Target = TargetDateViewModel.TargetDate.DateInputAsString();
            projectResult.Dates.HasTargetDateForTransfer = !TargetDateViewModel.TargetDate.UnknownDate;

            await _projectsRepository.Update(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }

            return RedirectToPage("/Projects/TransferDates/Index", new { Urn });
        }
    }
}