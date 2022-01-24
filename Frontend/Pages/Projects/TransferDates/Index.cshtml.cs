using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.TransferDates
{
    public class Index : CommonPageModel
    {
        private readonly IProjects _projectsRepository;
        public string FirstDiscussedDate { get; set; } 
        public bool? HasFirstDiscussedDate { get; set; } 
        public string HtbDate { get; set; } 
        public bool? HasHtbDate { get; set; } 
        public string TargetDate { get; set; } 
        public bool? HasTargetDate { get; set; } 

        public Index(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            var projectResult = project.Result;

            FirstDiscussedDate = projectResult.Dates?.FirstDiscussed;
            HasFirstDiscussedDate = projectResult.Dates?.HasFirstDiscussedDate;
            HtbDate = projectResult.Dates?.Htb;
            HasHtbDate = projectResult.Dates?.HasHtbDate;
            TargetDate = projectResult.Dates?.Target;
            HasTargetDate = projectResult.Dates?.HasTargetDateForTransfer;
            OutgoingAcademyUrn = projectResult.OutgoingAcademyUrn;

            return Page();
        }
    }
}