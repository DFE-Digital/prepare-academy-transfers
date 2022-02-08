using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Data.Models.Projects;
using Frontend.ExtensionMethods;
using Frontend.Helpers;
using Frontend.Models;
using Frontend.Services.Interfaces;

namespace Frontend.Services
{
    public class TaskListService : ITaskListService
    {
        private readonly IProjects _projectRepository;

        public TaskListService(IProjects projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public void BuildTaskListStatuses(Frontend.Pages.Projects.Index indexPage)
        {
            var project = _projectRepository.GetByUrn(indexPage.Urn).Result;
            indexPage.ProjectReference = project.Result.Reference;
            indexPage.IncomingTrustName = project.Result.IncomingTrustName.ToTitleCase();
            indexPage.Academies = project.Result.TransferringAcademies
                .Select(a => new Tuple<string, string>(a.OutgoingAcademyUkprn,a.OutgoingAcademyName)).ToList();
            indexPage.AcademyAndTrustInformationStatus = GetAcademyAndTrustInformationStatus(project.Result);
            indexPage.FeatureTransferStatus = GetFeatureTransferStatus(project.Result);
            indexPage.TransferDatesStatus = GetTransferDatesStatus(project.Result);
            indexPage.BenefitsAndOtherFactorsStatus = GetBenefitsAndOtherFactorsStatus(project.Result);
            indexPage.RationaleStatus = GetRationaleStatus(project.Result);
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
}