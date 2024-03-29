using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.TransferDates
{
    public class Index : CommonPageModel
    {
        private readonly IProjects _projectsRepository;
        public string AdvisoryBoardDate { get; set; } 
        public bool? HasAdvisoryBoardDate { get; set; } 
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
            ProjectReference = projectResult.Reference;
            AdvisoryBoardDate = projectResult.Dates?.Htb;
            HasAdvisoryBoardDate = projectResult.Dates?.HasHtbDate;
            TargetDate = projectResult.Dates?.Target;
            HasTargetDate = projectResult.Dates?.HasTargetDateForTransfer;
            OutgoingAcademyUrn = projectResult.OutgoingAcademyUrn;

            return Page();
        }
    }
}