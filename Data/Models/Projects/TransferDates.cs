namespace Data.Models.Projects
{
    public class TransferDates
    {
        public string Target { get; set; }
        public string Htb { get; set; }
        public string HtbInserted { get; set; }        

        public bool? HasHtbDate { get; set; }
        public bool? HasHtbInserted { get; set; }
        public bool? HasTargetDateForTransfer { get; set; }
    }
}