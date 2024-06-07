using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.GeneralInformation
{
    public class DistanceFromTrustModel : CommonPageModel
    {
        public string AcademyName { get; set; }

        [BindProperty] public decimal? DistanceToTrust { get; set; }
        [BindProperty] public string DistanceFromAcademyToTrustHqDetails { get; set; }

        public bool IsPreview { get; set; }

        [BindProperty(SupportsGet = true)] public bool AddOrEditPFIScheme { get; set; }

        [BindProperty(SupportsGet = true)] public string AcademyUkprn { get; set; }

        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;

        public DistanceFromTrustModel(IGetInformationForProject getInformationForProject, IProjects projectsRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var projectInformation = await _getInformationForProject.Execute(Urn);
            var academy = projectInformation.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn);

            OutgoingAcademyUrn = academy.Urn;
            AcademyName = academy.Name;
            DistanceToTrust = academy.DistanceFromAcademyToTrustHq;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!DistanceToTrust.HasValue)
            {
                ModelState.AddModelError(nameof(DistanceToTrust), "Please provide distance to trust.");
            }
            else if (!decimal.TryParse(DistanceToTrust.Value.ToString(), out var distanceToTrust))
            {
                ModelState.AddModelError(nameof(DistanceToTrust), "Please provide a valid distance to trust.");
            }
            else
            {
                DistanceToTrust = distanceToTrust;
            }

            if (!ModelState.IsValid)
            {
                // Return the current page with validation errors
                return Page();
            }

            var model = await _projectsRepository.GetByUrn(Urn);

            var academy = model.Result.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == AcademyUkprn);

            academy.DistanceFromAcademyToTrustHq = DistanceToTrust;
            academy.DistanceFromAcademyToTrustHqDetails = DistanceFromAcademyToTrustHqDetails;

            await _projectsRepository.UpdateAcademyGeneralInformation(model.Result.Urn, academy);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }

            return Redirect($"/project/{Urn}/general-information/{AcademyUkprn}");
        }

    }
}