using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.TransferDates
{
    public class Index : CommonPageModel
    {
        private readonly IProjects _projectsRepository;
        public string AdvisoryBoardDate { get; set; }
        public string PreviousAdvisoryBoardDate { get; set; }
        public bool? HasAdvisoryBoardDate { get; set; }
        public string TargetDate { get; set; }
        public bool? HasTargetDate { get; set; }
        [BindProperty]
        public MarkSectionCompletedViewModel MarkSectionCompletedViewModel { get; set; }
        public Index(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            Data.Models.Project projectResult = project.Result;
            ProjectReference = projectResult.Reference;
            AdvisoryBoardDate = projectResult.Dates?.Htb;
            PreviousAdvisoryBoardDate = projectResult.Dates?.PreviousAdvisoryBoardDate;
            HasAdvisoryBoardDate = projectResult.Dates?.HasHtbDate;
            TargetDate = projectResult.Dates?.Target;
            HasTargetDate = projectResult.Dates?.HasTargetDateForTransfer;
            OutgoingAcademyUrn = projectResult.OutgoingAcademyUrn;
            MarkSectionCompletedViewModel = new MarkSectionCompletedViewModel
            {
                IsCompleted = projectResult.Dates.IsCompleted ?? false,
                ShowIsCompleted = true
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            RepositoryResult<Data.Models.Project> project = await _projectsRepository.GetByUrn(Urn);

            var projectResult = project.Result;

            projectResult.Dates.IsCompleted = MarkSectionCompletedViewModel.IsCompleted;

            await _projectsRepository.UpdateDates(projectResult);

            return RedirectToPage(ReturnToPreview ? Links.Project.Index.PageName : "/Projects/Index",
                new { Urn });
        }
    }
}