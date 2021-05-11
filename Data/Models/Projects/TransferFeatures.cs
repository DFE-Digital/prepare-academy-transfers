namespace Data.Models.Projects
{
    public class TransferFeatures
    {
        public string WhoInitiatedTheTransfer { get; set; }
        public ReasonForTransfer ReasonForTransfer { get; set; }
        public string TypeOfTransfer { get; set; }

        public bool HasTransferReasonBeenSet => ReasonForTransfer.IsSubjectToRddOrEsfaIntervention != null;
    }
}