using Data.Models.Projects;

namespace Frontend.Models.Features
{
    public class FeaturesViewModel : FeaturesCommonViewModel
    {
        public string OutgoingAcademyUrn { get; set; }
        public bool? IsSubjectToRddOrEsfaIntervention { get; set; }
        public bool HasTransferReasonBeenSet =>
            IsSubjectToRddOrEsfaIntervention != null;

        public bool IsTransferSubjectToIntervention =>
            IsSubjectToRddOrEsfaIntervention == true;

        public TransferFeatures.ProjectInitiators WhoInitiatedTheTransfer { get; set; }
        public string InterventionDetails { get; set; }
        public TransferFeatures.TransferTypes TypeOfTransfer { get; set; }
        public string OtherTypeOfTransfer { get; set; }
    }
}