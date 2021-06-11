using System.Collections.Generic;
using System.Net;
using Data;
using Data.Models;
using Data.Models.Projects;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Frontend.Services;
using Frontend.Services.Responses;
using Moq;
using Xunit;

namespace Frontend.Tests.ServicesTests
{
    public class GetInformationForProjectTests
    {
        private readonly GetInformationForProject _subject;
        private readonly Mock<IProjects> _projectsRepository;
        private readonly Mock<IAcademies> _academiesRepository;
        private readonly string _projectUrn;
        private readonly string _academyUkprn;
        private Project _foundProject;
        private Academy _foundAcademy;

        public GetInformationForProjectTests()
        {
            _projectUrn = "projectId";
            _academyUkprn = "1234567";
            _projectsRepository = new Mock<IProjects>();
            _academiesRepository = new Mock<IAcademies>();

            _subject = new GetInformationForProject(_academiesRepository.Object, _projectsRepository.Object);

            SetupRepositories();
        }

        private void SetupRepositories()
        {
            _foundProject = new Project
            {
                Urn = _projectUrn,
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies {OutgoingAcademyUkprn = _academyUkprn}
                }
            };

            _foundAcademy = new Academy
            {
                Ukprn = _academyUkprn
            };

            _projectsRepository.Setup(r => r.GetByUrn(_projectUrn)).ReturnsAsync(
                new RepositoryResult<Project>
                {
                    Result = _foundProject
                });

            _academiesRepository.Setup(r => r.GetAcademyByUkprn(_academyUkprn)).ReturnsAsync(
                new RepositoryResult<Academy>
                {
                    Result = _foundAcademy
                });
        }

        [Fact]
        public async void GivenProjectId_LooksUpProjectInRepository()
        {
            await _subject.Execute(_projectUrn);

            _projectsRepository.Verify(r => r.GetByUrn(_projectUrn), Times.Once);
        }

        [Fact]
        public async void GivenProjectId_LooksUpProjectAcademyInDynamics()
        {
            await _subject.Execute(_projectUrn);

            _academiesRepository.Verify(r => r.GetAcademyByUkprn(_academyUkprn), Times.Once);
        }

        [Fact]
        public async void GivenProjectId_ReturnsFoundProjectAndAcademy()
        {
            var result = await _subject.Execute(_projectUrn);

            Assert.Equal(result.Project, _foundProject);
            Assert.Equal(result.OutgoingAcademy, _foundAcademy);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async void GivenAcademyRepositoryReturnsNotFound_ReturnsCorrectError()
        {
            _academiesRepository.Setup(r => r.GetAcademyByUkprn(It.IsAny<string>())).ReturnsAsync(
                new RepositoryResult<Academy>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        ErrorMessage = "Example error message"
                    }
                });

            var result = await _subject.Execute(_projectUrn);

            Assert.False(result.IsValid);
            Assert.Equal(ErrorCode.NotFound, result.ResponseError.ErrorCode);
            Assert.Equal("Outgoing academy not found", result.ResponseError.ErrorMessage);
        }
        
        [Fact]
        public async void GivenAcademyRepositoryReturnsServiceError_ReturnsCorrectError()
        {
            _academiesRepository.Setup(r => r.GetAcademyByUkprn(It.IsAny<string>())).ReturnsAsync(
                new RepositoryResult<Academy>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        ErrorMessage = "Example error message"
                    }
                });

            var result = await _subject.Execute(_projectUrn);

            Assert.False(result.IsValid);
            Assert.Equal(ErrorCode.ApiError, result.ResponseError.ErrorCode);
            Assert.Equal("API has encountered an error", result.ResponseError.ErrorMessage);
        }
    }
}