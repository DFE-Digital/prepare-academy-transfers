using System.Threading.Tasks;
using Frontend.ExtensionMethods;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.GeneralInformation
{
    public class Index : CommonPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        
        public GeneralInformationViewModel ViewModel { get; private set; }
        
        public Index(IGetInformationForProject getInformationForProject)
        {
            _getInformationForProject = getInformationForProject;
        }
        
        public async Task<IActionResult> OnGetAsync(string urn)
        {
            var getInformationForProjectResponse = await _getInformationForProject.Execute(urn);

            if (!getInformationForProjectResponse.IsValid)
            {
                return this.View("ErrorPage", getInformationForProjectResponse.ResponseError.ErrorMessage);
            }
            
            var generalInformation = getInformationForProjectResponse.OutgoingAcademy.GeneralInformation;

            ViewModel = new GeneralInformationViewModel
            {
                SchoolPhase = generalInformation.SchoolPhase,
                AgeRange = generalInformation.AgeRange,
                Capacity = generalInformation.Capacity,
                NumberOnRoll = $"{generalInformation.NumberOnRoll} ({generalInformation.PercentageFull})",
                FreeSchoolMeals = generalInformation.PercentageFsm,
                PublishedAdmissionNumber = generalInformation.Pan,
                PrivateFinanceInitiative = generalInformation.Pfi,
                ViabilityIssues = generalInformation.ViabilityIssue,
                FinancialDeficit = generalInformation.Deficit,
                SchoolType = generalInformation.SchoolType,
                DiocesePercent = generalInformation.DiocesesPercent,
                DistanceFromAcademyToTrustHq = generalInformation.DistanceToSponsorHq,
                MP = generalInformation.MpAndParty
            };

            Urn = getInformationForProjectResponse.Project.Urn;
            OutgoingAcademyUrn = getInformationForProjectResponse.Project.OutgoingAcademyUrn;

            return Page();
        }
    }
}