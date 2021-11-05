using System.Collections.Generic;
using System.Linq;
using Data.Models.Projects;
using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models.Features
{
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
}