using System.Collections.Generic;
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

        public SortedDictionary<string, string> InitiatedCheckboxes()
        {
            var values =
                EnumHelpers<TransferFeatures.ProjectInitiators>.GetDisplayableValues(TransferFeatures.ProjectInitiators
                    .Empty);
            var result = new SortedDictionary<string, string>();
            
            foreach (var value in values)
            {
                result.Add(value.ToString(), EnumHelpers<TransferFeatures.ProjectInitiators>.GetDisplayValue(value));
            }

            return result;
        }
    }
}