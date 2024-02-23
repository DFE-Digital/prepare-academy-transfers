using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dfe.PrepareTransfers.Data.Models.Projects
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

        public enum SpecificReasonForTheTransferTypes
        {
            // We store the enum name as a string in the API. This data was previously
            Empty = 0,

            [Display(Name = "Forced transfer following inadequate Ofsted inspection"), ReasonParent(ReasonForTheTransferTypes.Dfe)]
            Forced,
            [Display(Name = "Finance"), ReasonParent(ReasonForTheTransferTypes.Dfe)]
            Finance,
            [Display(Name = "Safeguarding"), ReasonParent(ReasonForTheTransferTypes.Dfe)]
            Safeguarding,
            [Display(Name = "Sponsor/Trust closed"), ReasonParent(ReasonForTheTransferTypes.Dfe)]
            TrustClosed,

            [Display(Name = "Voluntary transfers"), ReasonParent(ReasonForTheTransferTypes.OutgoingTrust)]
            VoluntaryTransfer,          
            [Display(Name = "Strategic consolidations"), ReasonParent(ReasonForTheTransferTypes.OutgoingTrust)]
            Strategic,

            [Display(Name = "Voluntary closures"), ReasonParent(ReasonForTheTransferTypes.SponsorOrTrustClosure)]
            VoluntaryClosure,
            [Display(Name = "Voluntary closures following intervention"), ReasonParent(ReasonForTheTransferTypes.SponsorOrTrustClosure)]
            VoluntaryClosureIntervention,
            [Display(Name = "Intervention closure"), ReasonParent(ReasonForTheTransferTypes.SponsorOrTrustClosure)]
            InterventionClosure,
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
        public List<SpecificReasonForTheTransferTypes> SpecificReasonsForTheTransfer { get; set; } = new();
        public TransferTypes TypeOfTransfer { get; set; }
        public string OtherTypeOfTransfer { get; set; }
        public bool? IsCompleted { get; set; }
    }
}