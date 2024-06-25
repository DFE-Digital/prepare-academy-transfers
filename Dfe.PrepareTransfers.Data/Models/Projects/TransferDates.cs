namespace Dfe.PrepareTransfers.Data.Models.Projects
{
    public class TransferDates
    {
        public string Target { get; set; }
        public string Htb { get; set; }
        public string PreviousAdvisoryBoardDate { get; set; }


        public bool? HasHtbDate { get; set; }
        public bool? HasTargetDateForTransfer { get; set; }
        public bool? IsCompleted { get; set; }
    }
}