using System.ComponentModel.DataAnnotations;

namespace Data.Models.Projects
{
    public class TransferFeatures
    {
        public enum ReasonForTheTransferTypes
        {
            // We store the enum name as a string in the API. This data was previously
            // used for a "Who initatied the transfer?" question, which is why some of the
            // member names relate to that. We may want to update these at a later date.
            Empty = 0,

            [Display(Name = "Intervention")]
            Dfe,
            [Display(Name = "Initiated by trust")]
            OutgoingTrust,
            [Display(Name = "Sponsor or trust closure")]
            SponsorOrTrustClosure
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

        public ReasonForTheTransferTypes ReasonForTheTransfer { get; set; }
        public TransferTypes TypeOfTransfer { get; set; }
        public string OtherTypeOfTransfer { get; set; }
        public bool? IsCompleted { get; set; }
    }
}