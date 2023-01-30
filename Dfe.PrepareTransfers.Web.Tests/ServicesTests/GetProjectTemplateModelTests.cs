using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.Web.Services;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ServicesTests
{
    public class GetProjectTemplateModelTests
    {
        private readonly GetProjectTemplateModel _subject;
        private readonly Mock<IGetInformationForProject> _getInformationForProject;
        private readonly string _projectUrn = "projectId";

        public GetProjectTemplateModelTests()
        {
            _getInformationForProject = new Mock<IGetInformationForProject>();
            _subject = new GetProjectTemplateModel(_getInformationForProject.Object);
        }

        public class ExecuteTemplateModelTests : GetProjectTemplateModelTests
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
                Assert.Equal(academy.GeneralInformation.PercentageFsm,
                    projectTemplateAcademyModel.PercentageFreeSchoolMeals);
                Assert.Equal("100 (100%)", projectTemplateAcademyModel.NumberOnRoll);
                Assert.Equal(academy.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years,
                    projectTemplateAcademyModel.PupilsFsm6Years);
                Assert.Equal("1 January 2020", projectTemplateAcademyModel.InspectionEndDate);
                Assert.Equal(academy.LatestOfstedJudgement.OverallEffectiveness,
                    projectTemplateAcademyModel.OverallEffectiveness);
                Assert.Equal(getTestInformationForProject.Project.Rationale.Project,
                    projectTemplateModel.RationaleForProject);
                Assert.Equal(getTestInformationForProject.Project.Rationale.Trust,
                    projectTemplateModel.RationaleForTrust);
                Assert.Equal("", projectTemplateModel.ClearedBy);
                Assert.Equal(System.DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentUICulture),
                    projectTemplateModel.Version);
                Assert.Equal("1 January 2020", projectTemplateModel.DateOfHtb);
                Assert.Equal("1 January 2020", projectTemplateModel.DateOfProposedTransfer);
                Assert.Equal(academy.GeneralInformation.ViabilityIssue, projectTemplateAcademyModel.ViabilityIssues);
                Assert.Equal(academy.GeneralInformation.Deficit, projectTemplateAcademyModel.FinancialDeficit);
                Assert.Equal(academy.GeneralInformation.Pfi, projectTemplateAcademyModel.Pfi);
                Assert.Equal(academy.GeneralInformation.DiocesesPercent,
                    projectTemplateAcademyModel.PercentageGoodOrOutstandingInDiocesanTrust);
                Assert.Equal(academy.GeneralInformation.DistanceToSponsorHq,
                    projectTemplateAcademyModel.DistanceFromTheAcademyToTheTrustHeadquarters);
                Assert.Equal(academy.GeneralInformation.MpAndParty, projectTemplateAcademyModel.MpAndParty);
                Assert.Equal("Closure of a SAT and the academy joining a MAT", projectTemplateModel.TypeOfTransfer);
                Assert.Equal("Strengthening governance\nStronger leadership\n", projectTemplateModel.TransferBenefits);
                Assert.Equal("Yes", projectTemplateModel.AnyRisks);
                Assert.Equal(getTestInformationForProject.Project.Benefits.OtherFactors.OrderBy(o => o.Key).Select(o => o.Value),
                    projectTemplateModel.OtherFactors.Select(o => o.Item2));
                Assert.Equal(academy.PupilNumbers.GirlsOnRoll, projectTemplateAcademyModel.GirlsOnRoll);
                Assert.Equal(academy.PupilNumbers.BoysOnRoll, projectTemplateAcademyModel.BoysOnRoll);
                Assert.Equal(academy.PupilNumbers.WithStatementOfSen, projectTemplateAcademyModel.PupilsWithSen);
                Assert.Equal(academy.PupilNumbers.WhoseFirstLanguageIsNotEnglish,
                    projectTemplateAcademyModel.PupilsWithFirstLanguageNotEnglish);
                Assert.Equal(academy.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years,
                    projectTemplateAcademyModel.PupilsFsm6Years);
                Assert.Equal(academy.PupilNumbers.AdditionalInformation,
                    projectTemplateAcademyModel.PupilNumbersAdditionalInformation);
                Assert.Equal(academy.LatestOfstedJudgement.OfstedReport, projectTemplateAcademyModel.OfstedReport);
                Assert.Equal(academy.LatestOfstedJudgement.AdditionalInformation,
                    projectTemplateAcademyModel.OfstedAdditionalInformation);
                Assert.Equal("2017-2018", projectTemplateAcademyModel.KeyStage2Performance.First().Year);
                Assert.Equal("2017-2018", projectTemplateAcademyModel.KeyStage4Performance.First().Year);
                Assert.Equal("2017-2018", projectTemplateAcademyModel.KeyStage5Performance.First().Year);
                Assert.Equal("Outstanding", projectTemplateAcademyModel.QualityOfEducation);
                Assert.Equal("Outstanding", projectTemplateAcademyModel.BehaviourAndAttitudes);
                Assert.Equal("Outstanding", projectTemplateAcademyModel.PersonalDevelopment);
                Assert.Equal("Outstanding", projectTemplateAcademyModel.EffectivenessOfLeadershipAndManagement);
                Assert.Equal("Outstanding", projectTemplateAcademyModel.EarlyYearsProvision);
                Assert.True(projectTemplateAcademyModel.EarlyYearsProvisionApplicable);
                Assert.Equal("N/A", projectTemplateAcademyModel.SixthFormProvision);
                Assert.False(projectTemplateAcademyModel.SixthFormProvisionApplicable);
                Assert.Equal("1 January 2021", projectTemplateAcademyModel.DateOfLatestSection8Inspection);
                Assert.True(projectTemplateAcademyModel.LatestInspectionIsSection8);
            }

        [Fact]
            public async void GivenExistingProjectWithMultipleAcademies_AssignsTheProjectToTheViewModel()
            {
                const string academyUrn = "urn 2";

                var getTestInformationForProject =
                    TestFixtures.GetInformationForProject.GetTestInformationForProject(_projectUrn);

                getTestInformationForProject.OutgoingAcademies.Add(
                    new Academy()
                    {
                        Ukprn = "ukprn2",
                        Urn = academyUrn,
                        EducationPerformance = new EducationPerformance
                        {
                            KeyStage2Performance = new List<KeyStage2> {new KeyStage2 {Year = "2017-2018"}},
                            KeyStage4Performance = new List<KeyStage4> {new KeyStage4 {Year = "2017-2018"}},
                            KeyStage5Performance = new List<KeyStage5> {new KeyStage5 {Year = "2017-2018"}}
                        }
                    });

                _getInformationForProject.Setup(s => s.Execute(_projectUrn)).ReturnsAsync(
                    getTestInformationForProject);

                var result = await _subject.Execute(_projectUrn);
                var projectTemplateModel = result.ProjectTemplateModel;
                var projectTemplateAcademyModels = projectTemplateModel.Academies;

                Assert.Equal(2, projectTemplateAcademyModels.Count);
                Assert.Equal(academyUrn, projectTemplateAcademyModels[1].SchoolUrn);
            }
        }
    }
}