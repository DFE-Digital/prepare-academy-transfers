using System.Collections.Generic;

namespace Data.Models.Academies
{
    public class GeneralInformation
    {
        public string SchoolPhase { get; set; }
        public string AgeRange { get; set; }
        public string Capacity { get; set; }
        public string NumberOnRoll { get; set; }
        public string PercentageFull { get; set; }
        public string Pan { get; set; }
        public string Pfi { get; set; }
        public string ViabilityIssue { get; set; }
        public string Deficit { get; set; }
        public string SchoolType { get; set; }
        public string DiocesesPercent { get; set; }
        public string DistanceToSponsorHq { get; set; }
        public string MpAndParty { get; set; }
        public string PercentageFsm { get; set; }

        public IEnumerable<FormField> FieldsToDisplay()
        {
            return new List<FormField>
            {
                new FormField {Title = "School phase", Value = SchoolPhase},
                new FormField {Title = "Age range", Value = AgeRange},
                new FormField {Title = "Capacity", Value = Capacity},
                new FormField
                {
                    Title = "Number on roll (percentage the school is full)",
                    Value = $"{NumberOnRoll} ({PercentageFull}%)"
                },
                new FormField {Title = "Percentage of free school meals (%FSM)", Value = PercentageFsm},
                new FormField {Title = "Published admission number (PAN)", Value = Pan},
                new FormField {Title = "Private finance initiative (PFI) scheme", Value = Pfi},
                new FormField {Title = "Viability issues", Value = ViabilityIssue},
                new FormField {Title = "Financial deficit", Value = Deficit},
                new FormField {Title = "School type", Value = SchoolType},
                new FormField
                {
                    Title = "Percentage of good or outstanding academies in the diocesan trust", Value = DiocesesPercent
                },
                new FormField
                    {Title = "Distance from the academy to the trust headquarters", Value = DistanceToSponsorHq},
                new FormField {Title = "MP (Party)", Value = MpAndParty},
            };
        }
    }
}