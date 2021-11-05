using System.Collections.Generic;
using System.Linq;
using Data.Models.Projects;
using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models.Features
{
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