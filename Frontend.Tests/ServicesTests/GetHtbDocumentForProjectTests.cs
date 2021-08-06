using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.Academies;
using Data.Models.KeyStagePerformance;
using Data.Models.Projects;
using Frontend.Services;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Moq;
using Xunit;

namespace Frontend.Tests.ServicesTests
{
    public class GetHtbDocumentForProjectTests
    {
        private readonly GetHtbDocumentForProject _subject;
        private readonly Mock<IGetInformationForProject> _getInformationForProject;
        private readonly string _projectUrn = "projectId";

        public GetHtbDocumentForProjectTests()
        {
            _getInformationForProject = new Mock<IGetInformationForProject>();
            _subject = new GetHtbDocumentForProject(_getInformationForProject.Object);
        }
        
        public class ExecuteTests : GetHtbDocumentForProjectTests
        {
            [Fact]
            public async void GivenExistingProject_AssignsTheProjectToTheViewModel()
            {
                var foundProject = new Project
                {
                    Urn = _projectUrn,
                    AcademyAndTrustInformation = new TransferAcademyAndTrustInformation
                    {
                        Author = "author",
                        Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Defer
                    },
                    Name = "test name",
                    OutgoingTrustName = "trust name",
                    OutgoingTrustUkprn = "trust ref number",
                    Rationale = new TransferRationale { Project = "project", Trust = "trust" },
                    Dates = new TransferDates { Htb = "01/01/2020", FirstDiscussed = "01/01/2020", Target = "01/01/2020"},
                    Features = new TransferFeatures
                    {
                        WhoInitiatedTheTransfer = TransferFeatures.ProjectInitiators.Dfe,
                        ReasonForTransfer = new ReasonForTransfer
                        {
                            IsSubjectToRddOrEsfaIntervention = true, 
                            InterventionDetails = "intervention"
                        },
                        TypeOfTransfer = TransferFeatures.TransferTypes.SatClosure
                    },
                    Benefits = new TransferBenefits
                    {
                        IntendedBenefits = new List<TransferBenefits.IntendedBenefit>
                        {
                            TransferBenefits.IntendedBenefit.StrengtheningGovernance, 
                            TransferBenefits.IntendedBenefit.StrongerLeadership
                        }, 
                        OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
                        {
                            {TransferBenefits.OtherFactor.HighProfile, "testHighProfile"},
                            {TransferBenefits.OtherFactor.FinanceAndDebtConcerns, "debtConcerns"},
                        }
                    },
                    PupilNumbersAdditionalInformation = "pupil numbers additional info",
                    KeyStage2PerformanceAdditionalInformation = "ks2 additional info",
                    KeyStage4PerformanceAdditionalInformation = "ks4 additional info",
                    KeyStage5PerformanceAdditionalInformation = "ks5 additional info",
                    LatestOfstedJudgementAdditionalInformation = "ofsted additional info",
                    TransferringAcademies = new List<TransferringAcademies> { new TransferringAcademies { IncomingTrustName = "incoming trust name"}}
                };

                var foundAcademy = new Academy
                {
                    Name = "academy Name",
                    Urn = "academy urn",
                    EstablishmentType = "establishment type",
                    GeneralInformation = new GeneralInformation
                    {
                        SchoolPhase = "school phase",
                        AgeRange = "age range",
                        Capacity = "capacity",
                        Pan = "pan",
                        NumberOnRoll = "100",
                        PercentageFull = "100",
                        ViabilityIssue = "viability issues",
                        Pfi = "pfi",
                        Deficit = "deficit",
                        DiocesesPercent = "100",
                        DistanceToSponsorHq = "distance",
                        MpAndParty = "mp",
                        SchoolType = "school type"
                    },
                    PupilNumbers = new PupilNumbers
                    {
                        EligibleForFreeSchoolMeals = "fsm",
                        BoysOnRoll = "50",
                        GirlsOnRoll = "45",
                        WithStatementOfSen = "sen",
                        WhoseFirstLanguageIsNotEnglish = "lan not english"
                    },
                    LatestOfstedJudgement = new LatestOfstedJudgement
                    {
                        InspectionDate = "01/01/2020",
                        OverallEffectiveness = "overall effectiveness",
                        OfstedReport = "ofsted report"
                    },
                    LocalAuthorityName = "LA Name"
                };

                var foundEducationPerformance = new EducationPerformance
                {
                    KeyStage2Performance = new List<KeyStage2> { new KeyStage2 { Year = "2017-2018" }},
                    KeyStage4Performance = new List<KeyStage4> { new KeyStage4 { Year = "2017-2018" }},
                    KeyStage5Performance = new List<KeyStage5> { new KeyStage5 { Year = "2017-2018" }}
                };
                
                _getInformationForProject.Setup(s => s.Execute(_projectUrn)).ReturnsAsync(
                    new GetInformationForProjectResponse()
                    {
                        Project = foundProject,
                        OutgoingAcademy = foundAcademy,
                        EducationPerformance = foundEducationPerformance
                    });

                var result = await _subject.Execute(_projectUrn);
                var htbDocumentResult = result.HtbDocument;
                
                Assert.Equal("Defer", htbDocumentResult.Recommendation);
                Assert.Equal("author", htbDocumentResult.Author);
                Assert.Equal(foundProject.Name, htbDocumentResult.ProjectName);
                Assert.Equal("incoming trust name", htbDocumentResult.SponsorName);
                Assert.Equal(foundAcademy.EstablishmentType, htbDocumentResult.AcademyTypeAndRoute);
                Assert.Equal(foundAcademy.Name, htbDocumentResult.SchoolName);
                Assert.Equal(foundAcademy.Urn, htbDocumentResult.SchoolUrn);
                Assert.Equal(foundProject.OutgoingTrustName, htbDocumentResult.TrustName);
                Assert.Equal(foundProject.OutgoingTrustUkprn, htbDocumentResult.TrustReferenceNumber);
                Assert.Equal(foundAcademy.GeneralInformation.SchoolType, htbDocumentResult.SchoolType);
                Assert.Equal(foundAcademy.GeneralInformation.SchoolPhase, htbDocumentResult.SchoolPhase);
                Assert.Equal(foundAcademy.GeneralInformation.AgeRange, htbDocumentResult.AgeRange);
                Assert.Equal(foundAcademy.GeneralInformation.Capacity, htbDocumentResult.SchoolCapacity);
                Assert.Equal(foundAcademy.GeneralInformation.Pan, htbDocumentResult.PublishedAdmissionNumber);
                Assert.Equal("100 (100%)", htbDocumentResult.NumberOnRoll);
                Assert.Equal(foundAcademy.PupilNumbers.EligibleForFreeSchoolMeals, htbDocumentResult.PercentageFreeSchoolMeals);
                Assert.Equal("1 January 2020", htbDocumentResult.OfstedLastInspection);
                Assert.Equal(foundAcademy.LatestOfstedJudgement.OverallEffectiveness, htbDocumentResult.OverallEffectiveness);
                Assert.Equal(foundProject.Rationale.Project, htbDocumentResult.RationaleForProject);
                Assert.Equal(foundProject.Rationale.Trust, htbDocumentResult.RationaleForTrust);
                Assert.Equal("Cleared by", htbDocumentResult.ClearedBy);
                Assert.Equal("Version", htbDocumentResult.Version);
                Assert.Equal("1 January 2020", htbDocumentResult.DateOfHtb);
                Assert.Equal("1 January 2020", htbDocumentResult.DateOfProposedTransfer);
                Assert.Equal("1 January 2020", htbDocumentResult.DateTransferWasFirstDiscussed);
                Assert.Equal(foundAcademy.GeneralInformation.ViabilityIssue, htbDocumentResult.ViabilityIssues);
                Assert.Equal(foundAcademy.GeneralInformation.Deficit, htbDocumentResult.FinancialDeficit);
                Assert.Equal(foundAcademy.GeneralInformation.Pfi, htbDocumentResult.Pfi);
                Assert.Equal(foundAcademy.GeneralInformation.DiocesesPercent, htbDocumentResult.PercentageGoodOrOutstandingInDiocesanTrust);
                Assert.Equal(foundAcademy.GeneralInformation.DistanceToSponsorHq, htbDocumentResult.DistanceFromTheAcademyToTheTrustHeadquarters);
                Assert.Equal(foundAcademy.GeneralInformation.MpAndParty, htbDocumentResult.MpAndParty);
                Assert.Equal("Department for Education", htbDocumentResult.WhoInitiatedTheTransfer);
                Assert.Equal("Subject to Intervention", htbDocumentResult.ReasonForTransfer);
                Assert.Equal("intervention", htbDocumentResult.MoreDetailsAboutTheTransfer);
                Assert.Equal("Closure of a SAT and the academy joining a MAT", htbDocumentResult.TypeOfTransfer);
                Assert.Equal("Strengthening governance\nStronger leadership\n", htbDocumentResult.TransferBenefits);
                Assert.Equal("This is a high profile transfer (ministers and media could be involved)\ntestHighProfile\nThere are finance and debt concerns\ndebtConcerns\n", htbDocumentResult.OtherFactors);
                Assert.Equal(foundAcademy.PupilNumbers.GirlsOnRoll, htbDocumentResult.GirlsOnRoll);
                Assert.Equal(foundAcademy.PupilNumbers.BoysOnRoll, htbDocumentResult.BoysOnRoll);
                Assert.Equal(foundAcademy.PupilNumbers.WithStatementOfSen, htbDocumentResult.PupilsWithSen);
                Assert.Equal(foundAcademy.PupilNumbers.WhoseFirstLanguageIsNotEnglish, htbDocumentResult.PupilsWithFirstLanguageNotEnglish);
                Assert.Equal(foundAcademy.PupilNumbers.EligibleForFreeSchoolMeals, htbDocumentResult.PupilsFsm6Years);
                Assert.Equal(foundProject.PupilNumbersAdditionalInformation, htbDocumentResult.PupilNumbersAdditionalInformation);
                Assert.Equal(foundAcademy.LatestOfstedJudgement.OfstedReport, htbDocumentResult.OfstedReport);
                Assert.Equal(foundProject.LatestOfstedJudgementAdditionalInformation, htbDocumentResult.OfstedAdditionalInformation);
                Assert.Equal("2017-2018", htbDocumentResult.KeyStage2Performance.First().Year);
                Assert.Equal("2017-2018", htbDocumentResult.KeyStage4Performance.First().Year);
                Assert.Equal("2017-2018", htbDocumentResult.KeyStage5Performance.First().Year);
            }

