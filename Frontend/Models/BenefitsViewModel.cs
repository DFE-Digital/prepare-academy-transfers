using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.Projects;
using Frontend.Helpers;
using Frontend.Models.Forms;

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
    }
}