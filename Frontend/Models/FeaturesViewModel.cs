using System.Collections.Generic;
using System.Linq;
using Data.Models.Projects;
using Frontend.Helpers;
using Frontend.Models.Forms;

namespace Frontend.Models
{
    public class FeaturesViewModel : ProjectViewModel
    {
        public readonly FormErrorsViewModel FormErrors;

        public FeaturesViewModel()
        {
            FormErrors = new FormErrorsViewModel();
        }

        public bool HasTransferReasonBeenSet =>
            Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention != null;

        public bool IsTransferSubjectToIntervention =>
            Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == true;

        public static List<RadioButtonViewModel> InitiatedRadioButtons(TransferFeatures.ProjectInitiators selected)
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

        public List<RadioButtonViewModel> ReasonRadioButtons()
        {
            var result = new[] {true, false}.Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(), Name = "isSubjectToIntervention",
                DisplayName = value ? "Yes" : "No",
                Checked = Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == value
            }).ToList();

            return result;
        }

        public List<RadioButtonViewModel> TypeOfTransferRadioButtons()
        {
            var values =
                EnumHelpers<TransferFeatures.TransferTypes>.GetDisplayableValues(TransferFeatures.TransferTypes
                    .Empty);

            var result = values.Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(), Name = "typeOfTransfer",
                DisplayName = EnumHelpers<TransferFeatures.TransferTypes>.GetDisplayValue(value),
                Checked = Project.Features.TypeOfTransfer == value
            }).ToList();

            return result;
        }
    }
}