namespace Frontend.Models
{
    public class FeaturesViewModel : ProjectViewModel
    {
        public bool HasTransferReasonBeenSet =>
            Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention != null;

        public bool IsTransferSubjectToIntervention =>
            Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == true;
    }
}