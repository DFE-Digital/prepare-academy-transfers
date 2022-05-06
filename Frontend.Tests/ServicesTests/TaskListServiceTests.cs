using System;
using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Data.Models.Projects;
using Data.TRAMS;
using Frontend.BackgroundServices;
using Frontend.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Index = Frontend.Pages.Projects.Index;

namespace Frontend.Tests.ServicesTests
{
    public class TaskListServiceTests : BaseTests
    {
        private readonly TaskListService _subject;
        private readonly Index _index;

        public TaskListServiceTests()
        {
            var educationPerformanceRepository = new Mock<IEducationPerformance>();
            educationPerformanceRepository.Setup(r => r.GetByAcademyUrn(AcademyUrn))
                .ReturnsAsync(new RepositoryResult<EducationPerformance>
                {
                    Result = new EducationPerformance
                    {
                        KeyStage2Performance = new List<KeyStage2>()
                    }
                });

            _subject = new TaskListService(ProjectRepository.Object);
            _index = new Index(_subject, new PerformanceDataChannel(new Mock<ILogger<PerformanceDataChannel>>().Object))
            {
                Urn = ProjectUrn0001
            };
        }

        [Fact]
        public void GivenGetByUrnReturnsError()
        {
            _index.Urn = ProjectErrorUrn;
            Assert.Throws<TramsApiException>(() => _subject.BuildTaskListStatuses(_index));
        }

        [Fact]
        public void GivenNoTransferringAcademies_ThrowsOutOfRangeException()
        {
            FoundProjectFromRepo.TransferringAcademies = new List<TransferringAcademies>();
            Assert.Throws<ArgumentOutOfRangeException>(() => _subject.BuildTaskListStatuses(_index));
        }

        [Fact]
        public void GivenAProjectID_PutsTheProjectReferenceInThePage()
        {
            _subject.BuildTaskListStatuses(_index);
            Assert.Equal(ProjectReference, _index.ProjectReference);
        }

        public class AcademyAndTrustInformationStatusTest : TaskListServiceTests
        {
            public static IEnumerable<object[]> AcademyAndTrustCompleted()
            {
                yield return new object[]
                {
                    new TransferAcademyAndTrustInformation
                    {
                        Author = "Author",
                        Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Approve
                    }
                };
                yield return new object[]
                {
                    new TransferAcademyAndTrustInformation
                    {
                        Author = "Author",
                        Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Decline
                    }
                };
            }

            [Theory]
            [MemberData(nameof(AcademyAndTrustCompleted))]
            public void GivenAcademyTrustCompleted_StatusCompleted(TransferAcademyAndTrustInformation info)
            {
                FoundProjectFromRepo.AcademyAndTrustInformation = info;
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.Completed, _index.AcademyAndTrustInformationStatus);
            }

            public static IEnumerable<object[]> AcademyAndTrustInProgress()
            {
                yield return new object[]
                {
                    new TransferAcademyAndTrustInformation
                    {
                        Author = "Author",
                        Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Empty
                    }
                };
                yield return new object[]
                {
                    new TransferAcademyAndTrustInformation
                    {
                        Author = "Author"
                    }
                };
                yield return new object[]
                {
                    new TransferAcademyAndTrustInformation
                    {
                        Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Decline
                    }
                };
                yield return new object[]
                {
                    new TransferAcademyAndTrustInformation
                    {
                        Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Approve
                    }
                };
            }

            [Theory]
            [MemberData(nameof(AcademyAndTrustInProgress))]
            public void GivenAcademyTrustInProgress_StatusInProgress(TransferAcademyAndTrustInformation info)
            {
                FoundProjectFromRepo.AcademyAndTrustInformation = info;
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.InProgress, _index.AcademyAndTrustInformationStatus);
            }

