using System.ComponentModel;

namespace Data.Models.Academies
{
    public class LatestOfstedJudgement
    {
        [DisplayName("School name")]
        public string SchoolName { get; set; }
        
        [DisplayName("Ofsted inspection date")]
        public string InspectionDate { get; set; }
        
        [DisplayName("Overall effectiveness")]
        public string OverallEffectiveness { get; set; }
        
        [DisplayName("Ofsted report")]
        public string OfstedReport { get; set; }
    }
}