            [Fact]
            public async void GivenGetInformationForProjectReturnsNotFound_ReturnsCorrectError()
            {
                _getInformationForProject.Setup(r => r.Execute(It.IsAny<string>())).ReturnsAsync(
                    new GetInformationForProjectResponse()
                    {
                        ResponseError = new ServiceResponseError
                        {
                            ErrorCode = ErrorCode.NotFound,
                            ErrorMessage = "Error message"
                        }
                    });

                var result = await _subject.Execute(_projectUrn);

                Assert.False(result.IsValid);
                Assert.Equal(ErrorCode.NotFound, result.ResponseError.ErrorCode);
                Assert.Equal("Not found", result.ResponseError.ErrorMessage);
            }

            [Fact]
            public async void GivenGetInformationForProjectReturnsServiceError_ReturnsCorrectError()
            {
                _getInformationForProject.Setup(r => r.Execute(It.IsAny<string>())).ReturnsAsync(
                    new GetInformationForProjectResponse()
                    {
                        ResponseError = new ServiceResponseError
                        {
                            ErrorCode = ErrorCode.ApiError,
                            ErrorMessage = "Error message"
                        }
                    });

                var result = await _subject.Execute(_projectUrn);

                Assert.False(result.IsValid);
                Assert.Equal(ErrorCode.ApiError, result.ResponseError.ErrorCode);
                Assert.Equal("API has encountered an error", result.ResponseError.ErrorMessage);
            }
        }
    }
}