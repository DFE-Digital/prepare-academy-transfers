using System.ComponentModel;

namespace Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;

public enum AdvisoryBoardDeclinedReasons
{
   Finance = 0,
   Performance = 1,
   Governance = 2,
   [Description("Choice of trust")] ChoiceOfTrust = 3,
   Other = 4
}
