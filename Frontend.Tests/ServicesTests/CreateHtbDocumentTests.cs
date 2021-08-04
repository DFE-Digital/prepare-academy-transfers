using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Services;
using Frontend.Services.Responses;
using Moq;
using System.Collections.Generic;
using System.Net;
using Frontend.Services.Interfaces;
using Xunit;

namespace Frontend.Tests.ServicesTests
{
    public class CreateHtbDocumentTests
    {
        private readonly CreateHtbDocument _subject;
        private readonly Mock<IProjects> _projectsRepository;
        private readonly Mock<IAcademies> _academiesRepository;
        private readonly Mock<IGetInformationForProject> _getInformationForProject;
        private readonly string _projectUrn = "projectId";
        private readonly string _academyUkprn = "academyId";
        private readonly Project _foundProject;
        private readonly Academy _foundAcademy;

        public CreateHtbDocumentTests()
        {
            _projectsRepository = new Mock<IProjects>();
            _academiesRepository = new Mock<IAcademies>();
            _getInformationForProject = new Mock<IGetInformationForProject>();

            _subject = new CreateHtbDocument(_projectsRepository.Object, _academiesRepository.Object, _getInformationForProject.Object);

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

        public class ExecuteTests : CreateHtbDocumentTests
        {
            [Fact]
            public async void GivenProjectRepositoryReturnsNotFound_ReturnsCorrectError()
            {
                _projectsRepository.Setup(r => r.GetByUrn(It.IsAny<string>())).ReturnsAsync(
                    new RepositoryResult<Project>
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
                Assert.Equal("Not found", result.ResponseError.ErrorMessage);
            }

            [Fact]
            public async void GivenProjectRepositoryReturnsServiceError_ReturnsCorrectError()
            {
                _projectsRepository.Setup(r => r.GetByUrn(It.IsAny<string>())).ReturnsAsync(
                    new RepositoryResult<Project>
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
                Assert.Equal("Not found", result.ResponseError.ErrorMessage);
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
