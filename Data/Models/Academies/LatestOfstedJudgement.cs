using System.Collections.Generic;
using System.ComponentModel;

namespace Data.Models.Academies
{
    public class LatestOfstedJudgement
    {
        public string SchoolName { get; set; }
        public string InspectionDate { get; set; }
        public string OverallEffectiveness { get; set; }
        
        [DisplayName("Ofsted Report")]
        public string OfstedReport { get; set; }

        public IEnumerable<FormField> FieldsToDisplay()
        {
            return new List<FormField>
            {
                new FormField {Title = "School name", Value = SchoolName},
                new FormField {Title = "Ofsted inspection date", Value = InspectionDate},
                new FormField {Title = "Overall effectiveness", Value = OverallEffectiveness}
            };
        }
    }
}