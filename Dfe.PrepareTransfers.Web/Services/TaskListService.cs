using System;
using System.Linq;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Dfe.PrepareTransfers.Helpers;

namespace Dfe.PrepareTransfers.Web.Services
{
    public class TaskListService : ITaskListService
    {
        private readonly IProjects _projectRepository;

        public TaskListService(IProjects projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public void BuildTaskListStatuses(Pages.Projects.Index indexPage)
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
            indexPage.LegalRequirementsStatus = GetLegalRequirementsStatus(project.Result);
            indexPage.RationaleStatus = GetRationaleStatus(project.Result);
            indexPage.AssignedUser = project.Result.AssignedUser;
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
            if (project.Features.ReasonForTheTransfer == TransferFeatures.ReasonForTheTransferTypes.Empty &&
                                                         project.Features.TypeOfTransfer == TransferFeatures.TransferTypes.Empty)
                return ProjectStatuses.NotStarted;

            return project.Features.IsCompleted == true ? ProjectStatuses.Completed : ProjectStatuses.InProgress;
        }

        private ProjectStatuses GetTransferDatesStatus(Project project)
        {
            if ((string.IsNullOrEmpty(project.Dates.Target) && (project.Dates.HasTargetDateForTransfer ?? true)) &&
                (string.IsNullOrEmpty(project.Dates.Htb) && (project.Dates.HasHtbDate ?? true)))
                return ProjectStatuses.NotStarted;

            if ((!string.IsNullOrEmpty(project.Dates.Target) || project.Dates.HasTargetDateForTransfer == false) &&
                (!string.IsNullOrEmpty(project.Dates.Htb) || project.Dates.HasHtbDate == false))
                return ProjectStatuses.Completed;

            return ProjectStatuses.InProgress;
        }

        private ProjectStatuses GetBenefitsAndOtherFactorsStatus(Project project)
        {
            if ((project.Benefits.IntendedBenefits == null || !project.Benefits.IntendedBenefits.Any()) &&
                (project.Benefits.OtherFactors == null || !project.Benefits.OtherFactors.Any()))
                return ProjectStatuses.NotStarted;

            return project.Benefits.IsCompleted == true ? ProjectStatuses.Completed : ProjectStatuses.InProgress;
        }
        private ProjectStatuses GetLegalRequirementsStatus(Project project)
        {
            if (project.LegalRequirements.DiocesanConsent == null &&
                    project.LegalRequirements.IncomingTrustAgreement == null &&
                    project.LegalRequirements.OutgoingTrustConsent == null &&
                    project.LegalRequirements.IsCompleted == null)
            {
                return ProjectStatuses.NotStarted;
            }
            return project.LegalRequirements.IsCompleted == true ? ProjectStatuses.Completed : ProjectStatuses.InProgress;
        }

        private ProjectStatuses GetRationaleStatus(Project project)
        {
            if (string.IsNullOrEmpty(project.Rationale.Project) &&
                string.IsNullOrEmpty(project.Rationale.Trust))
                return ProjectStatuses.NotStarted;

            return project.Rationale.IsCompleted == true ? ProjectStatuses.Completed : ProjectStatuses.InProgress;
        }
    }
}