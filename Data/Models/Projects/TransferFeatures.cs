namespace Data.Models.Projects
{
    public class TransferFeatures
    {
        public string WhoInitiatedTheTransfer { get; set; }
        public bool IsSubjectToRddOrEsfaIntervention { get; set; }
        public string InterventionDetails { get; set; }
        public string ReasonForTransfer { get; set; }
    }
}