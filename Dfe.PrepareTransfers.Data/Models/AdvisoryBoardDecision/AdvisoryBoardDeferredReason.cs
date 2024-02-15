using System.ComponentModel;

namespace Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;

public enum AdvisoryBoardDeferredReason
{
   [Description("Additional information needed")]
   AdditionalInformationNeeded = 0,

   [Description("Awaiting next ofsted report")]
   AwaitingNextOfstedReport = 1,
   [Description("Performance concerns")] PerformanceConcerns = 2,
   [Description("Other")] Other = 3
}
