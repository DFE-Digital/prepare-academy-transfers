using System.Linq;
using System.Threading.Tasks;
using Data;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Models.Benefits;
using Frontend.Models.Features;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Frontend.Pages.Projects;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Index = Frontend.Pages.Projects.Features.Index;
using LatestOfstedJudgementIndex = Frontend.Pages.Projects.LatestOfstedJudgement.Index;

namespace Frontend.Pages.TaskList.HtbDocument
{
    public class Preview : ProjectPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projects;
        public string ProjectUrn => Project.Urn;
        public object OutgoingAcademyUrn => TransferringAcademy.Urn;

        public Index FeaturesSummaryViewModel { get; set; }
        public BenefitsSummaryViewModel BenefitsSummaryViewModel { get; set; }
        public TransferDatesSummaryViewModel TransferDatesSummaryViewModel { get; set; }
        public Projects.AcademyAndTrustInformation.Index AcademyAndTrustInformationSummaryViewModel { get; set; }
        public Projects.PupilNumbers PupilNumbersViewModel { get; set; }
        public Projects.GeneralInformation.Index GeneralInformationViewModel { get; set; }
        public LatestOfstedJudgementIndex LatestOfstedJudgementViewModel { get; private set; }
        public Projects.Rationale.Index RationaleSummaryViewModel { get; set; }
        
        public Preview(IGetInformationForProject getInformationForProject, IProjects projects)
        {
            _getInformationForProject = getInformationForProject;
            _projects = projects;
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
                Projects.Benefits.OtherFactors.BuildOtherFactorsItemViewModel(Project.Benefits.OtherFactors).Where(o => o.Checked)
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

            PupilNumbersViewModel = new PupilNumbers(_getInformationForProject, _projects)
            {
                GirlsOnRoll = response.OutgoingAcademy.PupilNumbers.GirlsOnRoll,
                BoysOnRoll = response.OutgoingAcademy.PupilNumbers.BoysOnRoll,
                WithStatementOfSEN = response.OutgoingAcademy.PupilNumbers.WithStatementOfSen,
                WithEAL = response.OutgoingAcademy.PupilNumbers.WhoseFirstLanguageIsNotEnglish,
                FreeSchoolMealsLast6Years = response.OutgoingAcademy.PupilNumbers
                    .PercentageEligibleForFreeSchoolMealsDuringLast6Years,
                OutgoingAcademyUrn = response.OutgoingAcademy.Urn,
                OutgoingAcademyName = response.OutgoingAcademy.Name,
                IsPreview = true,
                Urn = response.Project.Urn,
                ReturnToPreview = true,
                AdditionalInformationViewModel = new AdditionalInformationViewModel
                {
                    AdditionalInformation = response.Project.PupilNumbersAdditionalInformation,
                    HintText =
                        "If you add comments, they'll be included in the pupil numbers section of your project template.",
                    Urn = response.Project.Urn,
                    ReturnToPreview = true
                }
            };

            LatestOfstedJudgementViewModel = new LatestOfstedJudgementIndex(_getInformationForProject, _projects)
            {
                Urn = Project.Urn,
                OutgoingAcademyUrn = Project.OutgoingAcademyUrn,
                OutgoingAcademyName = Project.OutgoingAcademyName,
                SchoolName = TransferringAcademy.LatestOfstedJudgement.SchoolName,
                InspectionDate = TransferringAcademy.LatestOfstedJudgement.InspectionDate,
                OverallEffectiveness = TransferringAcademy.LatestOfstedJudgement.OverallEffectiveness,
                OfstedReport = TransferringAcademy.LatestOfstedJudgement.OfstedReport,
                AdditionalInformationViewModel = new AdditionalInformationViewModel
                {
                    AdditionalInformation = Project.LatestOfstedJudgementAdditionalInformation,
                    HintText =
                        "If you add comments, they'll be included in the latest Ofsted judgement section of your project template.",
                    Urn = Project.Urn,
                    ReturnToPreview = true
                },
                IsPreview = true
            };

            return Page();
        }
    }
}