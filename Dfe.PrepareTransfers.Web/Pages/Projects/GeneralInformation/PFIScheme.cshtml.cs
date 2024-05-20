using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.GeneralInformation
{
    public class PFISchemeModel : CommonPageModel
    {
        protected const string FragmentEdit = "pfi-scheme-hint";
        public string AcademyName { get; set; }

        [BindProperty] public PFISchemeViewModel PFISchemeViewModel { get; set; }
        public bool IsPreview { get; set; }
        [BindProperty]
        public bool? YesChecked { get; set; }
        [BindProperty(SupportsGet = true)] public bool AddOrEditPFIScheme { get; set; }

        [BindProperty(SupportsGet = true)] public string AcademyUkprn { get; set; }

        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;

        public PFISchemeModel(IGetInformationForProject getInformationForProject, IProjects projectsRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var projectInformation = await _getInformationForProject.Execute(Urn);
            var academy = projectInformation.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn);
            var pupilNumbers = academy.PupilNumbers;
            YesChecked = academy.PFIScheme?.ToLower().Contains("yes") ?? false;


            OutgoingAcademyUrn = academy.Urn;
            AcademyName = academy.Name;
            PFISchemeViewModel = new PFISchemeViewModel
            {
                PFIScheme = academy.PFIScheme,
                PFISchemeDetails = academy.PFISchemeDetails,
                HintText =
                    "Please disclose if this Academy is part of a PFI Scheme",
                Urn = projectInformation.Project.Urn,
                AddOrEditPFIScheme = AddOrEditPFIScheme,
                ReturnToPreview = ReturnToPreview,
                HideWarning = true
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (YesChecked == true && string.IsNullOrWhiteSpace(PFISchemeViewModel.PFISchemeDetails))
            {
                ModelState.AddModelError(nameof(PFISchemeViewModel.PFIScheme), "Please provide details of the PFI scheme.");
            }

            if (!ModelState.IsValid)
            {
                // Return the current page with validation errors
                return Page();
            }

            var model = await _projectsRepository.GetByUrn(Urn);

            var academy = model.Result.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == AcademyUkprn);

            academy.PFIScheme = YesChecked == true ? "Yes " + PFISchemeViewModel.PFIScheme : "No";
            academy.PFISchemeDetails = PFISchemeViewModel.PFISchemeDetails;

            await _projectsRepository.UpdateAcademyGeneralInformation(model.Result.Urn, academy);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }

            return Redirect($"/project/{Urn}/general-information/{AcademyUkprn}#{FragmentEdit}");
        }

    }
}