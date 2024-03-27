using System;
using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;

public class AdvisoryBoardDecision
{
   private AdvisoryBoardDecisions? _decision;

   public AdvisoryBoardDecision()
   {
      DeferredReasons = new List<AdvisoryBoardDeferredReasonDetails>();
      DeclinedReasons = new List<AdvisoryBoardDeclinedReasonDetails>();
      WithdrawnReasons = new List<AdvisoryBoardWithdrawnReasonDetails>();
    }

   public int AdvisoryBoardDecisionId { get; set; }
   public int TransferProjectId { get; set; }
   public bool? ApprovedConditionsSet { get; set; }
   public string ApprovedConditionsDetails { get; set; }
   public List<AdvisoryBoardDeclinedReasonDetails> DeclinedReasons { get; set; }
   public List<AdvisoryBoardDeferredReasonDetails> DeferredReasons { get; set; }
   public List<AdvisoryBoardWithdrawnReasonDetails> WithdrawnReasons { get; set; }
    public DateTime? AdvisoryBoardDecisionDate { get; set; }
   public DecisionMadeBy? DecisionMadeBy { get; set; }


   public AdvisoryBoardDecisions? Decision
   {
      get => _decision;
      set
      {
         if (value != _decision)
         {
            if (value == AdvisoryBoardDecisions.Approved)
            {
               DeclinedReasons = new List<AdvisoryBoardDeclinedReasonDetails>();
               DeferredReasons = new List<AdvisoryBoardDeferredReasonDetails>();
               WithdrawnReasons = new List<AdvisoryBoardWithdrawnReasonDetails>();
            }

            if (value == AdvisoryBoardDecisions.Declined)
            {
               ApprovedConditionsSet = null;
               ApprovedConditionsDetails = null;
               DeferredReasons = new List<AdvisoryBoardDeferredReasonDetails>();
               WithdrawnReasons = new List<AdvisoryBoardWithdrawnReasonDetails>();
            }

            if (value == AdvisoryBoardDecisions.Deferred)
            {
               ApprovedConditionsSet = null;
               ApprovedConditionsDetails = null;
               DeclinedReasons = new List<AdvisoryBoardDeclinedReasonDetails>();
               WithdrawnReasons = new List<AdvisoryBoardWithdrawnReasonDetails>();
            }

            if (value == AdvisoryBoardDecisions.Withdrawn)
            {
                ApprovedConditionsSet = null;
                ApprovedConditionsDetails = null;
                DeferredReasons = new List<AdvisoryBoardDeferredReasonDetails>();
                DeclinedReasons = new List<AdvisoryBoardDeclinedReasonDetails>();
            }
            }

         _decision = value;
      }
   }

   public string GetDecisionAsFriendlyName()
   {
      return this switch
      {
         { Decision: AdvisoryBoardDecisions.Approved, ApprovedConditionsSet: true } => "Approved with Conditions",
         _ => Decision?.ToString()
      };
   }
}
