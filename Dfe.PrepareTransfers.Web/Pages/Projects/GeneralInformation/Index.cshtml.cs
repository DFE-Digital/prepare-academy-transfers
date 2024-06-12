using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.GeneralInformation
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
        public string GIASLastChangedDate { get; set; }
        public string MP { get; set; }

        [BindProperty(SupportsGet = true)]
        public string AcademyUkprn { get; set; }

        public string AcademyName { get; set; }
        public string Urn { get; set; }

        public Index(IGetInformationForProject getInformationForProject)
        {
            _getInformationForProject = getInformationForProject;
        }

        public async Task<IActionResult> OnGetAsync(string urn)
        {
            var getInformationForProjectResponse = await _getInformationForProject.Execute(urn);
            var academy = getInformationForProjectResponse.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn);
            var generalInformation = getInformationForProjectResponse.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn).GeneralInformation;
            AcademyName = academy.Name;
            SchoolPhase = generalInformation.SchoolPhase;
            AgeRange = generalInformation.AgeRange;
            Capacity = generalInformation.Capacity;
            NumberOnRoll = $"{generalInformation.NumberOnRoll} ({generalInformation.PercentageFull})";
            FreeSchoolMeals = generalInformation.PercentageFsm;
            PublishedAdmissionNumber = academy.PublishedAdmissionNumber;
            PrivateFinanceInitiative = $"{academy.PFIScheme} {academy.PFISchemeDetails}";
            ViabilityIssues = academy.ViabilityIssues;
            FinancialDeficit = academy.FinancialDeficit;
            SchoolType = generalInformation.SchoolType;
            DiocesePercent = generalInformation.DiocesesPercent;
            DistanceFromAcademyToTrustHq = $"{academy.DistanceFromAcademyToTrustHq?.ToString()} {academy.DistanceFromAcademyToTrustHqDetails}";
            MP = academy.MPNameAndParty;
            Urn = urn;
            GIASLastChangedDate = "N/A";
            if (academy.LastChangedDate.IsNullOrEmpty() is false)
            {
                GIASLastChangedDate = DateTime.Parse(academy.LastChangedDate, CultureInfo.GetCultureInfo("en-GB")).ToString("MMMM yyyy");
            }

            Urn = getInformationForProjectResponse.Project.Urn;
            OutgoingAcademyUrn = getInformationForProjectResponse.Project.OutgoingAcademyUrn;
            return Page();
        }
    }
}