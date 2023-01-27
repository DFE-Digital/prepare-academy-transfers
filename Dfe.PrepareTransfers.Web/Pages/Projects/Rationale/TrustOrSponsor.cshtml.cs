using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Rationale;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.Rationale
{
    public class TrustOrSponsor : CommonPageModel
    {
        private readonly IProjects _projectsRepository;

        [BindProperty] public RationaleTrustOrSponsorViewModel ViewModel { get; set; }

        public TrustOrSponsor(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);

            var projectResult = project.Result;

            IncomingTrustName = projectResult.IncomingTrustName;
            ViewModel = new RationaleTrustOrSponsorViewModel
            {
                TrustOrSponsorRationale = projectResult.Rationale.Trust
            };

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
            projectResult.Rationale.Trust = ViewModel.TrustOrSponsorRationale;

            var result = await _projectsRepository.Update(projectResult);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {Urn});
            }

            return RedirectToPage("/Projects/Rationale/Index", new {Urn});
        }
    }
}