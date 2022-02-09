using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.GeneralInformation
{
    public class Index : CommonPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;

        public string SchoolPhase { get; set; }
        public string AgeRange { get; set; }
        public string Capacity { get; set; }
        public string NumberOnRoll { get; set; }
        public string FreeSchoolMeals { get; set; }
        public string PublishedAdmissionNumber { get; set; }
        public string PrivateFinanceInitiative { get; set; }
        public string ViabilityIssues { get; set; }
        public string FinancialDeficit { get; set; }
        public string SchoolType { get; set; }
        public string DiocesePercent { get; set; }
        public string DistanceFromAcademyToTrustHq { get; set; }
        public string MP { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string AcademyUkprn { get; set; }

        public Index(IGetInformationForProject getInformationForProject)
        {
            _getInformationForProject = getInformationForProject;
        }

        public async Task<IActionResult> OnGetAsync(string urn)
        {
            var getInformationForProjectResponse = await _getInformationForProject.Execute(urn);

            var generalInformation = getInformationForProjectResponse.OutgoingAcademy.GeneralInformation;

            SchoolPhase = generalInformation.SchoolPhase;
            AgeRange = generalInformation.AgeRange;
            Capacity = generalInformation.Capacity;
            NumberOnRoll = $"{generalInformation.NumberOnRoll} ({generalInformation.PercentageFull})";
            FreeSchoolMeals = generalInformation.PercentageFsm;
            PublishedAdmissionNumber = generalInformation.Pan;
            PrivateFinanceInitiative = generalInformation.Pfi;
            ViabilityIssues = generalInformation.ViabilityIssue;
            FinancialDeficit = generalInformation.Deficit;
            SchoolType = generalInformation.SchoolType;
            DiocesePercent = generalInformation.DiocesesPercent;
            DistanceFromAcademyToTrustHq = generalInformation.DistanceToSponsorHq;
            MP = generalInformation.MpAndParty;

            Urn = getInformationForProjectResponse.Project.Urn;
            OutgoingAcademyUrn = getInformationForProjectResponse.Project.OutgoingAcademyUrn;
            return Page();
        }
    }
}