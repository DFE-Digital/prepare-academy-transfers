namespace Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;

public class AdvisoryBoardDeferredReasonDetails
{
   public AdvisoryBoardDeferredReasonDetails()
   {
   }

   public AdvisoryBoardDeferredReasonDetails(AdvisoryBoardDeferredReason reason, string details)
   {
      Reason = reason;
      Details = details;
   }

   public AdvisoryBoardDeferredReason Reason { get; set; }
   public string Details { get; set; }
}
