using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Helpers;

namespace Dfe.PrepareTransfers.Web.Models.Benefits
{
    public class BenefitsSummaryViewModel : CommonViewModel
    {
        private readonly IList<TransferBenefits.IntendedBenefit> _intendedBenefits;
        private readonly string _otherIntendedBenefit;
        public readonly IList<OtherFactorsItemViewModel> OtherFactorsItems;
        public readonly string OutgoingAcademyUrn;
        public readonly bool? AnyRisks;
        public readonly bool? EqualitiesImpactAssessmentConsidered;

        public BenefitsSummaryViewModel(IList<TransferBenefits.IntendedBenefit> intendedBenefits, 
            string otherIntendedBenefit, 
            IList<OtherFactorsItemViewModel> otherFactorsItems,
            string projectUrn,
            string outgoingAcademyUrn,
            bool? anyRisks = null,
            bool? equalitiesImpactAssessmentConsidered = null)
        {
            _intendedBenefits = intendedBenefits;
            _otherIntendedBenefit = otherIntendedBenefit;
            OtherFactorsItems = otherFactorsItems;
            Urn = projectUrn;
            OutgoingAcademyUrn = outgoingAcademyUrn;
            AnyRisks = anyRisks;
            EqualitiesImpactAssessmentConsidered = equalitiesImpactAssessmentConsidered;
        }

        public List<string> IntendedBenefitsSummary()
        {
            var summary = _intendedBenefits.ToList()
                .FindAll(EnumHelpers<TransferBenefits.IntendedBenefit>.HasDisplayValue)
                .Select(EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue)
                .ToList();

            if (_intendedBenefits.Contains(TransferBenefits.IntendedBenefit.Other))
            {
                summary.Add($"Other: {_otherIntendedBenefit}");
            }

            return summary;
        }
    }
}