using System.Collections.Generic;
using System.Net;
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
        private readonly Mock<IMapper<Project, TramsProjectUpdate>> _internalToUpdateMapper;
        private readonly Mock<IAcademies> _academies;
        private readonly Mock<ITrusts> _trusts;
        private readonly Trust _foundTrust;
        private readonly Academy _foundAcademy;

        public TramsProjectsRepositoryTests()
        {
            _httpClient = new Mock<ITramsHttpClient>();
            _externalToInternalMapper = new Mock<IMapper<TramsProject, Project>>();
            _summaryToInternalMapper = new Mock<IMapper<TramsProjectSummary, ProjectSearchResult>>();
            _internalToUpdateMapper = new Mock<IMapper<Project, TramsProjectUpdate>>();
            _academies = new Mock<IAcademies>();
            _trusts = new Mock<ITrusts>();
            _subject = new TramsProjectsRepository(
                _httpClient.Object, _externalToInternalMapper.Object, _summaryToInternalMapper.Object,
                _academies.Object, _trusts.Object, _internalToUpdateMapper.Object
            );
            _foundTrust = new Trust
            {
                Name = "Trust name",
                GiasGroupId = "Group ID"
            };

            _trusts.Setup(r => r.GetByUkprn(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResult<Trust> {Result = _foundTrust});

            _foundAcademy = new Academy
            {
                Name = "Trust name",
                Urn = "Urn"
            };

            _academies.Setup(r => r.GetAcademyByUkprn(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResult<Academy> {Result = _foundAcademy});
        }

        public class GetByUrnTests : TramsProjectsRepositoryTests
        {
            private readonly TramsProject _foundProject;

            public GetByUrnTests()
            {
                _foundProject = TramsProjects.GetSingleProject();
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

            [Theory]
            [InlineData(HttpStatusCode.NotFound)]
            [InlineData(HttpStatusCode.InternalServerError)]
            public async void GivenApiReturnsError_ThrowsApiError(HttpStatusCode httpStatusCode)
            {
                _httpClient.Setup(c => c.GetAsync("academyTransferProject/12345")).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = httpStatusCode
                });

                await Assert.ThrowsAsync<TramsApiException>(() => _subject.GetByUrn("12345"));
            }
        }

        public class UpdateProjectTests : TramsProjectsRepositoryTests
        {
            private readonly Project _projectToUpdate;
            private readonly TramsProjectUpdate _mappedProject;
            private readonly TramsProject _updatedProject;

            public UpdateProjectTests()
            {
                _projectToUpdate = new Project {Urn = "12345", Status = "New"};
                _mappedProject = new TramsProjectUpdate {ProjectUrn = "12345"};
                _updatedProject = new TramsProject {ProjectUrn = "12345 - Updated"};

                _internalToUpdateMapper.Setup(m => m.Map(_projectToUpdate)).Returns(_mappedProject);

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
            public async void GivenProject_MapsToProjectUpdate()
            {
                await _subject.Update(_projectToUpdate);

                _internalToUpdateMapper.Verify(m => m.Map(_projectToUpdate), Times.Once);
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
            
            [Theory]
            [InlineData(HttpStatusCode.NotFound)]
            [InlineData(HttpStatusCode.InternalServerError)]
            public async void GivenApiReturnsError_ThrowsApiError(HttpStatusCode httpStatusCode)
            {
                _httpClient.Setup(c => c.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ReturnsAsync(
                    new HttpResponseMessage
                    {
                        StatusCode = httpStatusCode
                    });
                
                var apiException = await Assert.ThrowsAsync<TramsApiException>(() => _subject.Update(_projectToUpdate));
                Assert.Equal(httpStatusCode, apiException.StatusCode);
            }
        }

        public class CreateProjectTests : TramsProjectsRepositoryTests
        {
            private readonly Project _projectToCreate;
            private readonly TramsProjectUpdate _mappedProject;
            private readonly TramsProject _createdProject;

            public CreateProjectTests()
            {
                _projectToCreate = new Project {Status = "New"};
                _mappedProject = new TramsProjectUpdate {Status = "Mapped new"};
                _createdProject = new TramsProject {ProjectUrn = "12345", Status = "Mapped new"};

                _internalToUpdateMapper.Setup(m => m.Map(_projectToCreate)).Returns(_mappedProject);
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

                _internalToUpdateMapper.Verify(m => m.Map(_projectToCreate), Times.Once);
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
            
            [Theory]
            [InlineData(HttpStatusCode.NotFound)]
            [InlineData(HttpStatusCode.InternalServerError)]
            public async void GivenApiReturnsError_ThrowsApiError(HttpStatusCode httpStatusCode)
            {
                _httpClient.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ReturnsAsync(
                    new HttpResponseMessage
                    {
                        StatusCode = httpStatusCode
                    });
                
                await Assert.ThrowsAsync<TramsApiException>(() => _subject.Create(_projectToCreate));
            }
        }

        public class GetProjectsTests : TramsProjectsRepositoryTests
        {
            private readonly List<TramsProjectSummary> _foundSummaries;

            public GetProjectsTests()
            {
                _foundSummaries = new List<TramsProjectSummary>
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
            }

            [Fact]
            public async void GivenSingleProjectSummaryReturned_MapsCorrectly()
            {
                _httpClient.Setup(c => c.GetAsync("academyTransferProject?page=1&title=")).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new PagedResult<TramsProjectSummary>(_foundSummaries, 12)))
                });

                await _subject.GetProjects();

                _summaryToInternalMapper.Verify(m =>
                    m.Map(It.Is<TramsProjectSummary>(summary => summary.ProjectUrn == "123")), Times.Once);
            }

            [Fact]
            public async void GivenMultipleProjectSummariesReturned_MapsCorrectly()
            {
                _foundSummaries.Add(
                    new TramsProjectSummary
                    {
                        ProjectUrn = "321",
                        TransferringAcademies = new List<TransferringAcademy>
                        {
                            new TransferringAcademy
                            {
                                IncomingTrustUkprn = "456",
                                OutgoingAcademyUkprn = "789"
                            }
                        }
                    }
                );

                _httpClient.Setup(c => c.GetAsync("academyTransferProject?page=1&title=")).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new PagedResult<TramsProjectSummary>(_foundSummaries)))
                });

                await _subject.GetProjects();

                _summaryToInternalMapper.Verify(m =>
                    m.Map(It.Is<TramsProjectSummary>(summary => summary.ProjectUrn == "123")), Times.Once);
                _summaryToInternalMapper.Verify(m =>
                    m.Map(It.Is<TramsProjectSummary>(summary => summary.ProjectUrn == "321")), Times.Once);
            }

            [Fact]
            public async void GivenMultipleProjectSummaries_ReturnsMappedSummariesCorrectly()
            {
                _httpClient.Setup(c => c.GetAsync("academyTransferProject?page=1&title=")).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new PagedResult<TramsProjectSummary>(_foundSummaries,12)))
                });

                _summaryToInternalMapper.Setup(m => m.Map(It.IsAny<TramsProjectSummary>()))
                    .Returns<TramsProjectSummary>(
                        input => new ProjectSearchResult {Urn = $"Mapped {input.ProjectUrn}"}
                    );

                var result = await _subject.GetProjects();

                Assert.Equal("Mapped 123", result.Result[0].Urn);
            }
            
            [Theory]
            [InlineData(HttpStatusCode.NotFound)]
            [InlineData(HttpStatusCode.InternalServerError)]
            public async void GivenApiReturnsError_ThrowsApiError(HttpStatusCode httpStatusCode)
            {
                _httpClient.Setup(c => c.GetAsync("academyTransferProject?page=1&title=")).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = httpStatusCode
                });
                
                await Assert.ThrowsAsync<TramsApiException>(() => _subject.GetProjects());
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            public async void GivenPage_GetsProjectForPage(int page)
            {
                _httpClient.Setup(c => c.GetAsync($"academyTransferProject?page={page}&title=")).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new PagedResult<TramsProjectSummary>(_foundSummaries, 12)))
                });

                await _subject.GetProjects(page);
                
                _httpClient.Verify(c => c.GetAsync($"academyTransferProject?page={page}&title="), Times.Once());
            }
        }

        private static async Task<bool> AssertStringContentMatches(string expectedContent, StringContent actualContent)
        {
            var actualContentString = await actualContent.ReadAsStringAsync();
            return expectedContent.Equals(actualContentString);
        }

        
    }
}