using System.Linq;
using System.Threading.Tasks;
using Data;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects
{
    public class PupilNumbers : CommonPageModel
    {
        protected const string FragmentEdit = "additional-information-hint";
        public string GirlsOnRoll { get; set; }
        public string BoysOnRoll { get; set; }
        public string WithStatementOfSEN { get; set; }
        public string WithEAL { get; set; }
        public string FreeSchoolMealsLast6Years { get; set; }
        public string AcademyName { get; set; }
        
        [BindProperty]
        public AdditionalInformationViewModel AdditionalInformationViewModel { get; set; }
        public bool IsPreview { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public bool AddOrEditAdditionalInformation { get; set; }

        [BindProperty(SupportsGet = true)]
        public string AcademyUkprn { get; set; }
        
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;

        public PupilNumbers(IGetInformationForProject getInformationForProject, IProjects projectsRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var projectInformation = await _getInformationForProject.Execute(Urn);
            var academy = projectInformation.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn);
            var pupilNumbers = projectInformation.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn).PupilNumbers;
            GirlsOnRoll = pupilNumbers.GirlsOnRoll;
            BoysOnRoll = pupilNumbers.BoysOnRoll;
            WithStatementOfSEN = pupilNumbers.WithStatementOfSen;
            WithEAL = pupilNumbers.WhoseFirstLanguageIsNotEnglish;
            FreeSchoolMealsLast6Years = pupilNumbers
                .PercentageEligibleForFreeSchoolMealsDuringLast6Years;
            OutgoingAcademyUrn = academy.Urn;
            AcademyName = academy.Name;
            AdditionalInformationViewModel = new AdditionalInformationViewModel
            {
                AdditionalInformation =academy.PupilNumbers.AdditionalInformation,
                HintText =
                    "If you add comments, they'll be included in the pupil numbers section of your project template.",
                Urn = projectInformation.Project.Urn,
                AddOrEditAdditionalInformation = AddOrEditAdditionalInformation,
                ReturnToPreview = ReturnToPreview
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var model = await _projectsRepository.GetByUrn(Urn);

            model.Result.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == AcademyUkprn).PupilNumbersAdditionalInformation 
                = AdditionalInformationViewModel.AdditionalInformation;
            await _projectsRepository.Update(model.Result);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {Urn});
            }

            return RedirectToPage("/Projects/PupilNumbers", null, new {Urn}, FragmentEdit);
        }
    }
}