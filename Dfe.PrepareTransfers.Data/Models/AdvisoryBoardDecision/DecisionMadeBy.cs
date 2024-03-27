using System.ComponentModel;

namespace Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;

public enum DecisionMadeBy
{
   [Description("Regional Director for the region")]
   RegionalDirectorForRegion = 0,

   [Description("A different Regional Director")]
   OtherRegionalDirector = 1,
   [Description("Minister")] Minister = 2,
   [Description("Director General")] DirectorGeneral = 3,
   [Description("None")] None = 4
}
