using System.Threading.Tasks;
using Data;
using Frontend.ExtensionMethods;
using Frontend.Models;
using Frontend.Models.Rationale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Projects.Rationale
{
    public class TrustOrSponsor : CommonPageModel
    {
        private readonly IProjects _projectsRepository;

        [BindProperty]
        public RationaleTrustOrSponsorViewModel ViewModel { get; set; }

        public TrustOrSponsor(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);
            if (!project.IsValid)
            {
                return this.View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;

            Urn = projectResult.Urn;
            OutgoingAcademyName = projectResult.OutgoingAcademyName;
            ReturnToPreview = ReturnToPreview;
            ViewModel = new RationaleTrustOrSponsorViewModel
            {
                TrustOrSponsorRationale = projectResult.Rationale.Trust
            };

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);
            if (!project.IsValid)
            {
                return this.View("ErrorPage", project.Error.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var projectResult = project.Result;
            projectResult.Rationale.Trust = ViewModel.TrustOrSponsorRationale;

            var result = await _projectsRepository.Update(projectResult);
            if (!result.IsValid)
            {
                return this.View("ErrorPage", result.Error.ErrorMessage);
            }

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { id = Urn });
            }

            return RedirectToAction("Index", "Rationale", new { Urn });
        }
    }
}