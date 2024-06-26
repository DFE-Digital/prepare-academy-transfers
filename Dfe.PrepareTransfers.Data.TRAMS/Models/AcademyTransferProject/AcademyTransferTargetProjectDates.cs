using Dfe.PrepareTransfers.Data.Models;
using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject
{
    public class AcademyTransferTargetProjectDates
    {
        public string HtbDate { get; set; }
        public string PreviousAdvisoryBoardDate { get; set; }
        public bool? HasHtbDate { get; set; }
        public string TargetDateForTransfer { get; set; }
        public bool? HasTargetDateForTransfer { get; set; }
        public bool? IsCompleted { get; set; }
        public string? ChangedBy { get; set; }
        public List<ReasonChange>? ReasonsChanged { get; set; }
    }
}