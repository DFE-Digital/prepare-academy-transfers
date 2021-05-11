using System.ComponentModel.DataAnnotations;

namespace Data.Models.Projects
{
    public class TransferFeatures
    {
        public enum ProjectInitiators
        {
            Empty = 0,

            [Display(Name = "Department for Education")]
            Dfe,
            [Display(Name = "Outgoing trust")] OutgoingTrust
        }

        public ProjectInitiators WhoInitiatedTheTransfer { get; set; }
        public ReasonForTransfer ReasonForTransfer { get; set; }
        public string TypeOfTransfer { get; set; }

        public bool HasTransferReasonBeenSet => ReasonForTransfer.IsSubjectToRddOrEsfaIntervention != null;
    }
}