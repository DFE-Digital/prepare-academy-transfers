using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using Dfe.PrepareTransfers.Web.Validators.TransferDates;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.TransferDates
{
    public class Target : CommonPageModel
    {
        private readonly IProjects _projectsRepository;
        protected string NameOfUser => User.FindFirstValue("name") ?? string.Empty;
        [BindProperty]
        [CustomizeValidator(Skip = true)]
        public TargetDateViewModel TargetDateViewModel { get; set; }
        public bool TargetDateAlreadyExists { get; set; }
        public string ExistingTargetDate { get; set; }

        public Target(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            var projectResult = project.Result;
            TargetDateAlreadyExists = projectResult.Dates.Target is not null;
            if (TargetDateAlreadyExists) ExistingTargetDate = projectResult.Dates.Target;

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
            TargetDateViewModel.TargetDate.IgnoreDayPart = true;

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
            if (projectResult.Dates.Target is not null)
            {
                return RedirectToPage("/Projects/TransferDates/Reason", new { Urn, TargetDate = TargetDateViewModel.TargetDate.DateInputAsString() });
            }
            projectResult.Dates.Target = TargetDateViewModel.TargetDate.DateInputAsString();

            projectResult.Dates.HasTargetDateForTransfer = !TargetDateViewModel.TargetDate.UnknownDate;

            await _projectsRepository.UpdateDates(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }

            return RedirectToPage("/Projects/TransferDates/Index", new { Urn });
        }
    }
}