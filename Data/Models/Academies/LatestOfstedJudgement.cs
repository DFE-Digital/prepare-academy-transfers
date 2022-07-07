using Helpers;
using System.ComponentModel;

namespace Data.Models.Academies
{
    public class LatestOfstedJudgement
    {
        [DisplayName("School name")]
        public string SchoolName { get; set; }

        [DisplayName("Overall effectiveness")]
        public string OverallEffectiveness { get; set; }

        [DisplayName("Ofsted report")]
        public string OfstedReport { get; set; }

        public string QualityOfEducation { get; set; }
        public string BehaviourAndAttitudes { get; set; }
        public string PersonalDevelopment { get; set; }
        public string EffectivenessOfLeadershipAndManagement { get; set; }

        // InspectionEndDate always refers to a full inspection.
        public string InspectionEndDate { get; set; }

        public string EarlyYearsProvision { get; set; }
        public bool EarlyYearsProvisionApplicable { get => OfstedRatingHasData(EarlyYearsProvision); }
        public string SixthFormProvision { get; set; }
        public bool SixthFormProvisionApplicable { get => OfstedRatingHasData(SixthFormProvision); }

        public string DateOfLatestSection8Inspection { get; set; }

        public string AdditionalInformation { get; set; }

        public bool LatestInspectionIsSection8
        {
            get
            {
                if (InspectionEndDate == null)
                {
                    return DateOfLatestSection8Inspection != null;
                }

                if (DateOfLatestSection8Inspection == null)
                {
                    return false;
                }

                return DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(DateOfLatestSection8Inspection, InspectionEndDate);
            }
        }

        private bool OfstedRatingHasData(string ofstedRating)
        {
            return !(ofstedRating == "No data" || ofstedRating == "N/A");
        }
    }
}