using Helpers;
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

        public string QualityOfEducation { get; set; }
        public string BehaviourAndAttitudes { get; set; }
        public string PersonalDevelopment { get; set; }
        public string EffectivenessOfLeadershipAndManagement { get; set; }
        public string InspectionEndDate { get; set; }

        public string EarlyYearsProvision { get; set; }
        public bool EarlyYearsProvisionApplicable { get => OfstedRatingHasData(EarlyYearsProvision); }
        public string SixthFormProvision { get; set; }
        public bool SixthFormProvisionApplicable { get => OfstedRatingHasData(SixthFormProvision); }

        public string DateOfLatestInspection
        {
            // We have previously been using InspectionDate as the date of the latest full inspection,
            // however, it's possible for InspectionDate to be null while InspectionEndDate matches
            // the latest report.
            get => InspectionDate ?? InspectionEndDate;
        }

        public string DateOfLatestSection8Inspection { get; set; }

        public string AdditionalInformation { get; set; }

        public bool LatestInspectionIsSection8
        {
            // InspectionDate can refer to either a full inspection or a section 8 inspection.
            get
            {
                if (DateOfLatestSection8Inspection == null || DateOfLatestInspection == null)
                {
                    return false;
                }

                return DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(DateOfLatestSection8Inspection, DateOfLatestInspection);
            }
        }

        private bool OfstedRatingHasData(string ofstedRating)
        {
            return !(ofstedRating == "No data" || ofstedRating == "N/A");
        }
    }
}