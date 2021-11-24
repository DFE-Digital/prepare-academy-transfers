using System.Linq;
using System.Threading.Tasks;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Models.AcademyAndTrustInformation;
using Frontend.Models.Benefits;
using Frontend.Models.Features;
using Frontend.Models.TransferDates;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.TaskList.HtbDocument
{
    //todo: remove models and add view models
    public class Preview : ProjectPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        public string ProjectUrn => Project.Urn;
        public object OutgoingAcademyUrn => TransferringAcademy.Urn;

        public FeaturesSummaryViewModel FeaturesSummaryViewModel { get; set; }
        public BenefitsSummaryViewModel BenefitsSummaryViewModel { get; set; }
        public TransferDatesSummaryViewModel TransferDatesSummaryViewModel { get; set; }
        public AcademyAndTrustInformationSummaryViewModel AcademyAndTrustInformationSummaryViewModel { get; set; }
        
        public Preview(IGetInformationForProject getInformationForProject)
        {
            _getInformationForProject = getInformationForProject;
        }

        public async Task<IActionResult> OnGet(string id)
        {
            var response = await _getInformationForProject.Execute(id);
            Project = response.Project;
            TransferringAcademy = response.OutgoingAcademy;
            EducationPerformance = response.EducationPerformance;

            FeaturesSummaryViewModel = new FeaturesSummaryViewModel
            {
                Urn = Project.Urn,
                IsSubjectToRddOrEsfaIntervention = Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention,
                TypeOfTransfer = Project.Features.TypeOfTransfer,
                OtherTypeOfTransfer = Project.Features.OtherTypeOfTransfer,
                OutgoingAcademyUrn = Project.OutgoingAcademyUrn,
                WhoInitiatedTheTransfer = Project.Features.WhoInitiatedTheTransfer,
                InterventionDetails = Project.Features.ReasonForTransfer.InterventionDetails,
                ReturnToPreview = true
            };

            BenefitsSummaryViewModel = new BenefitsSummaryViewModel(
                Project.Benefits.IntendedBenefits.ToList(),
                Project.Benefits.OtherIntendedBenefit,
                BenefitsController.BuildOtherFactorsItemViewModel(Project.Benefits.OtherFactors).Where(o => o.Checked)
                    .ToList(),
                Project.Urn,
                Project.OutgoingAcademyUrn
            )
            {
                ReturnToPreview = true
            };

            TransferDatesSummaryViewModel = new TransferDatesSummaryViewModel
            {
                Urn = Project.Urn,
                ReturnToPreview = true,
                OutgoingAcademyUrn = Project.OutgoingAcademyUrn,
                FirstDiscussedDate = Project.Dates.FirstDiscussed,
                HasFirstDiscussedDate = Project.Dates.HasFirstDiscussedDate,
                HtbDate = Project.Dates.Htb,
                HasHtbDate = Project.Dates.HasHtbDate,
                TargetDate = Project.Dates.Target,
                HasTargetDate = Project.Dates.HasTargetDateForTransfer
            };

            AcademyAndTrustInformationSummaryViewModel =
                new AcademyAndTrustInformationSummaryViewModel
                {
                    OutgoingAcademyName = Project.OutgoingAcademyName,
                    Recommendation = Project.AcademyAndTrustInformation.Recommendation,
                    Author = Project.AcademyAndTrustInformation.Author,
                    HtbDate = Project.Dates?.Htb,
                    ProjectName = Project.Name,
                    IncomingTrustName = Project.IncomingTrustName,
                    TargetDate = Project.Dates?.Target,
                    FirstDiscussedDate = Project.Dates?.FirstDiscussed,
                    OutgoingAcademyUrn = Project.OutgoingAcademyUrn,
                    Urn = Project.Urn,
                    ReturnToPreview = true
                };

            return Page();
        }
    }
}