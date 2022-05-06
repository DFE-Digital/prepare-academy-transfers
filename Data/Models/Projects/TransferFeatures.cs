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

        public enum TransferTypes
        {
            Empty = 0,

            [Display(Name = "Closure of a SAT and the academy joining a MAT")]
            SatClosure,

            [Display(Name = "Closure of a MAT and academies joining a MAT")]
            MatClosure,

            [Display(Name = "Academy moving out of a MAT and joining another MAT")]
            MatToMat,

            [Display(Name = "Academies joining together to form a new MAT")]
            JoiningToFormMat,

            [Display(Name = "Closure of one of more MAT(s) to form a new MAT")]
            TrustsMerging,

            Other
        }

        public ProjectInitiators WhoInitiatedTheTransfer { get; set; }
        public TransferTypes TypeOfTransfer { get; set; }
        public string OtherTypeOfTransfer { get; set; }
        public bool? IsCompleted { get; set; }
    }
}