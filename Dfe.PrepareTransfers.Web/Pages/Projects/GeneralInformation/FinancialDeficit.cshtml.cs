using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.GeneralInformation
{
    public class FinancialDeficitModel : CommonPageModel
    {
        public string AcademyName { get; set; }

        public bool IsPreview { get; set; }
        
        [BindProperty]
        public bool? YesChecked { get; set; }
        
        [BindProperty(SupportsGet = true)] public string AcademyUkprn { get; set; }

        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;

        public FinancialDeficitModel(IGetInformationForProject getInformationForProject, IProjects projectsRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var projectInformation = await _getInformationForProject.Execute(Urn);
            var academy = projectInformation.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn);
            var pupilNumbers = academy.PupilNumbers;
            YesChecked = academy.FinancialDeficit?.ToLower().Contains("yes") ?? false;

            OutgoingAcademyUrn = academy.Urn;
            AcademyName = academy.Name;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!YesChecked.HasValue)
            {
                ModelState.AddModelError("Financial Deficit", "Please select an option.");
            }

            if (!ModelState.IsValid)
            {
                // Return the current page with validation errors
                return Page();
            }

            var model = await _projectsRepository.GetByUrn(Urn);

            var academy = model.Result.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == AcademyUkprn);

            academy.FinancialDeficit = YesChecked == true ? "Yes " : "No";

            await _projectsRepository.UpdateAcademyGeneralInformation(model.Result.Urn, academy);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }

            return Redirect($"/project/{Urn}/general-information/{AcademyUkprn}");
        }

    }
}