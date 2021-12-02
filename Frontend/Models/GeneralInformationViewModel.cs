using System;
using System.Collections.Generic;
using Data.Models;
using Frontend.Models.Forms;

namespace Frontend.Models
{
    public class GeneralInformationViewModel : CommonViewModel
    {
        public string SchoolPhase { get; set; }
        public string AgeRange { get; set; }
        public string Capacity { get; set; }
        public string NumberOnRoll { get; set; }
        public string FreeSchoolMeals {get; set; }
        public string PublishedAdmissionNumber { get; set; }
        public string PrivateFinanceInitiative { get; set; }
        public string ViabilityIssues { get; set; }
        public string FinancialDeficit { get; set; }
        public string SchoolType { get; set; }
        public string DiocesePercent { get; set; }
        public string DistanceFromAcademyToTrustHq { get; set; }
        public string MP { get; set; }
        public string OutgoingAcademyUrn { get; set; }
    }
}