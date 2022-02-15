using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Frontend.Models;
using Frontend.Models.Benefits;
using Frontend.Models.Forms;
using Frontend.Pages.Projects;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Index = Frontend.Pages.Projects.Features.Index;
using LatestOfstedJudgementIndex = Frontend.Pages.Projects.LatestOfstedJudgement.Index;

namespace Frontend.Pages.TaskList.HtbDocument
{
    public class Preview : CommonPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projects;
        public Index FeaturesSummaryViewModel { get; set; }
        public BenefitsSummaryViewModel BenefitsSummaryViewModel { get; set; }
        public Projects.TransferDates.Index TransferDatesSummaryViewModel { get; set; }
        public Projects.AcademyAndTrustInformation.Index AcademyAndTrustInformationSummaryViewModel { get; set; }
        public Projects.PupilNumbers PupilNumbersViewModel { get; set; }
        public Projects.GeneralInformation.Index GeneralInformationViewModel { get; set; }
        public LatestOfstedJudgementIndex LatestOfstedJudgementViewModel { get; private set; }
        public Projects.Rationale.Index RationaleSummaryViewModel { get; set; }
        public EducationPerformance EducationPerformance { get; set; }
        public Academy Academy { get; set; }
        public string KeyStage2AdditionalInformation { get; set; }

        public Preview(IGetInformationForProject getInformationForProject, IProjects projects)
        {
            _getInformationForProject = getInformationForProject;
            _projects = projects;
        }

        public async Task<IActionResult> OnGet()
        {
            var response = await _getInformationForProject.Execute(Urn);
            
            var project = response.Project;
            Academy = response.OutgoingAcademy;
            EducationPerformance = response.EducationPerformance;

            // todo: refactor required (create viewmodel/viewcomponent)
            KeyStage2AdditionalInformation = response.Project.KeyStage2PerformanceAdditionalInformation;

            FeaturesSummaryViewModel = new Index(null)
            {
                Urn = project.Urn,
                IsSubjectToRddOrEsfaIntervention = project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention,
                TypeOfTransfer = project.Features.TypeOfTransfer,
                OtherTypeOfTransfer = project.Features.OtherTypeOfTransfer,
                OutgoingAcademyUrn = project.OutgoingAcademyUrn,
                WhoInitiatedTheTransfer = project.Features.WhoInitiatedTheTransfer,
                InterventionDetails = project.Features.ReasonForTransfer.InterventionDetails,
                ReturnToPreview = true
            };

            BenefitsSummaryViewModel = new BenefitsSummaryViewModel(
                project.Benefits.IntendedBenefits.ToList(),
                project.Benefits.OtherIntendedBenefit,
                Projects.Benefits.OtherFactors.BuildOtherFactorsItemViewModel(project.Benefits.OtherFactors).Where(o => o.Checked)
                    .ToList(),
                project.Urn,
                project.OutgoingAcademyUrn
            )
            {
                ReturnToPreview = true
            };

            TransferDatesSummaryViewModel = new Pages.Projects.TransferDates.Index(_projects)
            {
                Urn = project.Urn,
                ReturnToPreview = true,
                OutgoingAcademyUrn = project.OutgoingAcademyUrn,
                FirstDiscussedDate = project.Dates.FirstDiscussed,
                HasFirstDiscussedDate = project.Dates.HasFirstDiscussedDate,
                AdvisoryBoardDate = project.Dates.Htb,
                HasAdvisoryBoardDate = project.Dates.HasHtbDate,
                TargetDate = project.Dates.Target,
                HasTargetDate = project.Dates.HasTargetDateForTransfer
            };
            
            AcademyAndTrustInformationSummaryViewModel =
                new Pages.Projects.AcademyAndTrustInformation.Index(_getInformationForProject)
                {
                    Recommendation = project.AcademyAndTrustInformation.Recommendation,
                    Author = project.AcademyAndTrustInformation.Author,
                    AdvisoryBoardDate = project.Dates?.Htb,
                    IncomingTrustName = project.IncomingTrustName,
                    TargetDate = project.Dates?.Target,
                    FirstDiscussedDate = project.Dates?.FirstDiscussed,
                    OutgoingAcademyUrn = project.OutgoingAcademyUrn,
                    Urn = project.Urn,
                    ReturnToPreview = true
                };

            var generalInformation = Academy.GeneralInformation;
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
                ProjectRationale = project.Rationale.Project,
                TrustRationale = project.Rationale.Trust,
                OutgoingAcademyUrn = project.OutgoingAcademyUrn,
                Urn = project.Urn,
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
                AcademyUkprn = response.OutgoingAcademy.Ukprn,
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
                Urn = project.Urn,
                OutgoingAcademyUrn = project.OutgoingAcademyUrn,
                AcademyUkprn = response.OutgoingAcademy.Ukprn,
                SchoolName = Academy.LatestOfstedJudgement.SchoolName,
                InspectionDate = Academy.LatestOfstedJudgement.InspectionDate,
                OverallEffectiveness = Academy.LatestOfstedJudgement.OverallEffectiveness,
                OfstedReport = Academy.LatestOfstedJudgement.OfstedReport,
                AdditionalInformationViewModel = new AdditionalInformationViewModel
                {
                    AdditionalInformation = project.LatestOfstedJudgementAdditionalInformation,
                    HintText =
                        "If you add comments, they'll be included in the latest Ofsted judgement section of your project template.",
                    Urn = project.Urn,
                    ReturnToPreview = true
                },
                IsPreview = true
            };

            return Page();
        }
    }
}