namespace Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;

public class AdvisoryBoardWithdrawnReasonDetails
{
   public AdvisoryBoardWithdrawnReasonDetails()
   {
   }

   public AdvisoryBoardWithdrawnReasonDetails(AdvisoryBoardWithdrawnReason reason, string details)
   {
      Reason = reason;
      Details = details;
   }

   public AdvisoryBoardWithdrawnReason Reason { get; set; }
   public string Details { get; set; }
}
