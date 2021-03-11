using System;
using System.Collections.Generic;
using System.Net;
using API.Controllers;
using API.Models.Downstream.D365;
using API.Models.Upstream.Enums;
using API.Models.Upstream.Request;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace API.Tests.ControllersTests
{
    public class ProjectsControllerTests
    {
        private readonly Mock<IProjectsRepository> _projectsRepository;
        private readonly Mock<IAcademiesRepository> _academiesRepository;
        private readonly Mock<ITrustsRepository> _trustsRepository;
        private readonly Mock<IRepositoryErrorResultHandler> _repositoryErrorHandler;
        private readonly ProjectsController _subject;

        public ProjectsControllerTests()
        {
            _projectsRepository = new Mock<IProjectsRepository>();
            _academiesRepository = new Mock<IAcademiesRepository>();
            _trustsRepository = new Mock<ITrustsRepository>();
            _repositoryErrorHandler = new Mock<IRepositoryErrorResultHandler>();
            var config = new Mock<IConfiguration>();

            _subject = new ProjectsController(_projectsRepository.Object, _academiesRepository.Object,
                _trustsRepository.Object,
                _repositoryErrorHandler.Object, config.Object);
        }

        #region Search Projects Tests

        [Fact]
        public void SearchProjects_DefaultPaging_Test()
        {
            _subject.SearchProjects(string.Empty, ProjectStatusEnum.Completed, null, null, null);

            //Assert that defaults are applied
            _projectsRepository.Verify(
                p => p.SearchProject(string.Empty, ProjectStatusEnum.Completed, true, 10, 1),
                Times.Once);
        }

        [Fact]
        public void SearchProjects_InvalidPageSize_ReturnsBadRequest()
        {
            //Execute
            var result = _subject.SearchProjects(string.Empty, ProjectStatusEnum.Completed, true, 0, 1);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be a 400 Bad Request
            Assert.Equal("Page size cannot be zero", (string) castedResult.Value);
            Assert.Equal(400, castedResult.StatusCode);
        }

        [Fact]
        public void SearchProjects_InvalidPageNumber_ReturnsBadRequest()
        {
            //Execute
            var result = _subject.SearchProjects(string.Empty, ProjectStatusEnum.Completed, true, 1, 0);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be a 400 Bad Request
            Assert.Equal("Page number cannot be 0", (string) castedResult.Value);
            Assert.Equal(400, castedResult.StatusCode);
        }

        [Fact]
        public void SearchProjects_InvalidProjectRepoResult()
        {
            //Arrange 

            //Set up project repository to return a bad request
            _projectsRepository.Setup(r =>
                    r.SearchProject("searchQuery", ProjectStatusEnum.Completed, true, 10, 1))
                .ReturnsAsync(new RepositoryResult<SearchProjectsPageModel>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "Bad request error message"
                    }
                });

            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });


            //Execute
            var result = _subject.SearchProjects("searchQuery", ProjectStatusEnum.Completed, true, 10, 1);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be a what the Error Handler returns
            Assert.Equal("Some error message", (string) castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void SearchProjects_OkRepoResult()
        {
            //Arrange 

            //Set up project repository to return a good request
            _projectsRepository.Setup(r =>
                    r.SearchProject("searchQuery", ProjectStatusEnum.Completed, true, 2, 1))
                .ReturnsAsync(new RepositoryResult<SearchProjectsPageModel>
                {
                    Result = new SearchProjectsPageModel
                    {
                        Projects = new List<SearchProjectsModel>
                        {
                            new SearchProjectsModel {ProjectName = "AT-10000"},
                            new SearchProjectsModel {ProjectName = "AT-10001"}
                        },
                        CurrentPage = 1,
                        TotalPages = 3
                    }
                });

            //Execute
            var result = _subject.SearchProjects("searchQuery", ProjectStatusEnum.Completed, true, 2, 1);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be a 200OK wrapped around what the mapper returns
            Assert.Equal(1, ((SearchProjectsPageModel) castedResult.Value).CurrentPage);
            Assert.Equal(3, ((SearchProjectsPageModel) castedResult.Value).TotalPages);
            Assert.Equal(2, ((SearchProjectsPageModel) castedResult.Value).Projects.Count);
            Assert.Equal("AT-10000", ((SearchProjectsPageModel) castedResult.Value).Projects[0].ProjectName);
            Assert.Equal(200, castedResult.StatusCode);
        }

        #endregion

        #region GetProjectById Tests

        [Fact]
        public void GetProjectById_ProjectRepoFailure()
        {
            //Arrange
            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");

            //Set up project repository to return a bad request when getting a project by id
            _projectsRepository.Setup(p => p.GetProjectById(projectId))
                .ReturnsAsync(new RepositoryResult<GetProjectsResponseModel>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "Bad request error message"
                    }
                });

            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });

            //Execute
            var result = _subject.GetProjectById(projectId);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string) castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void GetProjectById_ProjectNotFound()
        {
            //Arrange
            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");

            //Set up project repository to return null when getting a project by id
            _projectsRepository.Setup(p => p.GetProjectById(projectId))
                .ReturnsAsync(new RepositoryResult<GetProjectsResponseModel>
                {
                    Result = null
                });


            //Execute
            var result = _subject.GetProjectById(projectId);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Repository Error Handler result should not be called
            _repositoryErrorHandler.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()),
                Times.Never);
            //Final result should be 404 Not Found with the proper message
            Assert.Equal("Project with id '00000003-0000-0ff1-ce00-000000000000' not found",
                (string) castedResult.Value);
            Assert.Equal(404, castedResult.StatusCode);
        }

        [Fact]
        public void GetProjectById_ProjectFound()
        {
            //Arrange
            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");

            _projectsRepository.Setup(p => p.GetProjectById(projectId))
                .ReturnsAsync(new RepositoryResult<GetProjectsResponseModel>
                {
                    Result = new GetProjectsResponseModel
                    {
                        ProjectName = "AT-10000"
                    }
                });

            //Execute
            var result = _subject.GetProjectById(projectId);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Repository Error Handler result should not be called
            _repositoryErrorHandler.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()),
                Times.Never);
            //Final result should be 200 OK wrapped around the mapped result
            Assert.Equal("AT-10000", ((GetProjectsResponseModel) castedResult.Value).ProjectName);
            Assert.Equal(200, castedResult.StatusCode);
        }

        #endregion

        #region GETProjectAcademyById Tests

        [Fact]
        public void GetProjectAcademyById_ProjectRepoFailure()
        {
            //Arrange 

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");

            //Set up project repository to return bad request when getting a project by id
            _projectsRepository.Setup(p => p.GetProjectById(projectId))
                .ReturnsAsync(new RepositoryResult<GetProjectsResponseModel>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "Bad request error message"
                    }
                });

            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });

            //Execute
            var result = _subject.GetProjectAcademy(projectId, projectAcademyId);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string) castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }


        [Fact]
        public void GetProjectAcademyById_ProjectNotFound()
        {
            //Arrange 

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");

            //Set up project repository to return an a null result
            _projectsRepository.Setup(p => p.GetProjectById(projectId))
                .ReturnsAsync(new RepositoryResult<GetProjectsResponseModel>
                {
                    Result = null
                });


            //Execute
            var result = _subject.GetProjectAcademy(projectId, projectAcademyId);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Repository Error Handler result should not be called
            _repositoryErrorHandler.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()),
                Times.Never);
            //Final result should be a what the 404 Not Found with the correct message
            Assert.Equal("Project with id '00000003-0000-0ff1-ce00-000000000000' not found",
                (string) castedResult.Value);
            Assert.Equal(404, castedResult.StatusCode);
        }

        [Fact]
        public void GetProjectAcademyById_ProjectAcademy_RepoFailure()
        {
            //Arrange 

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");

            //Set up project repository to return a valid set result
            _projectsRepository.Setup(p => p.GetProjectById(projectId))
                .ReturnsAsync(new RepositoryResult<GetProjectsResponseModel>
                {
                    Result = new GetProjectsResponseModel {ProjectName = "AT-1000"}
                });

            //Set up project repo to return a failed result when getting the Project Academy by id
            _projectsRepository.Setup(p => p.GetProjectAcademyById(projectAcademyId))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "Bad request error message"
                    }
                });

            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });

            //Execute
            var result = _subject.GetProjectAcademy(projectId, projectAcademyId);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be a what the Error Handler returns
            Assert.Equal("Some error message", (string) castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void GetProjectAcademyById_ProjectAcademy_NotFound()
        {
            //Arrange 

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");

            //Set up project repository to return a valid set found project
            _projectsRepository.Setup(p => p.GetProjectById(projectId))
                .ReturnsAsync(new RepositoryResult<GetProjectsResponseModel>
                {
                    Result = new GetProjectsResponseModel {ProjectName = "AT-1000"}
                });

            //Set up project repo to return a null result when getting the Project Academy by id
            _projectsRepository.Setup(p => p.GetProjectAcademyById(projectAcademyId))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Result = null
                });

            //Execute
            var result = _subject.GetProjectAcademy(projectId, projectAcademyId);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Repository Error Handler result should not be called
            _repositoryErrorHandler.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()),
                Times.Never);
            //Final result should be a what the 404 Not Found with the correct message
            Assert.Equal("Project Academy with id '00000003-0000-0ff1-ce00-000000000001' not found",
                (string) castedResult.Value);
            Assert.Equal(404, castedResult.StatusCode);
        }

        [Fact]
        public void GetProjectAcademyById_ProjectAcademy_Found()
        {
            //Arrange 

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");

            //Set up project repository to return a valid set result when getting the project by id
            _projectsRepository.Setup(p => p.GetProjectById(projectId))
                .ReturnsAsync(new RepositoryResult<GetProjectsResponseModel>
                {
                    Result = new GetProjectsResponseModel {ProjectName = "AT-1000"}
                });

            //Set up project repo to return a valid set result when getting the Project Academy by id
            _projectsRepository.Setup(p => p.GetProjectAcademyById(projectAcademyId))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Result = new GetProjectsAcademyResponseModel {AcademyName = "Project Academy 001"}
                });

            //Execute
            var result = _subject.GetProjectAcademy(projectId, projectAcademyId);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            _repositoryErrorHandler.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()),
                Times.Never);
            //Final result should be 200 OK wrapped around the mapped result
            Assert.Equal("Project Academy 001",
                ((GetProjectsAcademyResponseModel) castedResult.Value).AcademyName);
            Assert.Equal(200, castedResult.StatusCode);
        }

        #endregion

        #region UpdateProjectAcademy Tests

        [Fact]
        public void UpdateProjectAcademy_ProjectRepoFailure_WhenGettingProjectAcademyById()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");

            //Set up project repository to return a bad request when looking for a ProjectAcademy by id
            _projectsRepository.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "Bad request error message"
                    }
                });


            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });

            //Execute
            var result = _subject.UpdateProjectAcademy(projectId, projectAcademyId, null);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string) castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void UpdateProjectAcademy_NotFound_WhenGettingProjectAcademyById()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");

            //Set up project repository to return a null result when getting a ProjectAcademy by id
            _projectsRepository.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Result = null
                });

            //Execute
            var result = _subject.UpdateProjectAcademy(projectId, projectAcademyId, null);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Repository Error Handler result should not be called
            _repositoryErrorHandler.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()),
                Times.Never);
            //Final result should be a 404 Not Found with the correct message
            Assert.Equal("Project Academy with id '00000003-0000-0ff1-ce00-000000000001' not found",
                (string) castedResult.Value);
            Assert.Equal(404, castedResult.StatusCode);
        }

        [Fact]
        public void UpdateProjectAcademy_ProjectIdMismatch_WhenGettingProjectAcademyById()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");
            var mismatchingProjectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000002");

            //Set up project repository to return a ProjectAcademy with a mismatching ProjectId
            _projectsRepository.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Result = new GetProjectsAcademyResponseModel {ProjectId = mismatchingProjectId}
                });

            //Execute
            var result = _subject.UpdateProjectAcademy(projectId, projectAcademyId, null);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Repository Error Handler result should not be called
            _repositoryErrorHandler.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()),
                Times.Never);
            //Final result should be an Unprocessable Entity with the correct message
            Assert.Equal(
                "Project Academy with id '00000003-0000-0ff1-ce00-000000000001' not found within project with id '00000003-0000-0ff1-ce00-000000000000'",
                (string) castedResult.Value);
            Assert.Equal(422, castedResult.StatusCode);
        }

        [Fact]
        public void UpdateProjectAcademy_ReferencedAcademyId_RepoFailure()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");
            var referencedAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000003");

            var request = new PutProjectAcademiesRequestModel {AcademyId = referencedAcademyId};

            //Set up project repository to return a ProjectAcademy with a matching ProjectId
            _projectsRepository.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Result = new GetProjectsAcademyResponseModel {ProjectId = projectId}
                });

            //Set up the academies repository to return bad request when verifying the referenced academy
            _academiesRepository.Setup(m => m.GetAcademyById(referencedAcademyId))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "Bad request error message"
                    }
                });


            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });

            //Execute
            var result = _subject.UpdateProjectAcademy(projectId, projectAcademyId, request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string) castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void UpdateProjectAcademy_ReferencedAcademyId_NotFound()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");
            var referencedAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000003");

            var request = new PutProjectAcademiesRequestModel {AcademyId = referencedAcademyId};

            //Set up project repository to return a ProjectAcademy with a matching ProjectId
            _projectsRepository.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Result = new GetProjectsAcademyResponseModel {ProjectId = projectId}
                });

            //Set up the academies repository to return a null result when verifying the referenced academy
            _academiesRepository.Setup(m => m.GetAcademyById(referencedAcademyId))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = null
                });

            //Execute
            var result = _subject.UpdateProjectAcademy(projectId, projectAcademyId, request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be an Unprocessable Entity
            Assert.Equal($"No academy found with the id of: {referencedAcademyId}", (string) castedResult.Value);
            Assert.Equal(422, castedResult.StatusCode);
        }

        [Fact]
        public void UpdateProjectAcademy_WhenUpdatingEntity_RepositoryFailure()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");
            var referencedAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000003");

            var request = new PutProjectAcademiesRequestModel {AcademyId = referencedAcademyId};

            //Set up project repository to return a ProjectAcademy with a matching ProjectId
            _projectsRepository.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Result = new GetProjectsAcademyResponseModel {ProjectId = projectId}
                });

            //Set up the projects repository to fail when updating the Project Academy entity
            _projectsRepository.Setup(r =>
                    r.UpdateProjectAcademy(It.IsAny<Guid>(), It.IsAny<PutProjectAcademiesRequestModel>()))
                .ReturnsAsync(new RepositoryResult<Guid?>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "Bad request error message"
                    }
                });

            //Set up the academies repository to return a null result when verifying the referenced academy
            _academiesRepository.Setup(m => m.GetAcademyById(referencedAcademyId))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = referencedAcademyId}
                });

            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });

            //Execute
            var result = _subject.UpdateProjectAcademy(projectId, projectAcademyId, request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string) castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }


        [Fact]
        public void UpdateProjectAcademy_UpdateSucceeds_WhenRefreshingProjectAcademy_RepositoryFailure()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");
            var referencedAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000003");
            //Using this alternative Id to mock up the responses so that the second call to get ProjectAcademy by id fails
            var alternativeProjectAcademyIdForFailingRepo = Guid.Parse("00000003-0000-0ff1-ce00-000000000004");

            var request = new PutProjectAcademiesRequestModel {AcademyId = referencedAcademyId};

            //Set up project repository to return a ProjectAcademy with a matching ProjectId
            _projectsRepository.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Result = new GetProjectsAcademyResponseModel {ProjectId = projectId}
                });

            //Set up project repository to fail when refreshing the ProjectAcademy entity
            _projectsRepository.Setup(r => r.GetProjectAcademyById(alternativeProjectAcademyIdForFailingRepo))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "Bad request error message"
                    }
                });

            //Set up the projects repository to return a valid set result when updating the entity
            _projectsRepository.Setup(r =>
                    r.UpdateProjectAcademy(It.IsAny<Guid>(), It.IsAny<PutProjectAcademiesRequestModel>()))
                .ReturnsAsync(new RepositoryResult<Guid?>
                {
                    Result = alternativeProjectAcademyIdForFailingRepo
                });

            //Set up the academies repository to return a null result when verifying the referenced academy
            _academiesRepository.Setup(m => m.GetAcademyById(referencedAcademyId))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = referencedAcademyId}
                });

            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });

            //Execute
            var result = _subject.UpdateProjectAcademy(projectId, projectAcademyId, request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be a what the Error Handler returns
            Assert.Equal("Some error message", (string) castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void UpdateProjectAcademy_Success()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");
            var updatedProjectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000002");
            var referencedAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000003");

            var request = new PutProjectAcademiesRequestModel {AcademyId = referencedAcademyId};

            //Set up project repository to return a ProjectAcademy with a matching ProjectId
            _projectsRepository.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Result = new GetProjectsAcademyResponseModel {ProjectId = projectId}
                });


            //Set up the projects repository to return a valid set result when updating the entity
            _projectsRepository.Setup(r =>
                    r.UpdateProjectAcademy(It.IsAny<Guid>(), It.IsAny<PutProjectAcademiesRequestModel>()))
                .ReturnsAsync(new RepositoryResult<Guid?>
                {
                    Result = updatedProjectAcademyId
                });

            //Set up the academies repository to return a valid set result when verifying the referenced academy
            _academiesRepository.Setup(m => m.GetAcademyById(referencedAcademyId))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = referencedAcademyId}
                });

            _projectsRepository.Setup(r => r.GetProjectAcademyById(updatedProjectAcademyId))
                .ReturnsAsync(new RepositoryResult<GetProjectsAcademyResponseModel>
                {
                    Result = new GetProjectsAcademyResponseModel
                        {ProjectId = projectId, ProjectAcademyId = projectAcademyId}
                });

            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });

            //Execute
            var result = _subject.UpdateProjectAcademy(projectId, projectAcademyId, request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be a 200 OK wrapped around the output of the final mapper
            Assert.Equal(projectAcademyId, ((GetProjectsAcademyResponseModel) castedResult.Value).ProjectAcademyId);
            Assert.Equal(200, castedResult.StatusCode);
        }

        #endregion

        #region InsertTrusts Tests

        [Fact]
        public void InsertTrusts_CheckingReferencedAcademies_RepoFailure()
        {
            //Arrange 
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")},
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("10000003-0000-0ff1-ce00-000000000001")},
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")
                            }
                        }
                    }
                },
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel {TrustId = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")}
                }
            };

            //Set up academy id checks to return a bad request
            _academiesRepository.Setup(r => r.GetAcademyById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "Bad request error message"
                    }
                });

            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });

            //Execute
            var result = _subject.InsertProject(request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string) castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void InsertTrusts_CheckingReferencedTrusts_RepoFailure()
        {
            //Arrange 
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")},
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("10000003-0000-0ff1-ce00-000000000001")},
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")
                            }
                        }
                    }
                },
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel {TrustId = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")}
                }
            };

            //Set up academy checks to all return a valid and set result
            _academiesRepository.Setup(r => r.GetAcademyById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel()
                });

            //Set up the trust id checks to return a bad request
            _trustsRepository.Setup(r => r.GetTrustById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetTrustsD365Model>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "Bad request error message"
                    }
                });


            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });

            //Execute
            var result = _subject.InsertProject(request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string) castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void InsertTrusts_CheckingReferencedAcademies_OneAcademyNotFound()
        {
            //Arrange 
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")},
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("10000003-0000-0ff1-ce00-000000000001")},
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")
                            }
                        }
                    }
                },
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel {TrustId = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")}
                }
            };

            //Set up academy id checks to all pass but one
            _academiesRepository.Setup(r => r.GetAcademyById(Guid.Parse("00000003-0000-0ff1-ce00-000000000000")))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")}
                });

            _academiesRepository.Setup(r => r.GetAcademyById(Guid.Parse("10000003-0000-0ff1-ce00-000000000001")))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")}
                });
            //This is the one academy that won't be found
            _academiesRepository.Setup(r => r.GetAcademyById(Guid.Parse("20000003-0000-0ff1-ce00-000000000002")))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = null
                });

            //Set up the trust id checks to all pass
            _trustsRepository.Setup(r => r.GetTrustById(Guid.Parse("30000003-0000-0ff1-ce00-000000000003")))
                .ReturnsAsync(new RepositoryResult<GetTrustsD365Model>
                    {Result = new GetTrustsD365Model {Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")}});
            _trustsRepository.Setup(r => r.GetTrustById(Guid.Parse("40000003-0000-0ff1-ce00-000000000004")))
                .ReturnsAsync(new RepositoryResult<GetTrustsD365Model>
                    {Result = new GetTrustsD365Model {Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")}});

            //Execute
            var result = _subject.InsertProject(request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be an Unprocessable Entity
            Assert.Equal("No academy found with the id of: 20000003-0000-0ff1-ce00-000000000002",
                (string) castedResult.Value);
            Assert.Equal(422, castedResult.StatusCode);
        }

        [Fact]
        public void InsertTrusts_CheckingReferencedAcademies_NoAcademyFound()
        {
            //Arrange 
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")},
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("10000003-0000-0ff1-ce00-000000000001")},
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")
                            }
                        }
                    }
                },
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel {TrustId = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")}
                }
            };

            //All academies will fail
            _academiesRepository.Setup(r => r.GetAcademyById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = null
                });

            //Set up the trust id checks to all pass
            _trustsRepository.Setup(r => r.GetTrustById(Guid.Parse("30000003-0000-0ff1-ce00-000000000003")))
                .ReturnsAsync(new RepositoryResult<GetTrustsD365Model>
                    {Result = new GetTrustsD365Model {Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")}});

            _trustsRepository.Setup(r => r.GetTrustById(Guid.Parse("40000003-0000-0ff1-ce00-000000000004")))
                .ReturnsAsync(new RepositoryResult<GetTrustsD365Model>
                    {Result = new GetTrustsD365Model {Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")}});

            //Execute
            var result = _subject.InsertProject(request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be an Unprocessable Entity
            Assert.Equal(
                "No academy found with the id of: 00000003-0000-0ff1-ce00-000000000000. No academy found with the id of: 10000003-0000-0ff1-ce00-000000000001. No academy found with the id of: 20000003-0000-0ff1-ce00-000000000002",
                (string) castedResult.Value);
            Assert.Equal(422, castedResult.StatusCode);
        }

        [Fact]
        public void InsertTrusts_CheckingReferencedTrusts_OneTrustNotFound()
        {
            //Arrange 
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")},
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("10000003-0000-0ff1-ce00-000000000001")},
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")
                            }
                        }
                    }
                },
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel {TrustId = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")}
                }
            };

            //Set up academy id checks to all pass
            _academiesRepository.Setup(r => r.GetAcademyById(Guid.Parse("00000003-0000-0ff1-ce00-000000000000")))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")}
                });
            _academiesRepository.Setup(r => r.GetAcademyById(Guid.Parse("10000003-0000-0ff1-ce00-000000000001")))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = Guid.Parse("10000003-0000-0ff1-ce00-000000000001")}
                });
            _academiesRepository.Setup(r => r.GetAcademyById(Guid.Parse("20000003-0000-0ff1-ce00-000000000002")))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = Guid.Parse("20000003-0000-0ff1-ce00-000000000002")}
                });

            //Set up the trust id checks to all pass but one
            _trustsRepository.Setup(r => r.GetTrustById(Guid.Parse("30000003-0000-0ff1-ce00-000000000003")))
                .ReturnsAsync(new RepositoryResult<GetTrustsD365Model>
                    {Result = new GetTrustsD365Model {Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")}});

            //This trust check will fail
            _trustsRepository.Setup(r => r.GetTrustById(Guid.Parse("40000003-0000-0ff1-ce00-000000000004")))
                .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> {Result = null});

            //Execute
            var result = _subject.InsertProject(request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be an Unprocessable Entity
            Assert.Equal("No trust found with the id of: 40000003-0000-0ff1-ce00-000000000004",
                (string) castedResult.Value);
            Assert.Equal(422, castedResult.StatusCode);
        }

        [Fact]
        public void InsertTrusts_CheckingReferencedTrusts_NoTrustFound()
        {
            //Arrange 
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")},
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("10000003-0000-0ff1-ce00-000000000001")},
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")
                            }
                        }
                    }
                },
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel {TrustId = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")}
                }
            };

            //Set up academy id checks to all pass
            _academiesRepository.Setup(r => r.GetAcademyById(Guid.Parse("00000003-0000-0ff1-ce00-000000000000")))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")}
                });
            _academiesRepository.Setup(r => r.GetAcademyById(Guid.Parse("10000003-0000-0ff1-ce00-000000000001")))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = Guid.Parse("10000003-0000-0ff1-ce00-000000000001")}
                });
            _academiesRepository.Setup(r => r.GetAcademyById(Guid.Parse("20000003-0000-0ff1-ce00-000000000002")))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = Guid.Parse("20000003-0000-0ff1-ce00-000000000002")}
                });

            //Set up the trust id checks to all fail
            _trustsRepository.Setup(r => r.GetTrustById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> {Result = null});

            //Execute
            var result = _subject.InsertProject(request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be an Unprocessable Entity with the trust messages concatenated
            Assert.Equal(
                "No trust found with the id of: 30000003-0000-0ff1-ce00-000000000003. No trust found with the id of: 40000003-0000-0ff1-ce00-000000000004",
                (string) castedResult.Value);
            Assert.Equal(422, castedResult.StatusCode);
        }

        [Fact]
        public void InsertTrusts_AllTrustsAndAcademiesVerified_InsertingProject_RepoFailure()
        {
            //Arrange 
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")},
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("10000003-0000-0ff1-ce00-000000000001")},
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")
                            }
                        }
                    }
                },
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel {TrustId = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")}
                }
            };

            //Set up academy id checks to all pass
            _academiesRepository.Setup(r => r.GetAcademyById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")}
                });

            //Set up the trust id checks to all pass
            _trustsRepository.Setup(r => r.GetTrustById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetTrustsD365Model>
                    {Result = new GetTrustsD365Model {Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")}});

            //Set up project repository to return a bad request when inserting a project
            _projectsRepository.Setup(r => r.InsertProject(It.IsAny<PostProjectsRequestModel>()))
                .ReturnsAsync(new RepositoryResult<Guid?>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "Bad request error message"
                    }
                });


            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });

            //Execute
            var result = _subject.InsertProject(request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string) castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void InsertTrusts_AllTrustsAndAcademiesVerified_InsertSuccessful_FailureWhenReloadingFromRepo()
        {
            //Arrange 
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")},
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("10000003-0000-0ff1-ce00-000000000001")},
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")
                            }
                        }
                    }
                },
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel {TrustId = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")}
                }
            };

            //Set up academy id checks to all pass
            _academiesRepository.Setup(r => r.GetAcademyById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")}
                });

            //Set up the trust id checks to all pass
            _trustsRepository.Setup(r => r.GetTrustById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetTrustsD365Model>
                    {Result = new GetTrustsD365Model {Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")}});

            //Set up project repository to a valid set result when inserting a project
            _projectsRepository.Setup(r => r.InsertProject(It.IsAny<PostProjectsRequestModel>()))
                .ReturnsAsync(new RepositoryResult<Guid?>
                {
                    Result = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")
                });

            //Set up project repo to fail when obtaining the full project from the repo
            _projectsRepository.Setup(r => r.GetProjectById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetProjectsResponseModel>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "Bad request error message"
                    }
                });

            //Set up repository error handler to handle the bad request result described above
            _repositoryErrorHandler.Setup(h =>
                    h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e =>
                        e.Error.StatusCode == HttpStatusCode.BadRequest)))
                .Returns(new ObjectResult("Some error message")
                {
                    StatusCode = 499
                });

            //Execute
            var result = _subject.InsertProject(request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string) castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void InsertTrusts_Success()
        {
            //Arrange 
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")},
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("10000003-0000-0ff1-ce00-000000000001")},
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")
                            }
                        }
                    }
                },
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel {TrustId = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")}
                }
            };

            //Set up academy id checks to all pass
            _academiesRepository.Setup(r => r.GetAcademyById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel {Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")}
                });

            //Set up the trust id checks to all pass
            _trustsRepository.Setup(r => r.GetTrustById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetTrustsD365Model>
                    {Result = new GetTrustsD365Model {Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003")}});

            //Set up project repository to a valid set result when inserting a project
            _projectsRepository.Setup(r => r.InsertProject(It.IsAny<PostProjectsRequestModel>()))
                .ReturnsAsync(new RepositoryResult<Guid?>
                {
                    Result = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")
                });

            //Set up project repo to fail when obtaining the full project from the repo
            _projectsRepository.Setup(r => r.GetProjectById(It.IsAny<Guid>()))
                .ReturnsAsync(new RepositoryResult<GetProjectsResponseModel>
                {
                    Result = new GetProjectsResponseModel
                        {ProjectId = Guid.Parse("80000003-0000-0ff1-ce00-000000000008")}
                });

            //Execute
            var result = _subject.InsertProject(request);
            var castedResult = (ObjectResult) result.Result;

            //Assert

            //Final result should be a 200OK wrapped around what the final mapper returns
            Assert.Equal(Guid.Parse("80000003-0000-0ff1-ce00-000000000008"),
                ((GetProjectsResponseModel) castedResult.Value).ProjectId);
            Assert.Equal(201, castedResult.StatusCode);
        }

        #endregion
    }
}