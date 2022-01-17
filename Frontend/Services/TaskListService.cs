using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Data.Models.Projects;
using Frontend.Helpers;
using Frontend.Models;

namespace Frontend.Services
{
    public class TaskListService : ITaskListService
    {
        private readonly IProjects _projectRepository;
        private readonly IEducationPerformance _projectRepositoryEducationPerformance;

        public TaskListService(IProjects projectRepository,
            IEducationPerformance projectRepositoryEducationPerformance)
        {
            _projectRepository = projectRepository;
            _projectRepositoryEducationPerformance = projectRepositoryEducationPerformance;
        }
        
        public async Task<ProjectTaskListViewModel> BuildTaskListStatusesAsync(string urn)
        {  
            var project = await _projectRepository.GetByUrn(urn);
            var educationPerformance =
                await _projectRepositoryEducationPerformance.GetByAcademyUrn(project.Result.OutgoingAcademyUrn);
            
            var vm = new ProjectTaskListViewModel
            {
                AcademyAndTrustInformationStatus = GetAcademyAndTrustInformationStatus(project.Result),
                FeatureTransferStatus = GetFeatureTransferStatus(project.Result),
                TransferDatesStatus = GetTransferDatesStatus(project.Result),
                BenefitsAndOtherFactorsStatus = GetBenefitsAndOtherFactorsStatus(project.Result),
                RationaleStatus = GetRationaleStatus(project.Result),
                HasKeyStage2PerformanceInformation = PerformanceDataHelpers.HasKeyStage2PerformanceInformation(educationPerformance.Result.KeyStage2Performance),
                HasKeyStage4PerformanceInformation = PerformanceDataHelpers.HasKeyStage4PerformanceInformation(educationPerformance.Result.KeyStage4Performance),
                HasKeyStage5PerformanceInformation = PerformanceDataHelpers.HasKeyStage5PerformanceInformation(educationPerformance.Result.KeyStage5Performance)
            };
            return vm;
        }
        
        private ProjectStatuses GetAcademyAndTrustInformationStatus(Project project)
        {
            var academyAndTrustInformation = project.AcademyAndTrustInformation;
            return (string.IsNullOrEmpty(academyAndTrustInformation.Author) &&
                    (academyAndTrustInformation.Recommendation ==
                     TransferAcademyAndTrustInformation.RecommendationResult.Empty)) ? ProjectStatuses.NotStarted
                : (!string.IsNullOrEmpty(academyAndTrustInformation.Author) &&
                   (academyAndTrustInformation.Recommendation !=
                    TransferAcademyAndTrustInformation.RecommendationResult.Empty)) ? ProjectStatuses.Completed
                : ProjectStatuses.InProgress;
        }

        private ProjectStatuses GetFeatureTransferStatus(Project project)
        {
            if (project.Features.WhoInitiatedTheTransfer == TransferFeatures.ProjectInitiators.Empty &&
                project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == null &&
                project.Features.TypeOfTransfer == TransferFeatures.TransferTypes.Empty)
                return ProjectStatuses.NotStarted;

            if (project.Features.WhoInitiatedTheTransfer != TransferFeatures.ProjectInitiators.Empty &&
                project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention != null &&
                project.Features.TypeOfTransfer != TransferFeatures.TransferTypes.Empty)
                return ProjectStatuses.Completed;

            return ProjectStatuses.InProgress;
        }

        private ProjectStatuses GetTransferDatesStatus(Project project)
        {
            if ((string.IsNullOrEmpty(project.Dates.FirstDiscussed) && (project.Dates.HasFirstDiscussedDate ?? true)) &&
                (string.IsNullOrEmpty(project.Dates.Target) && (project.Dates.HasTargetDateForTransfer ?? true)) &&
                (string.IsNullOrEmpty(project.Dates.Htb) && (project.Dates.HasHtbDate ?? true)))
                return ProjectStatuses.NotStarted;

            if ((!string.IsNullOrEmpty(project.Dates.FirstDiscussed) || project.Dates.HasFirstDiscussedDate == false) &&
                (!string.IsNullOrEmpty(project.Dates.Target) || project.Dates.HasTargetDateForTransfer == false) &&
                (!string.IsNullOrEmpty(project.Dates.Htb) || project.Dates.HasHtbDate == false))
                return ProjectStatuses.Completed;

            return ProjectStatuses.InProgress;
        }

        private ProjectStatuses GetBenefitsAndOtherFactorsStatus(Project project)
        {
            if ((project.Benefits.IntendedBenefits == null || !project.Benefits.IntendedBenefits.Any()) &&
                (project.Benefits.OtherFactors == null || !project.Benefits.OtherFactors.Any()))
                return ProjectStatuses.NotStarted;

            if ((project.Benefits.IntendedBenefits != null && project.Benefits.IntendedBenefits.Any()) &&
                (project.Benefits.OtherFactors != null && project.Benefits.OtherFactors.Any()))
                return ProjectStatuses.Completed;

            return ProjectStatuses.InProgress;
        }

        private ProjectStatuses GetRationaleStatus(Project project)
        {
            if (string.IsNullOrEmpty(project.Rationale.Project) &&
                string.IsNullOrEmpty(project.Rationale.Trust))
                return ProjectStatuses.NotStarted;

            if (!string.IsNullOrEmpty(project.Rationale.Project) &&
                !string.IsNullOrEmpty(project.Rationale.Trust))
                return ProjectStatuses.Completed;

            return ProjectStatuses.InProgress;
        }
    }

    public interface ITaskListService
    {
        Task<ProjectTaskListViewModel> BuildTaskListStatusesAsync(string urn);
    }
}