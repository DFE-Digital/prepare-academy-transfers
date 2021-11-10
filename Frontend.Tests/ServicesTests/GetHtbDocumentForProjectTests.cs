using System.Globalization;
using System.Linq;
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
                var getTestInformationForProject =
                    TestFixtures.GetInformationForProject.GetTestInformationForProject(_projectUrn);
                
                _getInformationForProject.Setup(s => s.Execute(_projectUrn)).ReturnsAsync(
                    getTestInformationForProject);

                var result = await _subject.Execute(_projectUrn);
                var htbDocumentResult = result.HtbDocument;
                
                Assert.Equal("Approve", htbDocumentResult.Recommendation);
                Assert.Equal("author", htbDocumentResult.Author);
                Assert.Equal(getTestInformationForProject.Project.Name, htbDocumentResult.ProjectName);
                Assert.Equal("Incoming Trust Name", htbDocumentResult.SponsorName);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.EstablishmentType, htbDocumentResult.AcademyTypeAndRoute);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.Name, htbDocumentResult.SchoolName);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.Urn, htbDocumentResult.SchoolUrn);
                Assert.Equal(getTestInformationForProject.Project.OutgoingTrustName, htbDocumentResult.TrustName);
                Assert.Equal(getTestInformationForProject.Project.OutgoingTrustUkprn, htbDocumentResult.TrustReferenceNumber);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.GeneralInformation.SchoolType, htbDocumentResult.SchoolType);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.GeneralInformation.SchoolPhase, htbDocumentResult.SchoolPhase);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.GeneralInformation.AgeRange, htbDocumentResult.AgeRange);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.GeneralInformation.Capacity, htbDocumentResult.SchoolCapacity);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.GeneralInformation.Pan, htbDocumentResult.PublishedAdmissionNumber);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.GeneralInformation.PercentageFsm, htbDocumentResult.PercentageFreeSchoolMeals);
                Assert.Equal("100 (100%)", htbDocumentResult.NumberOnRoll);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years, htbDocumentResult.PupilsFsm6Years);
                Assert.Equal("1 January 2020", htbDocumentResult.OfstedLastInspection);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.LatestOfstedJudgement.OverallEffectiveness, htbDocumentResult.OverallEffectiveness);
                Assert.Equal(getTestInformationForProject.Project.Rationale.Project, htbDocumentResult.RationaleForProject);
                Assert.Equal(getTestInformationForProject.Project.Rationale.Trust, htbDocumentResult.RationaleForTrust);
                Assert.Equal("", htbDocumentResult.ClearedBy);
                Assert.Equal(System.DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentUICulture), htbDocumentResult.Version);
                Assert.Equal("1 January 2020", htbDocumentResult.DateOfHtb);
                Assert.Equal("1 January 2020", htbDocumentResult.DateOfProposedTransfer);
                Assert.Equal("1 January 2020", htbDocumentResult.DateTransferWasFirstDiscussed);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.GeneralInformation.ViabilityIssue, htbDocumentResult.ViabilityIssues);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.GeneralInformation.Deficit, htbDocumentResult.FinancialDeficit);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.GeneralInformation.Pfi, htbDocumentResult.Pfi);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.GeneralInformation.DiocesesPercent, htbDocumentResult.PercentageGoodOrOutstandingInDiocesanTrust);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.GeneralInformation.DistanceToSponsorHq, htbDocumentResult.DistanceFromTheAcademyToTheTrustHeadquarters);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.GeneralInformation.MpAndParty, htbDocumentResult.MpAndParty);
                Assert.Equal("Department for Education", htbDocumentResult.WhoInitiatedTheTransfer);
                Assert.Equal("Yes", htbDocumentResult.ReasonForTransfer);
                Assert.Equal("intervention", htbDocumentResult.MoreDetailsAboutTheTransfer);
                Assert.Equal("Closure of a SAT and the academy joining a MAT", htbDocumentResult.TypeOfTransfer);
                Assert.Equal("Strengthening governance\nStronger leadership\n", htbDocumentResult.TransferBenefits);
                Assert.Equal("This is a high profile transfer (ministers and media could be involved)\ntestHighProfile\nThere are finance and debt concerns\ndebtConcerns\n", htbDocumentResult.OtherFactors);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.PupilNumbers.GirlsOnRoll, htbDocumentResult.GirlsOnRoll);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.PupilNumbers.BoysOnRoll, htbDocumentResult.BoysOnRoll);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.PupilNumbers.WithStatementOfSen, htbDocumentResult.PupilsWithSen);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.PupilNumbers.WhoseFirstLanguageIsNotEnglish, htbDocumentResult.PupilsWithFirstLanguageNotEnglish);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years, htbDocumentResult.PupilsFsm6Years);
                Assert.Equal(getTestInformationForProject.Project.PupilNumbersAdditionalInformation, htbDocumentResult.PupilNumbersAdditionalInformation);
                Assert.Equal(getTestInformationForProject.OutgoingAcademy.LatestOfstedJudgement.OfstedReport, htbDocumentResult.OfstedReport);
                Assert.Equal(getTestInformationForProject.Project.LatestOfstedJudgementAdditionalInformation, htbDocumentResult.OfstedAdditionalInformation);
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