namespace Data.Models.Projects
{
    public class TransferDates
    {
        public string FirstDiscussed { get; set; }
        public string Target { get; set; }
        public string Htb { get; set; }

        public bool? HasFirstDiscussedDate { get; set; }
        public bool? HasHtbDate { get; set; }
        public bool? HasTargetDateForTransfer { get; set; }
    }
}