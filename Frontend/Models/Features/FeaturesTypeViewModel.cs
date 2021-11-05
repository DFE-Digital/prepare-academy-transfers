using System.Collections.Generic;
using System.Linq;
using Data.Models.Projects;
using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models.Features
{
    public class FeaturesTypeViewModel
    {
        public string Urn { get; set; }
        public bool ReturnToPreview { get; set; }
        public TransferFeatures.TransferTypes TypeOfTransfer { get; set; }

        public string OtherType { get; set; }
        public string OutgoingAcademyName { get; set; }

        public List<RadioButtonViewModel> TypeOfTransferRadioButtons()
        {
            var values =
                EnumHelpers<TransferFeatures.TransferTypes>.GetDisplayableValues(TransferFeatures.TransferTypes
                    .Empty);

            var result = values.Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(),
                Name = nameof(TypeOfTransfer),
                DisplayName = EnumHelpers<TransferFeatures.TransferTypes>.GetDisplayValue(value),
                Checked = TypeOfTransfer == value
            }).ToList();

            return result;
        }

    }
}