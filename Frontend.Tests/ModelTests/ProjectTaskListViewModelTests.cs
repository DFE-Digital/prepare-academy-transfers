using Data.Models;
using Data.Models.Projects;
using Frontend.Models;
using System.Collections.Generic;
using System.Linq;
using Data.Models.KeyStagePerformance;
using Xunit;
using static Data.Models.Projects.TransferFeatures;
using KeyStage2 = Data.Models.KeyStagePerformance.KeyStage2;
using KeyStage4 = Data.Models.KeyStagePerformance.KeyStage4;
using KeyStage5 = Data.Models.KeyStagePerformance.KeyStage5;

namespace Frontend.Tests.ModelTests
{
    public class ProjectTaskListViewModelTests
    {
        public class TransferFeatureTests
        {
            [Fact]
            public void GivenTransferFeatureDataIsEmpty_ReturnsNotStarted()
            {
                // Arrange
                var project = new Project
                {
                    Features = new TransferFeatures
                    {
                        WhoInitiatedTheTransfer = TransferFeatures.ProjectInitiators.Empty,
                        ReasonForTransfer = new ReasonForTransfer
                        {
                            IsSubjectToRddOrEsfaIntervention = null,
                        },
                        TypeOfTransfer = TransferFeatures.TransferTypes.Empty
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.FeatureTransferStatus;

                // Assert
                Assert.Equal(ProjectStatuses.NotStarted, result);
            }

            [Fact]
            public void GivenTransferFeatureDataIsFullyCompleted_ReturnCompleted()
            {
                // Arrange
                var project = new Project
                {
                    Features = new TransferFeatures
                    {
                        WhoInitiatedTheTransfer = TransferFeatures.ProjectInitiators.Dfe,
                        ReasonForTransfer = new ReasonForTransfer
                        {
                            IsSubjectToRddOrEsfaIntervention = false,
                        },
                        TypeOfTransfer = TransferFeatures.TransferTypes.MatClosure
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.FeatureTransferStatus;

                // Assert
                Assert.Equal(ProjectStatuses.Completed, result);
            }

            [Theory]
            [InlineData(ProjectInitiators.OutgoingTrust, null, TransferTypes.Empty)]
            [InlineData(ProjectInitiators.OutgoingTrust, false, TransferTypes.Empty)]
            [InlineData(ProjectInitiators.OutgoingTrust, null, TransferTypes.JoiningToFormMat)]
            [InlineData(ProjectInitiators.Empty, null, TransferTypes.JoiningToFormMat)]
            [InlineData(ProjectInitiators.Empty, true, TransferTypes.Empty)]
            [InlineData(ProjectInitiators.Empty, true, TransferTypes.JoiningToFormMat)]
            public void GivenTransferFeatureDataIsPartiallyCompleted_ReturnInProgress(
                ProjectInitiators projectInitiator,
                bool? isSubjectToRddOrEsfaIntervention,
                TransferTypes transferType)
            {
                // Arrange
                var project = new Project
                {
                    Features = new TransferFeatures
                    {
                        WhoInitiatedTheTransfer = projectInitiator,
                        ReasonForTransfer = new ReasonForTransfer
                            {IsSubjectToRddOrEsfaIntervention = isSubjectToRddOrEsfaIntervention},
                        TypeOfTransfer = transferType
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.FeatureTransferStatus;

                // Assert
                Assert.Equal(ProjectStatuses.InProgress, result);
            }
        }

        public class HasKeyStage2PerformanceDataTests
        {
            [Fact]
            public void GivenNull_ReturnFalse()
            {
                var model = new ProjectTaskListViewModel {EducationPerformance = new EducationPerformance()};
                Assert.False(model.HasKeyStage2PerformanceInformation);
            }

            [Fact]
            public void GivenEmptyList_ReturnFalse()
            {
                var model = new ProjectTaskListViewModel
                    {EducationPerformance = new EducationPerformance() {KeyStage2Performance = new List<KeyStage2>()}};
                Assert.False(model.HasKeyStage2PerformanceInformation);
            }

            [Theory]
            [InlineData("12.5", null, null, null, null, null, null, null, null, null)]
            [InlineData(null, "12.5", null, null, null, null, null, null, null, null)]
            [InlineData(null, null, "12.5", null, null, null, null, null, null, null)]
            [InlineData(null, null, null, "12.5", null, null, null, null, null, null)]
            [InlineData(null, null, null, null, "12.5", null, null, null, null, null)]
            [InlineData(null, null, null, null, null, "12.5", null, null, null, null)]
            [InlineData(null, null, null, null, null, null, "12.5", null, null, null)]
            [InlineData(null, null, null, null, null, null, null, "12.5", null, null)]
            [InlineData(null, null, null, null, null, null, null, null, "12.5", null)]
            [InlineData(null, null, null, null, null, null, null, null, null, "12.5")]
            public void GivenAnyEducationPerformanceData_ReturnTrue(
                string mathsDisadvantaged, string mathsNonDisadvantaged,
                string readDisadvantaged, string readNonDisadvantaged,
                string writeDisadvantaged, string writeNonDisadvantaged,
                string rwmExpectedDisadvantaged, string rwmExpectedNonDisadvantaged,
                string rwmHigherDisadvantaged, string rwmHigherNonDisadvantaged)
            {
                var model = new ProjectTaskListViewModel
                {
                    EducationPerformance = new EducationPerformance
                    {
                        KeyStage2Performance = new List<KeyStage2>
                        {
                            new KeyStage2
                            {
                                MathsProgressScore = new DisadvantagedPupilsResult
                                    {Disadvantaged = mathsDisadvantaged, NotDisadvantaged = mathsNonDisadvantaged},
                                ReadingProgressScore = new DisadvantagedPupilsResult
                                    {Disadvantaged = readDisadvantaged, NotDisadvantaged = readNonDisadvantaged},
                                WritingProgressScore = new DisadvantagedPupilsResult
                                    {Disadvantaged = writeDisadvantaged, NotDisadvantaged = writeNonDisadvantaged},
                                PercentageMeetingExpectedStdInRWM = new DisadvantagedPupilsResult
                                {
                                    Disadvantaged = rwmExpectedDisadvantaged,
                                    NotDisadvantaged = rwmExpectedNonDisadvantaged
                                },
                                PercentageAchievingHigherStdInRWM = new DisadvantagedPupilsResult
                                {
                                    Disadvantaged = rwmHigherDisadvantaged, NotDisadvantaged = rwmHigherNonDisadvantaged
                                }
                            }
                        }
                    }
                };
                Assert.True(model.HasKeyStage2PerformanceInformation);
            }
        }

        public class HasKeyStage4PerformanceDataTests
        {
            [Fact]
            public void GivenNull_ReturnFalse()
            {
                var model = new ProjectTaskListViewModel {EducationPerformance = new EducationPerformance()};
                Assert.False(model.HasKeyStage4PerformanceInformation);
            }

            [Fact]
            public void GivenEmptyList_ReturnFalse()
            {
                var model = new ProjectTaskListViewModel
                    {EducationPerformance = new EducationPerformance() {KeyStage4Performance = new List<KeyStage4>()}};
                Assert.False(model.HasKeyStage4PerformanceInformation);
            }

            [Fact]
            public void GivenAnyEducationPerformanceData_ReturnTrue()
            {
                var model = new ProjectTaskListViewModel
                {
                    EducationPerformance = new EducationPerformance
                    {
                        KeyStage4Performance = new List<KeyStage4>
                        {
                            new KeyStage4()
                        }
                    }
                };

                var keyStage4PropertiesToTest = new[]
                {
                    "SipAttainment8score", "SipAttainment8scoreebacc",
                    "SipAttainment8scoreenglish", "SipAttainment8scoremaths", "SipAttainment8score",
                    "SipProgress8ebacc",
                    "SipProgress8english", "SipProgress8maths", "SipProgress8Score", "SipNumberofpupilsprogress8"
                };
                var keyStage4Performance = model.EducationPerformance.KeyStage4Performance[0];
                var keyStage4Properties = keyStage4Performance.GetType().GetProperties()
                    .Where(n => keyStage4PropertiesToTest.Contains(n.Name)).ToList();

                foreach (var property in keyStage4Properties)
                {
                    SetAllToNull();

                    var disadvantagedPupilResult = new DisadvantagedPupilsResult
                    {
                        Disadvantaged = "10.45",
                        NotDisadvantaged = null
                    };
                    property.SetValue(keyStage4Performance, disadvantagedPupilResult);
                    Assert.True(model.HasKeyStage4PerformanceInformation);

                    var notDisadvantagedPupilResult = new DisadvantagedPupilsResult
                    {
                        Disadvantaged = null,
                        NotDisadvantaged = "10.45"
                    };
                    property.SetValue(keyStage4Performance, notDisadvantagedPupilResult);
                    Assert.True(model.HasKeyStage4PerformanceInformation);
                }

                void SetAllToNull() => keyStage4Properties
                    .ForEach(p => p.SetValue(keyStage4Performance, new DisadvantagedPupilsResult()));
            }
        }

        public class HasKeyStage5PerformanceDataTests
        {
            [Fact]
            public void GivenNull_ReturnFalse()
            {
                var model = new ProjectTaskListViewModel {EducationPerformance = new EducationPerformance()};
                Assert.False(model.HasKeyStage5PerformanceInformation);
            }

            [Fact]
            public void GivenEmptyList_ReturnFalse()
            {
                var model = new ProjectTaskListViewModel
                    {EducationPerformance = new EducationPerformance() {KeyStage5Performance = new List<KeyStage5>()}};
                Assert.False(model.HasKeyStage5PerformanceInformation);
            }

            [Fact]
            public void GivenListWithItemWithNoAcademy_ReturnFalse()
            {
                var model = new ProjectTaskListViewModel
                {
                    EducationPerformance = new EducationPerformance
                        {KeyStage5Performance = new List<KeyStage5> {new KeyStage5()}}
                };
                Assert.False(model.HasKeyStage5PerformanceInformation);
            }

            [Theory]
            [InlineData("12.12", null)]
            [InlineData("12.12", "")]
            [InlineData(null, "12.12")]
            [InlineData("", "12.12")]
            [InlineData("12.12", "12.12")]
            public void GivenListWithItemWithAcademyWithData_ReturnTrue(string academicAverage,
                string appliedGeneralAverage)
            {
                var model = new ProjectTaskListViewModel
                {
                    EducationPerformance = new EducationPerformance
                    {
                        KeyStage5Performance = new List<KeyStage5>
                        {
                            new KeyStage5
                            {
                                Academy = new KeyStage5Result
                                {
                                    AcademicAverage = academicAverage,
                                    AppliedGeneralAverage = appliedGeneralAverage
                                }
                            }
                        }
                    }
                };
                Assert.True(model.HasKeyStage5PerformanceInformation);
            }
        }

        public class TransferDatesTests
        {
            [Fact]
            public void GivenTransferDatesAreEmpty_ReturnNotStarted()
            {
                // Arrange
                var project = new Project
                {
                    Dates = new TransferDates
                    {
                        FirstDiscussed = string.Empty,
                        Target = string.Empty,
                        Htb = string.Empty
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.TransferDatesStatus;

                // Assert
                Assert.Equal(ProjectStatuses.NotStarted, result);
            }

            [Fact]
            public void GivenTransferDatesAreNull_ReturnNotStarted()
            {
                // Arrange
                var project = new Project
                {
                    Dates = new TransferDates
                    {
                        FirstDiscussed = null,
                        Target = null,
                        Htb = null
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.TransferDatesStatus;

                // Assert
                Assert.Equal(ProjectStatuses.NotStarted, result);
            }

            [Theory]
            [InlineData("test", "", "")]
            [InlineData("test", "test", "")]
            [InlineData("test", "", "test")]
            [InlineData("", "", "test")]
            [InlineData("", "test", "")]
            [InlineData("", "test", "test")]
            public void GivenTransferDatesArePartiallyCompleted_ReturnInProgress(string firstDiscussed,
                string targetDate, string htbDate)
            {
                // Arrange
                var project = new Project
                {
                    Dates = new TransferDates
                    {
                        FirstDiscussed = firstDiscussed,
                        Target = targetDate,
                        Htb = htbDate
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.TransferDatesStatus;

                // Assert
                Assert.Equal(ProjectStatuses.InProgress, result);
            }

            [Fact]
            public void GivenTransferDatesAreCompleted_ReturnCompleted()
            {
                // Arrange
                var project = new Project
                {
                    Dates = new TransferDates
                    {
                        FirstDiscussed = "test",
                        Target = "test",
                        Htb = "test"
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.TransferDatesStatus;

                // Assert
                Assert.Equal(ProjectStatuses.Completed, result);
            }
        }

        public class BenefitsAndOtherFactorsTests
        {
            [Fact]
            public void GivenBenefitsAreEmpty_ReturnNotStarted()
            {
                // Arrange
                var project = new Project
                {
                    Benefits = new TransferBenefits
                    {
                        IntendedBenefits = new List<TransferBenefits.IntendedBenefit>(),
                        OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>()
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.BenefitsAndOtherFactorsStatus;

                // Assert
                Assert.Equal(ProjectStatuses.NotStarted, result);
            }

            [Fact]
            public void GivenIntendedBenefitIsCompletedAndOtherFactorsIsEmpty_ReturnInProgress()
            {
                // Arrange
                var project = new Project
                {
                    Benefits = new TransferBenefits
                    {
                        IntendedBenefits = new List<TransferBenefits.IntendedBenefit>
                            {TransferBenefits.IntendedBenefit.CentralFinanceTeamAndSupport},
                        OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>()
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.BenefitsAndOtherFactorsStatus;

                // Assert
                Assert.Equal(ProjectStatuses.InProgress, result);
            }

            [Fact]
            public void GivenIntendedBenefitIsEmptyAndOtherFactorsIsCompleted_ReturnInProgress()
            {
                // Arrange
                var project = new Project
                {
                    Benefits = new TransferBenefits
                    {
                        IntendedBenefits = new List<TransferBenefits.IntendedBenefit>(),
                        OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
                            {{TransferBenefits.OtherFactor.HighProfile, "test"}}
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.BenefitsAndOtherFactorsStatus;

                // Assert
                Assert.Equal(ProjectStatuses.InProgress, result);
            }

            [Fact]
            public void GivenBenefitsAreNull_ReturnNotStarted()
            {
                // Arrange
                var project = new Project
                {
                    Benefits = new TransferBenefits
                    {
                        IntendedBenefits = null,
                        OtherFactors = null
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.BenefitsAndOtherFactorsStatus;

                // Assert
                Assert.Equal(ProjectStatuses.NotStarted, result);
            }

            [Fact]
            public void GivenBenefitsAreCompleted_ReturnCompleted()
            {
                // Arrange
                var project = new Project
                {
                    Benefits = new TransferBenefits
                    {
                        IntendedBenefits = new List<TransferBenefits.IntendedBenefit>
                            {TransferBenefits.IntendedBenefit.CentralFinanceTeamAndSupport},
                        OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
                            {{TransferBenefits.OtherFactor.HighProfile, "test"}}
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.BenefitsAndOtherFactorsStatus;

                // Assert
                Assert.Equal(ProjectStatuses.Completed, result);
            }
        }

        public class RationaleTests
        {
            [Fact]
            public void GivenRationaleIsEmpty_ReturnNotStarted()
            {
                // Arrange
                var project = new Project
                {
                    Rationale = new TransferRationale
                    {
                        Project = string.Empty,
                        Trust = string.Empty
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.RationaleStatus;

                // Assert
                Assert.Equal(ProjectStatuses.NotStarted, result);
            }

            [Theory]
            [InlineData("some value", "")]
            [InlineData("some value", null)]
            [InlineData("", "some value")]
            [InlineData(null, "some value")]
            public void GivenRationaleIsPartiallyCompleted_ReturnInProgress(string projectValue, string trustValue)
            {
                // Arrange
                var project = new Project
                {
                    Rationale = new TransferRationale
                    {
                        Project = projectValue,
                        Trust = trustValue
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.RationaleStatus;

                // Assert
                Assert.Equal(ProjectStatuses.InProgress, result);
            }

            [Fact]
            public void GivenRationaleIsCompleted_ReturnCompleted()
            {
                // Arrange
                var project = new Project
                {
                    Rationale = new TransferRationale
                    {
                        Project = "some value",
                        Trust = "some value"
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};

                // Act
                var result = model.RationaleStatus;

                // Assert
                Assert.Equal(ProjectStatuses.Completed, result);
            }
        }

        public class AcademyAndTrustInformationTests
        {
            [Theory]
            [InlineData(TransferAcademyAndTrustInformation.RecommendationResult.Empty, null,
                ProjectStatuses.NotStarted)]
            [InlineData(TransferAcademyAndTrustInformation.RecommendationResult.Approve, "test author",
                ProjectStatuses.Completed)]
            [InlineData(TransferAcademyAndTrustInformation.RecommendationResult.Empty, "test author",
                ProjectStatuses.InProgress)]
            [InlineData(TransferAcademyAndTrustInformation.RecommendationResult.Decline, null,
                ProjectStatuses.InProgress)]
            [InlineData(TransferAcademyAndTrustInformation.RecommendationResult.Decline, "",
                ProjectStatuses.InProgress)]
            public void GivenRelevantData_ReturnCorrectStatus(
                TransferAcademyAndTrustInformation.RecommendationResult recommendation, string author,
                ProjectStatuses expectedStatus)
            {
                var project = new Project
                {
                    AcademyAndTrustInformation = new TransferAcademyAndTrustInformation
                    {
                        Recommendation = recommendation,
                        Author = author
                    }
                };
                var model = new ProjectTaskListViewModel {Project = project};
                var result = model.AcademyAndTrustInformationStatus();

                Assert.Equal(expectedStatus, result);
            }
        }
    }
}