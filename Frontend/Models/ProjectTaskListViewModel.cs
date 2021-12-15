using Data.Models;
using Data.Models.Projects;
using System.Linq;
using Data.Models.KeyStagePerformance;
using Frontend.Helpers;
using Frontend.Models.Forms;

namespace Frontend.Models
{
    public class ProjectTaskListViewModel 
    {
        //todo: Remove full project here
        public Project Project { get; set; }
        public EducationPerformance EducationPerformance { get; set; }
        public ProjectStatuses FeatureTransferStatus => GetFeatureTransferStatus();
        public ProjectStatuses TransferDatesStatus => GetTransferDatesStatus();
        public ProjectStatuses BenefitsAndOtherFactorsStatus => GetBenefitsAndOtherFactorsStatus();
        public ProjectStatuses RationaleStatus => GetRationaleStatus();
        public ProjectStatuses AcademyAndTrustInformationStatus() => GetAcademyAndTrustInformationStatus();
        public bool HasKeyStage2PerformanceInformation => PerformanceDataHelpers.HasKeyStage2PerformanceInformation(EducationPerformance.KeyStage2Performance);
        public bool HasKeyStage4PerformanceInformation =>
            PerformanceDataHelpers.HasKeyStage4PerformanceInformation(EducationPerformance.KeyStage4Performance);

        public bool HasKeyStage5PerformanceInformation =>
            PerformanceDataHelpers.HasKeyStage5PerformanceInformation(EducationPerformance.KeyStage5Performance);
        
        //todo:move to a Tasklist Service
        private ProjectStatuses GetAcademyAndTrustInformationStatus()
        {
            var academyAndTrustInformation = Project.AcademyAndTrustInformation;
            return (string.IsNullOrEmpty(academyAndTrustInformation.Author) &&
                    (academyAndTrustInformation.Recommendation ==
                     TransferAcademyAndTrustInformation.RecommendationResult.Empty)) ? ProjectStatuses.NotStarted
                : (!string.IsNullOrEmpty(academyAndTrustInformation.Author) &&
                   (academyAndTrustInformation.Recommendation !=
                    TransferAcademyAndTrustInformation.RecommendationResult.Empty)) ? ProjectStatuses.Completed
                : ProjectStatuses.InProgress;
        }

        private ProjectStatuses GetFeatureTransferStatus()
        {
            if (Project.Features.WhoInitiatedTheTransfer == TransferFeatures.ProjectInitiators.Empty &&
                Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == null &&
                Project.Features.TypeOfTransfer == TransferFeatures.TransferTypes.Empty)
                return ProjectStatuses.NotStarted;

            if (Project.Features.WhoInitiatedTheTransfer != TransferFeatures.ProjectInitiators.Empty &&
                Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention != null &&
                Project.Features.TypeOfTransfer != TransferFeatures.TransferTypes.Empty)
                return ProjectStatuses.Completed;

            return ProjectStatuses.InProgress;
        }

        private ProjectStatuses GetTransferDatesStatus()
        {
            if ((string.IsNullOrEmpty(Project.Dates.FirstDiscussed) && (Project.Dates.HasFirstDiscussedDate ?? true)) &&
                (string.IsNullOrEmpty(Project.Dates.Target) && (Project.Dates.HasTargetDateForTransfer ?? true)) &&
                (string.IsNullOrEmpty(Project.Dates.Htb) && (Project.Dates.HasHtbDate ?? true)))
                return ProjectStatuses.NotStarted;

            if ((!string.IsNullOrEmpty(Project.Dates.FirstDiscussed) || Project.Dates.HasFirstDiscussedDate == false) &&
                (!string.IsNullOrEmpty(Project.Dates.Target) || Project.Dates.HasTargetDateForTransfer == false) &&
                (!string.IsNullOrEmpty(Project.Dates.Htb) || Project.Dates.HasHtbDate == false))
                return ProjectStatuses.Completed;

            return ProjectStatuses.InProgress;
        }

        private ProjectStatuses GetBenefitsAndOtherFactorsStatus()
        {
            if ((Project.Benefits.IntendedBenefits == null || !Project.Benefits.IntendedBenefits.Any()) &&
                (Project.Benefits.OtherFactors == null || !Project.Benefits.OtherFactors.Any()))
                return ProjectStatuses.NotStarted;

            if ((Project.Benefits.IntendedBenefits != null && Project.Benefits.IntendedBenefits.Any()) &&
                (Project.Benefits.OtherFactors != null && Project.Benefits.OtherFactors.Any()))
                return ProjectStatuses.Completed;

            return ProjectStatuses.InProgress;
        }

        private ProjectStatuses GetRationaleStatus()
        {
            if (string.IsNullOrEmpty(Project.Rationale.Project) &&
                string.IsNullOrEmpty(Project.Rationale.Trust))
                return ProjectStatuses.NotStarted;

            if (!string.IsNullOrEmpty(Project.Rationale.Project) &&
                !string.IsNullOrEmpty(Project.Rationale.Trust))
                return ProjectStatuses.Completed;

            return ProjectStatuses.InProgress;
        }
    }
}