using Data.Models.Projects;
using Frontend.Models.Forms;
using Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Frontend.Models
{
    //todo:remove base project model!
    public class FeaturesInitiatedViewModel
    {
        public string Urn { get; set; }
        public string OutgoingAcademyName { get; set; }
        public TransferFeatures.ProjectInitiators WhoInitiated { get; set; }
        public bool ReturnToPreview { get; set; }

        public static List<RadioButtonViewModel> InitiatedRadioButtons(TransferFeatures.ProjectInitiators selected)
        {
            var values =
                EnumHelpers<TransferFeatures.ProjectInitiators>.GetDisplayableValues(TransferFeatures.ProjectInitiators
                    .Empty);

            var result = values.Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(),
                Name = "WhoInitiated",
                DisplayName = EnumHelpers<TransferFeatures.ProjectInitiators>.GetDisplayValue(value),
                Checked = selected == value
            }).ToList();

            return result;
        }
    }

    public class FeaturesReasonViewModel
    {
        public string Urn { get; set; }
        public string OutgoingAcademyName { get; set; }
        public bool? IsSubjectToIntervention { get; set; }
        public string MoreDetail { get; set; }
        public bool ReturnToPreview { get; set; }

        public List<RadioButtonViewModel> ReasonRadioButtons()
        {
            var result = new[] { true, false }.Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(),
                Name = nameof(IsSubjectToIntervention),
                DisplayName = value ? "Yes" : "No",
                Checked = IsSubjectToIntervention == value
            }).ToList();

            return result;
        }
    }

    public class FeaturesViewModel : ProjectViewModel
    {
        //todo: remove once all views using ModelState
        public readonly FormErrorsViewModel FormErrors;

        public FeaturesViewModel()
        {
            FormErrors = new FormErrorsViewModel();
        }

        public bool HasTransferReasonBeenSet =>
            Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention != null;

        public bool IsTransferSubjectToIntervention =>
            Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == true; 

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

        public bool ReturnToPreview { get; set; }

    }
}