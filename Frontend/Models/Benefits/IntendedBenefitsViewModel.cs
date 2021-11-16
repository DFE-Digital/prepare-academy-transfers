using System.Collections.Generic;
using Data.Models.Projects;
using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models.Benefits
{
    public class IntendedBenefitsViewModel
    {
        public IntendedBenefitsViewModel()
        {
            SelectedIntendedBenefits = new List<TransferBenefits.IntendedBenefit>();
        }
        public string ProjectUrn { get; set; }
        public string OutgoingAcademyName { get; set; }
        public bool ReturnToPreview { get; set; }
        public IList<TransferBenefits.IntendedBenefit> SelectedIntendedBenefits { get; set; }
        public string OtherBenefit { get; set; }

        public IList<CheckboxViewModel> GetIntendedBenefitsCheckboxes()
        {
            IList<CheckboxViewModel> items = new List<CheckboxViewModel>();
            foreach (var intendedBenefit in EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayableValues(TransferBenefits.IntendedBenefit.Empty))
            {
                items.Add(new CheckboxViewModel
                {
                    DisplayName = EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue(intendedBenefit),
                    Name = nameof(SelectedIntendedBenefits),
                    Value = intendedBenefit.ToString(),
                    Checked = SelectedIntendedBenefits.Contains(intendedBenefit)
                });
            }

            return items;
        }
    }
}
