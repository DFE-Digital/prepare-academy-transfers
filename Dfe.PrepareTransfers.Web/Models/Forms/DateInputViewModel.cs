namespace Dfe.PrepareTransfers.Web.Models.Forms
{
    public class DateInputViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Label { get; set; }
        public bool HeadingLabel { get; set; }
        public string Hint { get; set; }
        public string ErrorMessage { get; set; }
        public bool DayInvalid { get; set; }
        public bool MonthInvalid { get; set; }
        public bool YearInvalid { get; set; }
    }
}