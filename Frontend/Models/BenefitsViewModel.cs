using System.Collections.Generic;
using System.Linq;
using Data.Models.Projects;
using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models
{
    public class BenefitsViewModel : ProjectViewModel
    {
        public readonly FormErrorsViewModel FormErrors;

        public BenefitsViewModel()
        {
            FormErrors = new FormErrorsViewModel();
        }

        public List<string> IntendedBenefitsSummary()
        {
            var summary = Project.Benefits.IntendedBenefits
                .FindAll(EnumHelpers<TransferBenefits.IntendedBenefit>.HasDisplayValue)
                .Select(EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue)
                .ToList();

            if (Project.Benefits.IntendedBenefits.Contains(TransferBenefits.IntendedBenefit.Other))
            {
                summary.Add($"Other: {Project.Benefits.OtherIntendedBenefit}");
            }

            return summary;
        }

        public List<string[]> OtherFactorsSummary()
        {
            return Project.Benefits.OtherFactors.Select(otherFactor => new[]
            {
                EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(otherFactor.Key),
                otherFactor.Value
            }).ToList();
        }

        public List<OtherFactorsViewModel> OtherFactorsVm { get; set; }

        public class OtherFactorsViewModel
        {
            public TransferBenefits.OtherFactor OtherFactor { get; set; }
            public bool Checked { get; set; }
            public string Description { get; set; }
        }
    }
}