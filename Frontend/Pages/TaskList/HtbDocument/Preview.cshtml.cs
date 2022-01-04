using System.Linq;
using System.Threading.Tasks;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Models.Benefits;
using Frontend.Models.Features;
using Frontend.Models.TransferDates;
using Frontend.Pages.Projects.Features;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.TaskList.HtbDocument
{
    public class Preview : ProjectPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        public string ProjectUrn => Project.Urn;
        public object OutgoingAcademyUrn => TransferringAcademy.Urn;

        public Projects.Features.Index FeaturesSummaryViewModel { get; set; }
        public BenefitsSummaryViewModel BenefitsSummaryViewModel { get; set; }
        public TransferDatesSummaryViewModel TransferDatesSummaryViewModel { get; set; }
        public Projects.AcademyAndTrustInformation.Index AcademyAndTrustInformationSummaryViewModel { get; set; }
        public PupilNumbersViewModel PupilNumbersViewModel { get; set; }
        public Projects.GeneralInformation.Index GeneralInformationViewModel { get; set; }
        public LatestOfstedJudgementViewModel LatestOfstedJudgementViewModel { get; private set; }
        public Projects.Rationale.Index RationaleSummaryViewModel { get; set; }
        
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

            FeaturesSummaryViewModel = new Index(null)
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
                Pages.Projects.Benefits.OtherFactors.BuildOtherFactorsItemViewModel(Project.Benefits.OtherFactors).Where(o => o.Checked)
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
                new Pages.Projects.AcademyAndTrustInformation.Index(_getInformationForProject)
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

            var generalInformation = TransferringAcademy.GeneralInformation;
            GeneralInformationViewModel = new Pages.Projects.GeneralInformation.Index(_getInformationForProject)
            {
                SchoolPhase = generalInformation.SchoolPhase,
                AgeRange = generalInformation.AgeRange,
                Capacity = generalInformation.Capacity,
                NumberOnRoll = $"{generalInformation.NumberOnRoll} ({generalInformation.PercentageFull})",
                FreeSchoolMeals = generalInformation.PercentageFsm,
                PublishedAdmissionNumber = generalInformation.Pan,
                PrivateFinanceInitiative = generalInformation.Pfi,
                ViabilityIssues = generalInformation.ViabilityIssue,
                FinancialDeficit = generalInformation.Deficit,
                SchoolType = generalInformation.SchoolType,
                DiocesePercent = generalInformation.DiocesesPercent,
                DistanceFromAcademyToTrustHq = generalInformation.DistanceToSponsorHq,
                MP = generalInformation.MpAndParty
            };

            RationaleSummaryViewModel = new Pages.Projects.Rationale.Index(_getInformationForProject)
            {
                OutgoingAcademyName = Project.OutgoingAcademyName,
                ProjectRationale = Project.Rationale.Project,
                TrustRationale = Project.Rationale.Trust,
                OutgoingAcademyUrn = Project.OutgoingAcademyUrn,
                Urn = Project.Urn,
                ReturnToPreview = true
            };

            PupilNumbersViewModel = PupilNumbersController.BuildViewModel(response,true, true);

            LatestOfstedJudgementViewModel = LatestOfstedJudgementController.BuildViewModel(response, true, true);

            return Page();
        }
    }
}