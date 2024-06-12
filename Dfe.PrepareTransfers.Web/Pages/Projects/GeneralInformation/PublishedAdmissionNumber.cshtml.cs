using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.GeneralInformation
{
    public class PublishedAdmissionNumberModel : CommonPageModel
    {
        public string AcademyName { get; set; }

        [BindProperty] public string PublishedAdmissionNumber { get; set; }

        public bool IsPreview { get; set; }

        [BindProperty(SupportsGet = true)] public string AcademyUkprn { get; set; }

        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;

        public PublishedAdmissionNumberModel(IGetInformationForProject getInformationForProject, IProjects projectsRepository)
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
            PublishedAdmissionNumber = academy.PublishedAdmissionNumber;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(PublishedAdmissionNumber))
            {
                ModelState.AddModelError("Published Admission Number", "Please provide a published admission number");
            }
            else if (!int.TryParse(PublishedAdmissionNumber, out var publishedAdmissionNumber))
            {
                ModelState.AddModelError("Published Admission Number", "Please provide a valid published admission number.");
            }
            else
            {
                PublishedAdmissionNumber = publishedAdmissionNumber.ToString();
            }

            if (!ModelState.IsValid)
            {
                // Return the current page with validation errors
                return Page();
            }

            var model = await _projectsRepository.GetByUrn(Urn);

            var academy = model.Result.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == AcademyUkprn);

            academy.PublishedAdmissionNumber = PublishedAdmissionNumber;

            await _projectsRepository.UpdateAcademyGeneralInformation(model.Result.Urn, academy);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }

            return Redirect($"/project/{Urn}/general-information/{AcademyUkprn}");
        }

    }
}