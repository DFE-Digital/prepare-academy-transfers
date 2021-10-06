namespace Data.TRAMS.Models.AcademyTransferProject
{
    public class AcademyTransferProjectDates
    {
        public string HtbDate { get; set; }
        public bool HasHtbDate { get; set; } = true;
        public string TargetDateForTransfer { get; set; }
        public bool HasTargetDateForTransfer { get; set; } = true;
        public string TransferFirstDiscussed { get; set; }
        public bool HasTransferFirstDiscussedDate { get; set; } = true;
    }
}