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
            GirlsOnRoll = projectInformation.OutgoingAcademy.PupilNumbers.GirlsOnRoll;
            BoysOnRoll = projectInformation.OutgoingAcademy.PupilNumbers.BoysOnRoll;
            WithStatementOfSEN = projectInformation.OutgoingAcademy.PupilNumbers.WithStatementOfSen;
            WithEAL = projectInformation.OutgoingAcademy.PupilNumbers.WhoseFirstLanguageIsNotEnglish;
            FreeSchoolMealsLast6Years = projectInformation.OutgoingAcademy.PupilNumbers
                .PercentageEligibleForFreeSchoolMealsDuringLast6Years;
            OutgoingAcademyUrn = projectInformation.OutgoingAcademy.Urn;
            AcademyName = projectInformation.OutgoingAcademy.Name;
            AdditionalInformationViewModel = new AdditionalInformationViewModel
            {
                AdditionalInformation = projectInformation.Project.PupilNumbersAdditionalInformation,
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

            model.Result.PupilNumbersAdditionalInformation = AdditionalInformationViewModel.AdditionalInformation;
            await _projectsRepository.Update(model.Result);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {Urn});
            }

            return RedirectToPage("/Projects/PupilNumbers", null, new {Urn}, FragmentEdit);
        }
    }
}