            [Fact]
            public void GivenNoAuthorAndEmptyRecommendation_StatusNotStarted()
            {
                FoundProjectFromRepo.AcademyAndTrustInformation = new TransferAcademyAndTrustInformation();
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.NotStarted, _index.AcademyAndTrustInformationStatus);
            }
        }

        public class GetFeatureTransferStatusTests : TaskListServiceTests
        {
            public static IEnumerable<object[]> FeatureTransferInProgress()
            {
                yield return new object[]
                {
                    new TransferFeatures
                    {
                        ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.Dfe
                    }
                };
                yield return new object[]
                {
                    new TransferFeatures
                    {
                        ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.OutgoingTrust
                    }
                };
                yield return new object[]
                {
                    new TransferFeatures
                    {
                        ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.OutgoingTrust
                    }
                };
                yield return new object[]
                {
                    new TransferFeatures
                    {
                        ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.OutgoingTrust,
                        TypeOfTransfer = TransferFeatures.TransferTypes.Other
                    }
                };
                yield return new object[]
                {
                    new TransferFeatures
                    {
                        ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.OutgoingTrust,
                        TypeOfTransfer = TransferFeatures.TransferTypes.MatClosure,
                        IsCompleted = false
                    }
                };
            }

            public static IEnumerable<object[]> FeatureTransferCompleted()
            {
                yield return new object[]
                {
                    new TransferFeatures
                    {
                        ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.OutgoingTrust,
                        TypeOfTransfer = TransferFeatures.TransferTypes.MatClosure,
                        IsCompleted = true
                    }
                };
                yield return new object[]
                {
                    new TransferFeatures
                    {
                        ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.OutgoingTrust,
                        TypeOfTransfer = TransferFeatures.TransferTypes.SatClosure,
                        IsCompleted = true
                    }
                };
                yield return new object[]
                {
                    new TransferFeatures
                    {
                        ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.Dfe,
                        TypeOfTransfer = TransferFeatures.TransferTypes.MatToMat,
                        IsCompleted = true
                    }
                };
                yield return new object[]
                {
                    new TransferFeatures
                    {
                        ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.Dfe,
                        TypeOfTransfer = TransferFeatures.TransferTypes.MatToMat,
                        IsCompleted = true
                    }
                };
            }

            [Theory]
            [MemberData(nameof(FeatureTransferInProgress))]
            public void GivenWhoInitiated_StatusInProgress(TransferFeatures transferFeatures)
            {
                FoundProjectFromRepo.Features = transferFeatures;
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.InProgress, _index.FeatureTransferStatus);
            }

            [Fact]
            public void GivenWhoInitiatedEmpty_StatusNotStarted()
            {
                FoundProjectFromRepo.Features = new TransferFeatures()
                {
                    ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.Empty
                };
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.NotStarted, _index.FeatureTransferStatus);
            }

            [Theory]
            [MemberData(nameof(FeatureTransferCompleted))]
            public void GivenTransferComplete_StatusCompleted(TransferFeatures transferFeatures)
            {
                FoundProjectFromRepo.Features = transferFeatures;
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.Completed, _index.FeatureTransferStatus);
            }
        }

        public class GetTransferDatesStatus : TaskListServiceTests
        {
            [Fact]
            public void GivenNoDates_StatusNotStarted()
            {
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.NotStarted, _index.TransferDatesStatus);
            }

            public static IEnumerable<object[]> TargetDatesInProgress()
            {
                yield return new object[]
                {
                    new TransferDates
                    {
                        Target = "12/03/2051"
                    }
                };
                yield return new object[]
                {
                    new TransferDates
                    {
                        FirstDiscussed = "23/09/2004"
                    }
                };
                yield return new object[]
                {
                    new TransferDates
                    {
                        Htb = "23/12/2034"
                    }
                };
                yield return new object[]
                {
                    new TransferDates
                    {
                        Htb = "23/12/2034",
                        Target = "23/01/2050"
                    }
                };
                yield return new object[]
                {
                    new TransferDates
                    {
                        Htb = "23/12/2034",
                        HasFirstDiscussedDate = true
                    }
                };
            }

            public static IEnumerable<object[]> TargetDatesCompleted()
            {
                yield return new object[]
                {
                    new TransferDates
                    {
                        Target = "12/03/2051",
                        Htb = "23/12/2034",
                        FirstDiscussed = "01/01/2001"
                    }
                };
                yield return new object[]
                {
                    new TransferDates
                    {
                        Htb = "23/12/2034",
                        HasFirstDiscussedDate = false,
                        Target = "12/01/2021"
                    }
                };
                yield return new object[]
                {
                    new TransferDates
                    {
                        HasHtbDate = false,
                        HasFirstDiscussedDate = false,
                        HasTargetDateForTransfer = false
                    }
                };
            }

            [Theory]
            [MemberData(nameof(TargetDatesInProgress))]
            public void GivenDates_StatusInProgress(TransferDates dates)
            {
                FoundProjectFromRepo.Dates = dates;
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.InProgress, _index.TransferDatesStatus);
            }

            [Theory]
            [MemberData(nameof(TargetDatesCompleted))]
            public void GivenCompletedDates_StatusComplete(TransferDates dates)
            {
                FoundProjectFromRepo.Dates = dates;
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.Completed, _index.TransferDatesStatus);
            }
        }

        public class GetBenefitsStatus : TaskListServiceTests
        {
            [Fact]
            public void GivenNoData_StatusNotStarted()
            {
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.NotStarted, _index.BenefitsAndOtherFactorsStatus);
            }

            public static IEnumerable<object[]> BenefitsInProgress()
            {
                yield return new object[]
                {
                    new TransferBenefits
                    {
                        IntendedBenefits = new List<TransferBenefits.IntendedBenefit>
                        {
                            TransferBenefits.IntendedBenefit.StrengtheningGovernance,
                            TransferBenefits.IntendedBenefit.ImprovingOfstedRating
                        }
                    }
                };

                yield return new object[]
                {
                    new TransferBenefits
                    {
                        OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
                            {{TransferBenefits.OtherFactor.HighProfile, "High Profile"}}
                    }
                };
                
                yield return new object[]
                {
                    new TransferBenefits
                    {
                        IntendedBenefits = new List<TransferBenefits.IntendedBenefit>
                        {
                            TransferBenefits.IntendedBenefit.StrengtheningGovernance
                        },
                        OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
                            {{TransferBenefits.OtherFactor.HighProfile, "High Profile"}},
                        IsCompleted = false
                    }
                };
                
            }

            public static IEnumerable<object[]> BenefitsComplete()
            {
                yield return new object[]
                {
                    new TransferBenefits
                    {
                        IntendedBenefits = new List<TransferBenefits.IntendedBenefit>
                        {
                            TransferBenefits.IntendedBenefit.StrengtheningGovernance
                        },
                        OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
                            {{TransferBenefits.OtherFactor.HighProfile, "High Profile"}},
                        IsCompleted = true
                    }
                };

                yield return new object[]
                {
                    new TransferBenefits
                    {
                        IntendedBenefits = new List<TransferBenefits.IntendedBenefit>
                        {
                            TransferBenefits.IntendedBenefit.StrengtheningGovernance,
                            TransferBenefits.IntendedBenefit.ImprovingOfstedRating,
                            TransferBenefits.IntendedBenefit.SecurityFinancialRecovery,
                            TransferBenefits.IntendedBenefit.CentralFinanceTeamAndSupport
                        },
                        OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
                            {{TransferBenefits.OtherFactor.HighProfile, "High Profile"}},
                        IsCompleted = true
                    }
                };
                
                yield return new object[]
                {
                    new TransferBenefits
                    {
                        OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
                            {{TransferBenefits.OtherFactor.HighProfile, "High Profile"}},
                        IsCompleted = true
                    }
                };
            }

            [Theory]
            [MemberData(nameof(BenefitsInProgress))]
            public void GivenBenefitsInProgress_StatusInProgress(TransferBenefits benefits)
            {
                FoundProjectFromRepo.Benefits = benefits;
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.InProgress, _index.BenefitsAndOtherFactorsStatus);
            }

            [Theory]
            [MemberData(nameof(BenefitsComplete))]
            public void GivenBenefitsComplete_StatusCompleted(TransferBenefits benefits)
            {
                FoundProjectFromRepo.Benefits = benefits;
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.Completed, _index.BenefitsAndOtherFactorsStatus);
            }
        }

        public class GetRationaleStatus : TaskListServiceTests
        {
            [Fact]
            public void GivenNoData_StatusNotStarted()
            {
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.NotStarted, _index.RationaleStatus);
            }

            public static IEnumerable<object[]> RationaleInProgress()
            {
                yield return new object[]
                {
                    new TransferRationale
                    {
                        Project = "Project rationale"
                    }
                };

                yield return new object[]
                {
                    new TransferRationale
                    {
                        Trust = "Trust rationale"
                    }
                };
                
                yield return new object[]
                {
                    new TransferRationale
                    {
                        Trust = "Trust rationale",
                        Project = "Project rationale",
                        IsCompleted = false
                    }
                };
            }

            [Theory]
            [MemberData(nameof(RationaleInProgress))]
            public void GivenRationaleInProgress_StatusInProgress(TransferRationale rationale)
            {
                FoundProjectFromRepo.Rationale = rationale;
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.InProgress, _index.RationaleStatus);
            }

            [Fact]
            public void GivenRationaleComplete_StatusCompleted()
            {
                FoundProjectFromRepo.Rationale = new TransferRationale
                {
                    Project = "Project Rationale",
                    Trust = "Trust Rationale",
                    IsCompleted = true
                };
                _subject.BuildTaskListStatuses(_index);
                Assert.Equal(ProjectStatuses.Completed, _index.RationaleStatus);
            }
        }
    }
}