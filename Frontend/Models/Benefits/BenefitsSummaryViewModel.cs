using System.Collections.Generic;
using System.Linq;
using Data.Models.Projects;
using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models.Benefits
{
    public class BenefitsSummaryViewModel : CommonViewModel
    {
        private readonly IList<TransferBenefits.IntendedBenefit> _intendedBenefits;
        private readonly string _otherIntendedBenefit;
        private readonly IList<OtherFactorsItemViewModel> _otherFactorsItems;
        public readonly string OutgoingAcademyUrn;
        public readonly bool? AnyRisks;

        public BenefitsSummaryViewModel(IList<TransferBenefits.IntendedBenefit> intendedBenefits, 
            string otherIntendedBenefit, 
            IList<OtherFactorsItemViewModel> otherFactorsItems,
            string projectUrn,
            string outgoingAcademyUrn,
            bool? anyRisks = null)
        {
            _intendedBenefits = intendedBenefits;
            _otherIntendedBenefit = otherIntendedBenefit;
            _otherFactorsItems = otherFactorsItems;
            Urn = projectUrn;
            OutgoingAcademyUrn = outgoingAcademyUrn;
            AnyRisks = anyRisks;
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

        public List<string[]> OtherFactorsSummary()
        {
            return _otherFactorsItems.OrderBy(o => (int)o.OtherFactor).Select(otherFactor => new[]
            {
                EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(otherFactor.OtherFactor),
                otherFactor.Description
            }).ToList();
        }
        
    }
}