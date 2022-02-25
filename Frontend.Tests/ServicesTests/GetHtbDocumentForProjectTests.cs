using System.Globalization;
using System.Linq;
using Frontend.Services;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Moq;
using Xunit;

namespace Frontend.Tests.ServicesTests
{
    public class GetHtbDocumentForProjectTests : BaseTests
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

                //todo: Loop through academies
                var academy = getTestInformationForProject.OutgoingAcademies.First();
                
                Assert.Equal("Approve", htbDocumentResult.Recommendation);
                Assert.Equal("author", htbDocumentResult.Author);
                Assert.Equal(getTestInformationForProject.Project.IncomingTrustName, htbDocumentResult.ProjectName);
                Assert.Equal(academy.EstablishmentType, htbDocumentResult.AcademyTypeAndRoute);
                Assert.Equal(academy.Name, htbDocumentResult.SchoolName);
                Assert.Equal(academy.Urn, htbDocumentResult.SchoolUrn);
                Assert.Equal(getTestInformationForProject.Project.OutgoingTrustName, htbDocumentResult.TrustName);
                Assert.Equal(getTestInformationForProject.Project.OutgoingTrustUkprn, htbDocumentResult.TrustReferenceNumber);
                Assert.Equal(academy.GeneralInformation.SchoolType, htbDocumentResult.SchoolType);
                Assert.Equal(academy.GeneralInformation.SchoolPhase, htbDocumentResult.SchoolPhase);
                Assert.Equal(academy.GeneralInformation.AgeRange, htbDocumentResult.AgeRange);
                Assert.Equal(academy.GeneralInformation.Capacity, htbDocumentResult.SchoolCapacity);
                Assert.Equal(academy.GeneralInformation.Pan, htbDocumentResult.PublishedAdmissionNumber);
                Assert.Equal(academy.GeneralInformation.PercentageFsm, htbDocumentResult.PercentageFreeSchoolMeals);
                Assert.Equal("100 (100%)", htbDocumentResult.NumberOnRoll);
                Assert.Equal(academy.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years, htbDocumentResult.PupilsFsm6Years);
                Assert.Equal("1 January 2020", htbDocumentResult.OfstedLastInspection);
                Assert.Equal(academy.LatestOfstedJudgement.OverallEffectiveness, htbDocumentResult.OverallEffectiveness);
                Assert.Equal(getTestInformationForProject.Project.Rationale.Project, htbDocumentResult.RationaleForProject);
                Assert.Equal(getTestInformationForProject.Project.Rationale.Trust, htbDocumentResult.RationaleForTrust);
                Assert.Equal("", htbDocumentResult.ClearedBy);
                Assert.Equal(System.DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentUICulture), htbDocumentResult.Version);
                Assert.Equal("1 January 2020", htbDocumentResult.DateOfHtb);
                Assert.Equal("1 January 2020", htbDocumentResult.DateOfProposedTransfer);
                Assert.Equal("1 January 2020", htbDocumentResult.DateTransferWasFirstDiscussed);
                Assert.Equal(academy.GeneralInformation.ViabilityIssue, htbDocumentResult.ViabilityIssues);
                Assert.Equal(academy.GeneralInformation.Deficit, htbDocumentResult.FinancialDeficit);
                Assert.Equal(academy.GeneralInformation.Pfi, htbDocumentResult.Pfi);
                Assert.Equal(academy.GeneralInformation.DiocesesPercent, htbDocumentResult.PercentageGoodOrOutstandingInDiocesanTrust);
                Assert.Equal(academy.GeneralInformation.DistanceToSponsorHq, htbDocumentResult.DistanceFromTheAcademyToTheTrustHeadquarters);
                Assert.Equal(academy.GeneralInformation.MpAndParty, htbDocumentResult.MpAndParty);
                Assert.Equal("Department for Education", htbDocumentResult.WhoInitiatedTheTransfer);
                Assert.Equal("Yes", htbDocumentResult.ReasonForTransfer);
                Assert.Equal("intervention", htbDocumentResult.MoreDetailsAboutTheTransfer);
                Assert.Equal("Closure of a SAT and the academy joining a MAT", htbDocumentResult.TypeOfTransfer);
                Assert.Equal("Strengthening governance\nStronger leadership\n", htbDocumentResult.TransferBenefits);
                Assert.Equal("This is a high profile transfer (ministers and media could be involved)\ntestHighProfile\nThere are finance and debt concerns\ndebtConcerns\n", htbDocumentResult.OtherFactors);
                Assert.Equal(academy.PupilNumbers.GirlsOnRoll, htbDocumentResult.GirlsOnRoll);
                Assert.Equal(academy.PupilNumbers.BoysOnRoll, htbDocumentResult.BoysOnRoll);
                Assert.Equal(academy.PupilNumbers.WithStatementOfSen, htbDocumentResult.PupilsWithSen);
                Assert.Equal(academy.PupilNumbers.WhoseFirstLanguageIsNotEnglish, htbDocumentResult.PupilsWithFirstLanguageNotEnglish);
                Assert.Equal(academy.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years, htbDocumentResult.PupilsFsm6Years);
                Assert.Equal(academy.PupilNumbers.AdditionalInformation, htbDocumentResult.PupilNumbersAdditionalInformation);
                Assert.Equal(academy.LatestOfstedJudgement.OfstedReport, htbDocumentResult.OfstedReport);
                Assert.Equal(academy.LatestOfstedJudgement.AdditionalInformation, htbDocumentResult.OfstedAdditionalInformation);
                Assert.Equal("2017-2018", htbDocumentResult.KeyStage2Performance.First().Year);
                Assert.Equal("2017-2018", htbDocumentResult.KeyStage4Performance.First().Year);
                Assert.Equal("2017-2018", htbDocumentResult.KeyStage5Performance.First().Year);
            }
        }
    }
}