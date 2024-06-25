namespace Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject
{
    public class AcademyTransferProjectDates
    {
        public string HtbDate { get; set; }
        public string PreviousAdvisoryBoardDate { get; set; }
        public bool? HasHtbDate { get; set; }
        public string TargetDateForTransfer { get; set; }
        public bool? HasTargetDateForTransfer { get; set; }
        public bool? IsCompleted { get; set; }
    }
}