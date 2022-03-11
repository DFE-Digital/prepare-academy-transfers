using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.Projects
{
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

            [Display(Name = "Stronger leadership")]
            StrongerLeadership,

            [Display(Name = "Strengthening governance")]
            StrengtheningGovernance,

            [Display(Name = "Improving safeguarding")]
            ImprovingSafeguarding,

            [Display(Name = "Secure financial position")]
            SecurityFinancialRecovery,

            [Display(Name = "A central financial team and central support")]
            CentralFinanceTeamAndSupport,

            [Display(Name = "Improved pupil performance")]
            ImprovedPupilPerformance,

            [Display(Name = "Improved ofsted rating")]
            ImprovingOfstedRating,

            [Display(Name = "Long term stability")]
            LongTermStability,

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
}