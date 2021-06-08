using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Data.Models;
using Data.TRAMS.Models;
using Data.TRAMS.Models.AcademyTransferProject;
using Data.TRAMS.Tests.TestFixtures;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Data.TRAMS.Tests
{
    public class TramsProjectsRepositoryTests
    {
        private readonly TramsProjectsRepository _subject;
        private readonly Mock<ITramsHttpClient> _httpClient;
        private readonly Mock<IMapper<TramsProject, Project>> _externalToInternalMapper;
        private readonly Mock<IMapper<TramsProjectSummary, ProjectSearchResult>> _summaryToInternalMapper;
        private readonly Mock<IMapper<Project, TramsProject>> _internalToExternalMapper;
        private readonly Mock<IAcademies> _academies;
        private readonly Mock<ITrusts> _trusts;

        public TramsProjectsRepositoryTests()
        {
            _httpClient = new Mock<ITramsHttpClient>();
            _externalToInternalMapper = new Mock<IMapper<TramsProject, Project>>();
            _summaryToInternalMapper = new Mock<IMapper<TramsProjectSummary, ProjectSearchResult>>();
            _internalToExternalMapper = new Mock<IMapper<Project, TramsProject>>();
            _academies = new Mock<IAcademies>();
            _trusts = new Mock<ITrusts>();
            _subject = new TramsProjectsRepository(
                _httpClient.Object, _externalToInternalMapper.Object,
                _internalToExternalMapper.Object, _summaryToInternalMapper.Object,
                _academies.Object, _trusts.Object
            );
        }

        public class GetByUrnTests : TramsProjectsRepositoryTests
        {
            private readonly TramsProject _foundProject;

            public GetByUrnTests()
            {
                _foundProject = Projects.GetSingleProject();
                _httpClient.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(_foundProject))
                });
                _externalToInternalMapper.Setup(m => m.Map(It.IsAny<TramsProject>())).Returns<TramsProject>(
                    externalProject =>
                        new Project
                        {
                            Urn = $"Mapped {externalProject.ProjectUrn}"
                        });
            }

            [Fact]
            public async void GivenUrn_GetsProjectFromAPI()
            {
                await _subject.GetByUrn("12345");
                _httpClient.Verify(c => c.GetAsync("academyTransferProject/12345"), Times.Once);
            }

            [Fact]
            public async void GivenApiReturnsProject_MapProjectToInternalModel()
            {
                await _subject.GetByUrn("12345");

                _externalToInternalMapper.Verify(m =>
                    m.Map(It.Is<TramsProject>(project => _foundProject.ProjectUrn == project.ProjectUrn)), Times.Once);
            }

            [Fact]
            public async void GivenApiReturnsProject_ReturnsMappedProject()
            {
                var response = await _subject.GetByUrn("12345");

                Assert.Equal($"Mapped {_foundProject.ProjectUrn}", response.Result.Urn);
            }
        }

        public class UpdateProjectTests : TramsProjectsRepositoryTests
        {
            private readonly Project _projectToUpdate;
            private readonly TramsProject _mappedProject;
            private readonly TramsProject _updatedProject;

            public UpdateProjectTests()
            {
                _projectToUpdate = new Project {Urn = "12345", Status = "New"};
                _mappedProject = new TramsProject {ProjectUrn = "12345"};
                _updatedProject = new TramsProject {ProjectUrn = "12345 - Updated"};

                _internalToExternalMapper.Setup(m => m.Map(_projectToUpdate)).Returns(_mappedProject);

                _httpClient.Setup(c => c.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ReturnsAsync(
                    new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(_updatedProject))
                    });

                _externalToInternalMapper.Setup(m => m.Map(It.IsAny<TramsProject>())).Returns<TramsProject>(input =>
                    new Project
                    {
                        Urn = $"Mapped {input.ProjectUrn}"
                    });
            }

            [Fact]
            public async void GivenProject_MapsToExternalProject()
            {
                await _subject.Update(_projectToUpdate);

                _internalToExternalMapper.Verify(m => m.Map(_projectToUpdate), Times.Once);
            }

            [Fact]
            public async void GivenProjectGetsMapped_PostsMappedProjectToTheApi()
            {
                await _subject.Update(_projectToUpdate);
                var expectedPostedContent = JsonConvert.SerializeObject(_mappedProject);

                _httpClient.Verify(c => c.PatchAsync(
                    "academyTransferProject/12345",
                    It.Is<StringContent>(content => AssertStringContentMatches(expectedPostedContent, content).Result)
                ), Times.Once);
            }

            [Fact]
            public async void GivenProjectCreated_MapsCreatedProject()
            {
                await _subject.Update(_projectToUpdate);

                _externalToInternalMapper.Verify(m =>
                    m.Map(It.Is<TramsProject>(project => project.ProjectUrn == _updatedProject.ProjectUrn)));
            }

            [Fact]
            public async void GivenProjectCreated_ReturnsMappedCreatedProject()
            {
                var response = await _subject.Update(_projectToUpdate);

                Assert.Equal($"Mapped {_updatedProject.ProjectUrn}", response.Result.Urn);
            }
        }

        public class CreateProjectTests : TramsProjectsRepositoryTests
        {
            private readonly Project _projectToCreate;
            private readonly TramsProject _mappedProject;
            private readonly TramsProject _createdProject;

            public CreateProjectTests()
            {
                _projectToCreate = new Project {Status = "New"};
                _mappedProject = new TramsProject {Status = "Mapped new"};
                _createdProject = new TramsProject {ProjectUrn = "12345", Status = "Mapped new"};

                _internalToExternalMapper.Setup(m => m.Map(_projectToCreate)).Returns(_mappedProject);
                _httpClient.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ReturnsAsync(
                    new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(_createdProject))
                    });
                _externalToInternalMapper.Setup(m => m.Map(It.IsAny<TramsProject>())).Returns<TramsProject>(input =>
                    new Project
                    {
                        Urn = $"Mapped {input.ProjectUrn}"
                    });
            }

            [Fact]
            public async void GivenProject_MapsToExternalProject()
            {
                await _subject.Create(_projectToCreate);

                _internalToExternalMapper.Verify(m => m.Map(_projectToCreate), Times.Once);
            }

            [Fact]
            public async void GivenProjectGetsMapped_PostsMappedProjectToTheApi()
            {
                await _subject.Create(_projectToCreate);
                var expectedPostedContent = JsonConvert.SerializeObject(_mappedProject);

                _httpClient.Verify(c => c.PostAsync(
                    "academyTransferProject",
                    It.Is<StringContent>(content => AssertStringContentMatches(expectedPostedContent, content).Result)
                ), Times.Once);
            }

            [Fact]
            public async void GivenProjectCreated_MapsCreatedProject()
            {
                await _subject.Create(_projectToCreate);

                _externalToInternalMapper.Verify(m =>
                    m.Map(It.Is<TramsProject>(project => project.ProjectUrn == _createdProject.ProjectUrn)));
            }

            [Fact]
            public async void GivenProjectCreated_ReturnsMappedCreatedProject()
            {
                var response = await _subject.Create(_projectToCreate);

                Assert.Equal($"Mapped {_createdProject.ProjectUrn}", response.Result.Urn);
            }
        }

        public class GetProjectsTests : TramsProjectsRepositoryTests
        {
            [Fact]
            public async void GivenSingleProjectSummaryReturned_MapsCorrectly()
            {
                var foundSummaries = new List<TramsProjectSummary> {new TramsProjectSummary {ProjectUrn = "123"}};
                _httpClient.Setup(c => c.GetAsync("academyTransferProject")).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(foundSummaries))
                });

                await _subject.GetProjects();

                _summaryToInternalMapper.Verify(m =>
                    m.Map(It.Is<TramsProjectSummary>(summary => summary.ProjectUrn == "123")), Times.Once);
            }

            [Fact]
            public async void GivenMultipleProjectSummariesReturned_MapsCorrectly()
            {
                var foundSummaries = new List<TramsProjectSummary>
                    {new TramsProjectSummary {ProjectUrn = "123"}, new TramsProjectSummary {ProjectUrn = "456"}};

                _httpClient.Setup(c => c.GetAsync("academyTransferProject")).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(foundSummaries))
                });

                await _subject.GetProjects();

                _summaryToInternalMapper.Verify(m =>
                    m.Map(It.Is<TramsProjectSummary>(summary => summary.ProjectUrn == "123")), Times.Once);
                _summaryToInternalMapper.Verify(m =>
                    m.Map(It.Is<TramsProjectSummary>(summary => summary.ProjectUrn == "456")), Times.Once);
            }

            [Fact]
            public async void GivenMultipleProjectSummaries_ReturnsMappedSummariesCorrectly()
            {
                var foundSummaries = new List<TramsProjectSummary>
                    {new TramsProjectSummary {ProjectUrn = "123"}, new TramsProjectSummary {ProjectUrn = "456"}};

                _httpClient.Setup(c => c.GetAsync("academyTransferProject")).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(foundSummaries))
                });

                _summaryToInternalMapper.Setup(m => m.Map(It.IsAny<TramsProjectSummary>()))
                    .Returns<TramsProjectSummary>(
                        input => new ProjectSearchResult {Urn = $"Mapped {input.ProjectUrn}"}
                    );

                var result = await _subject.GetProjects();

                Assert.Equal("Mapped 123", result.Result[0].Urn);
                Assert.Equal("Mapped 456", result.Result[1].Urn);
            }

            #region ApiInterim

            [Fact]
            public async void GivenProjectSummary_FillInExtraInformationFromMultipleRequestsAndMap()
            {
                var foundSummaries = new List<TramsProjectSummary>
                {
                    new TramsProjectSummary
                    {
                        ProjectUrn = "123",
                        TransferringAcademies = new List<TransferringAcademy>
                        {
                            new TransferringAcademy
                            {
                                IncomingTrustUkprn = "456",
                                OutgoingAcademyUkprn = "789"
                            }
                        }
                    }
                };

                _httpClient.Setup(c => c.GetAsync("academyTransferProject")).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(foundSummaries))
                });

                var foundTrust = new Trust
                {
                    Name = "Trust name",
                    GiasGroupId = "Group ID"
                };

                _trusts.Setup(r => r.GetByUkprn("456")).ReturnsAsync(new RepositoryResult<Trust> {Result = foundTrust});

                var foundAcademy = new Academy
                {
                    Name = "Trust name",
                    Urn = "Urn"
                };

                _academies.Setup(r => r.GetAcademyByUkprn("789"))
                    .ReturnsAsync(new RepositoryResult<Academy> {Result = foundAcademy});

                var result = await _subject.GetProjects();

                _trusts.Verify(r => r.GetByUkprn("456"), Times.Once);
                _academies.Verify(r => r.GetAcademyByUkprn("789"), Times.Once);
                _summaryToInternalMapper.Verify(
                    m => m.Map(It.Is<TramsProjectSummary>(
                            toMap =>
                                toMap.OutgoingTrust.Ukprn == foundSummaries[0].OutgoingTrustUkprn &&
                                toMap.TransferringAcademies[0].IncomingTrust.GroupName == foundTrust.Name &&
                                toMap.TransferringAcademies[0].IncomingTrust.GroupId == foundTrust.GiasGroupId &&
                                toMap.TransferringAcademies[0].OutgoingAcademy.Name == foundAcademy.Name &&
                                toMap.TransferringAcademies[0].OutgoingAcademy.Urn == foundAcademy.Urn
                        )
                    )
                );
            }

            #endregion
        }

        private static async Task<bool> AssertStringContentMatches(string expectedContent, StringContent actualContent)
        {
            var actualContentString = await actualContent.ReadAsStringAsync();
            return expectedContent.Equals(actualContentString);
        }
    }
}