using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.TransferDates;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.TransferDates
{
    public class FirstDiscussed : CommonPageModel
    {
        private readonly IProjects _projectsRepository;

        [BindProperty] public FirstDiscussedViewModel FirstDiscussedViewModel { get; set; }

        public FirstDiscussed(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            var projectResult = project.Result;

            FirstDiscussedViewModel = new FirstDiscussedViewModel()
            {
                FirstDiscussed = new DateViewModel
                {
                    Date = DateViewModel.SplitDateIntoDayMonthYear(projectResult.Dates.FirstDiscussed),
                    UnknownDate = projectResult.Dates.HasFirstDiscussedDate is false
                }
            };
            IncomingTrustName = projectResult.IncomingTrustName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var projectResult = project.Result;
            projectResult.Dates.FirstDiscussed = FirstDiscussedViewModel.FirstDiscussed.DateInputAsString();
            projectResult.Dates.HasFirstDiscussedDate = !FirstDiscussedViewModel.FirstDiscussed.UnknownDate;

            await _projectsRepository.Update(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {Urn});
            }

            return RedirectToPage("/Projects/TransferDates/Index", new {Urn});
        }
    }
}