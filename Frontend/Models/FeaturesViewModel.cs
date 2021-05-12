using System.Collections.Generic;
using System.Linq;
using Data.Models.Projects;
using Frontend.Helpers;

namespace Frontend.Models
{
    public class FeaturesViewModel : ProjectViewModel
    {
        public bool HasTransferReasonBeenSet =>
            Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention != null;

        public bool IsTransferSubjectToIntervention =>
            Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == true;

        public bool HasError => !string.IsNullOrEmpty(Error);

        public string Error { get; set; }

        public static List<RadioButtonViewModel> InitiatedCheckboxes(TransferFeatures.ProjectInitiators selected)
        {
            var values =
                EnumHelpers<TransferFeatures.ProjectInitiators>.GetDisplayableValues(TransferFeatures.ProjectInitiators
                    .Empty);

            var result = values.Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(), Name = "whoInitiated",
                DisplayName = EnumHelpers<TransferFeatures.ProjectInitiators>.GetDisplayValue(value),
                Checked = selected == value
            }).ToList();

            return result;
        }
    }
}