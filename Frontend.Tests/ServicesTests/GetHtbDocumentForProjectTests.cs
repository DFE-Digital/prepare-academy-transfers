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
        private readonly GetProjectTemplateData _subject;
        private readonly Mock<IGetInformationForProject> _getInformationForProject;
        private readonly string _projectUrn = "projectId";

        public GetHtbDocumentForProjectTests()
        {
            _getInformationForProject = new Mock<IGetInformationForProject>();
            _subject = new GetProjectTemplateData(_getInformationForProject.Object);
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
                var projectTemplateModel = result.ProjectTemplateModel;
                var projectTemplateAcademyModel = projectTemplateModel.Academies.First();

                //todo: Loop through academies
                var academy = getTestInformationForProject.OutgoingAcademies.First();
                
                Assert.Equal("Approve", projectTemplateModel.Recommendation);
                Assert.Equal("author", projectTemplateModel.Author);
                Assert.Equal(getTestInformationForProject.Project.IncomingTrustName, projectTemplateModel.ProjectName);
                Assert.Equal(academy.EstablishmentType, projectTemplateAcademyModel.AcademyTypeAndRoute);
                Assert.Equal(academy.Name, projectTemplateAcademyModel.SchoolName);
                Assert.Equal(academy.Urn, projectTemplateAcademyModel.SchoolUrn);
                Assert.Equal(getTestInformationForProject.Project.IncomingTrustName, projectTemplateModel.ProjectName);
                Assert.Equal(getTestInformationForProject.Project.Reference, projectTemplateModel.ProjectReference);
                Assert.Equal(academy.GeneralInformation.SchoolType, projectTemplateAcademyModel.SchoolType);
                Assert.Equal(academy.GeneralInformation.SchoolPhase, projectTemplateAcademyModel.SchoolPhase);
                Assert.Equal(academy.GeneralInformation.AgeRange, projectTemplateAcademyModel.AgeRange);
                Assert.Equal(academy.GeneralInformation.Capacity, projectTemplateAcademyModel.SchoolCapacity);
                Assert.Equal(academy.GeneralInformation.Pan, projectTemplateAcademyModel.PublishedAdmissionNumber);
                Assert.Equal(academy.GeneralInformation.PercentageFsm, projectTemplateAcademyModel.PercentageFreeSchoolMeals);
                Assert.Equal("100 (100%)", projectTemplateAcademyModel.NumberOnRoll);
                Assert.Equal(academy.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years, projectTemplateAcademyModel.PupilsFsm6Years);
                Assert.Equal("1 January 2020", projectTemplateAcademyModel.OfstedLastInspection);
                Assert.Equal(academy.LatestOfstedJudgement.OverallEffectiveness, projectTemplateAcademyModel.OverallEffectiveness);
                Assert.Equal(getTestInformationForProject.Project.Rationale.Project, projectTemplateModel.RationaleForProject);
                Assert.Equal(getTestInformationForProject.Project.Rationale.Trust, projectTemplateModel.RationaleForTrust);
                Assert.Equal("", projectTemplateModel.ClearedBy);
                Assert.Equal(System.DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentUICulture), projectTemplateModel.Version);
                Assert.Equal("1 January 2020", projectTemplateModel.DateOfHtb);
                Assert.Equal("1 January 2020", projectTemplateModel.DateOfProposedTransfer);
                Assert.Equal("1 January 2020", projectTemplateModel.DateTransferWasFirstDiscussed);
                Assert.Equal(academy.GeneralInformation.ViabilityIssue, projectTemplateAcademyModel.ViabilityIssues);
                Assert.Equal(academy.GeneralInformation.Deficit, projectTemplateAcademyModel.FinancialDeficit);
                Assert.Equal(academy.GeneralInformation.Pfi, projectTemplateAcademyModel.Pfi);
                Assert.Equal(academy.GeneralInformation.DiocesesPercent, projectTemplateAcademyModel.PercentageGoodOrOutstandingInDiocesanTrust);
                Assert.Equal(academy.GeneralInformation.DistanceToSponsorHq, projectTemplateAcademyModel.DistanceFromTheAcademyToTheTrustHeadquarters);
                Assert.Equal(academy.GeneralInformation.MpAndParty, projectTemplateAcademyModel.MpAndParty);
                Assert.Equal("Department for Education", projectTemplateModel.WhoInitiatedTheTransfer);
                Assert.Equal("Yes", projectTemplateModel.ReasonForTransfer);
                Assert.Equal("intervention", projectTemplateModel.MoreDetailsAboutTheTransfer);
                Assert.Equal("Closure of a SAT and the academy joining a MAT", projectTemplateModel.TypeOfTransfer);
                Assert.Equal("Strengthening governance\nStronger leadership\n", projectTemplateModel.TransferBenefits);
                Assert.Equal("This is a high profile transfer (ministers and media could be involved)\ntestHighProfile\nThere are finance and debt concerns\ndebtConcerns\n", projectTemplateModel.OtherFactors);
                Assert.Equal(academy.PupilNumbers.GirlsOnRoll, projectTemplateAcademyModel.GirlsOnRoll);
                Assert.Equal(academy.PupilNumbers.BoysOnRoll, projectTemplateAcademyModel.BoysOnRoll);
                Assert.Equal(academy.PupilNumbers.WithStatementOfSen, projectTemplateAcademyModel.PupilsWithSen);
                Assert.Equal(academy.PupilNumbers.WhoseFirstLanguageIsNotEnglish, projectTemplateAcademyModel.PupilsWithFirstLanguageNotEnglish);
                Assert.Equal(academy.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years, projectTemplateAcademyModel.PupilsFsm6Years);
                Assert.Equal(academy.PupilNumbers.AdditionalInformation, projectTemplateAcademyModel.PupilNumbersAdditionalInformation);
                Assert.Equal(academy.LatestOfstedJudgement.OfstedReport, projectTemplateAcademyModel.OfstedReport);
                Assert.Equal(academy.LatestOfstedJudgement.AdditionalInformation, projectTemplateAcademyModel.OfstedAdditionalInformation);
                Assert.Equal("2017-2018", projectTemplateAcademyModel.KeyStage2Performance.First().Year);
                Assert.Equal("2017-2018", projectTemplateAcademyModel.KeyStage4Performance.First().Year);
                Assert.Equal("2017-2018", projectTemplateAcademyModel.KeyStage5Performance.First().Year);
            }
        }
    }
}