using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dfe.PrepareTransfers.Data.Models.Projects
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

            [Display(Name = "Improved Ofsted rating")]
            ImprovingOfstedRating,

            [Display(Name = "Long term stability")]
            LongTermStability,

            Other
        }

        public enum OtherFactor
        {
            Empty = 0,

            [Display(Name = "Complex land and building issues")]
            ComplexLandAndBuildingIssues,
            
            [Display(Name = "Finance and debt concerns")]
            FinanceAndDebtConcerns,
            
            [Display(Name = "High profile transfer")]
            HighProfile,

            [Display(Name = "Other risks")]
            OtherRisks
        }

        public List<IntendedBenefit> IntendedBenefits { get; set; }
        public string OtherIntendedBenefit { get; set; }
        public Dictionary<OtherFactor, string> OtherFactors { get; set; }
        
        public bool? AnyRisks { get; set; }
        public bool? EqualitiesImpactAssessmentConsidered { get; set; }
        public bool? IsCompleted { get; set; }
    }
}