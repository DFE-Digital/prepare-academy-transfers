using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Models.Projects;

namespace Data.Models
{
    public class Project
    {
        public Project()
        {
            Features = new TransferFeatures();
            TransferDates = new TransferDates();
            TransferBenefits = new TransferBenefits();
            TransferringAcademies = new List<TransferringAcademies>();
        }

        public string Urn { get; set; }
        public string Name { get; set; }
        public string OutgoingTrustUkprn { get; set; }
        public string OutgoingTrustName { get; set; }

        public string State { get; set; }
        public string Status { get; set; }
        public List<TransferringAcademies> TransferringAcademies { get; set; }
        public TransferFeatures Features { get; set; }
        public TransferDates TransferDates { get; set; }

        public TransferBenefits TransferBenefits { get; set; }
    }

    public class TransferBenefits
    {
        public TransferBenefits()
        {
            IntendedBenefits = new List<IntendedBenefit>();
            OtherFactors = new Dictionary<OtherFactor, string>();
        }

        public enum IntendedBenefit
        {
            Empty = 0,

            [Display(Name = "Strengthening governance")]
            StrengtheningGovernance,

            [Display(Name = "Improving safeguarding")]
            ImprovingSafeguarding,

            [Display(Name = "Stronger leadership")]
            StrongerLeadership,

            [Display(Name = "Securing financial recovery")]
            SecurityFinancialRecovery,

            [Display(Name = "Improving Ofsted rating")]
            ImprovingOfstedRating,

            [Display(Name = "A central financial team and central support")]
            CentralFinanceTeamAndSupport,
            Other
        }

        public enum OtherFactor
        {
            Empty = 0,

            [Display(Name = "This is a high profile transfer (ministers and media could be involved)")]
            HighProfile,

            [Display(Name = "There are complex land and building issues")]
            ComplexLandAndBuildingIssues,

            [Display(Name = "There are finance and debt concerns")]
            FinanceAndDebtConcerns
        }

        public List<IntendedBenefit> IntendedBenefits { get; set; }
        public string OtherIntendedBenefit { get; set; }
        public Dictionary<OtherFactor, string> OtherFactors { get; set; }
    }

    public class TransferDates
    {
        public string FirstDiscussed { get; set; }
        public string Target { get; set; }
        public string Htb { get; set; }
    }
}