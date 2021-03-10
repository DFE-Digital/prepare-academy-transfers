using System;
using System.Collections.Generic;
using System.Net;
using API.Controllers;
using API.Mapping;
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
        //These default mocks are passed to the constructor in tests for methods that don't use them
        private readonly IProjectsRepository _projectsRepository;
        private readonly IAcademiesRepository _academiesRepository;
        private readonly ITrustsRepository _trustsRepository;
        private readonly IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model> _postProjectsMapper;
        private readonly IMapper<GetProjectsD365Model, GetProjectsResponseModel> _getProjectsMapper;
        private readonly IMapper<AcademyTransfersProjectAcademy,
                                 GetProjectsAcademyResponseModel> _getProjectAcademyMapper;
        private readonly IMapper<PutProjectAcademiesRequestModel, PatchProjectAcademiesD365Model> _putProjectAcademiesMapper;
        private readonly IMapper<SearchProjectsD365PageModel, SearchProjectsPageModel> _searchProjectsMapper;
        private readonly IRepositoryErrorResultHandler _repositoryErrorHandler;
        private readonly IConfiguration _config;

        public ProjectsControllerTests()
        {
            _projectsRepository = new Mock<IProjectsRepository>().Object;
            _academiesRepository = new Mock<IAcademiesRepository>().Object;
            _trustsRepository = new Mock<ITrustsRepository>().Object;


            _postProjectsMapper = new Mock<IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model>>().Object;
            _getProjectsMapper = new Mock<IMapper<GetProjectsD365Model, GetProjectsResponseModel>>().Object;
            _getProjectAcademyMapper = new Mock<IMapper<AcademyTransfersProjectAcademy,
                                 GetProjectsAcademyResponseModel>>().Object;
            _putProjectAcademiesMapper = new Mock<IMapper<PutProjectAcademiesRequestModel, PatchProjectAcademiesD365Model>>().Object;
            _searchProjectsMapper = new Mock<IMapper<SearchProjectsD365PageModel, SearchProjectsPageModel>>().Object;
            _repositoryErrorHandler = new Mock<IRepositoryErrorResultHandler>().Object;
            _config = new Mock<IConfiguration>().Object;
        }

        #region Search Projects Tests

        [Fact]
        public void SearchProjects_DefaultPaging_Test()
        {
            //Arrange 
            var projectRepository = new Mock<IProjectsRepository>();

            var controller = new ProjectsController(projectRepository.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, _repositoryErrorHandler, _config);

            //Execute
            var result = controller.SearchProjects(string.Empty, ProjectStatusEnum.Completed, null, null, null);

            //Assert that defaults are applied
            projectRepository.Verify(p => p.SearchProject(string.Empty, Models.D365.Enums.ProjectStatusEnum.Completed, true, 10, 1), Times.Once);
        }

        [Fact]
        public void SearchProjects_InvalidPageSize_ReturnsBadRequest()
        {
            //Arrange 
            var controller = new ProjectsController(_projectsRepository, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, _repositoryErrorHandler, _config);

            //Execute
            var result = controller.SearchProjects(string.Empty, ProjectStatusEnum.Completed, true, 0, 1);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be a 400 Bad Request
            Assert.Equal("Page size cannot be zero", (string)castedResult.Value);
            Assert.Equal(400, castedResult.StatusCode);
        }

        [Fact]
        public void SearchProjects_InvalidPageNumber_ReturnsBadRequest()
        {
            //Arrange 
            var controller = new ProjectsController(_projectsRepository, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, _repositoryErrorHandler, _config);

            //Execute
            var result = controller.SearchProjects(string.Empty, ProjectStatusEnum.Completed, true, 1, 0);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be a 400 Bad Request
            Assert.Equal("Page number cannot be 0", (string)castedResult.Value);
            Assert.Equal(400, castedResult.StatusCode);
        }

        [Fact]
        public void SearchProjects_InvalidProjectRepoResult()
        {
            //Arrange 

            //Set up project repository to return a bad request
            var projectRepoMock = new Mock<IProjectsRepository>();
            projectRepoMock.Setup(r => r.SearchProject("searchQuery", Models.D365.Enums.ProjectStatusEnum.Completed, true, 10, 1))
                           .ReturnsAsync(new RepositoryResult<SearchProjectsD365PageModel>
                           {
                               Error = new RepositoryResultBase.RepositoryError
                               {
                                   StatusCode = HttpStatusCode.BadRequest,
                                   ErrorMessage = "Bad request error message"
                               }
                           });

            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });


            var controller = new ProjectsController(projectRepoMock.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.SearchProjects("searchQuery", ProjectStatusEnum.Completed, true, 10, 1);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be a what the Error Handler returns
            Assert.Equal("Some error message", (string)castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void SearchProjects_OkRepoResult()
        {
            //Arrange 

            //Set up project repository to return a good request
            var projectRepoMock = new Mock<IProjectsRepository>();
            projectRepoMock.Setup(r => r.SearchProject("searchQuery", Models.D365.Enums.ProjectStatusEnum.Completed, true, 2, 1))
                           .ReturnsAsync(new RepositoryResult<SearchProjectsD365PageModel>
                           {
                               Result = new SearchProjectsD365PageModel
                               {
                                   Projects = new List<SearchProjectsD365Model>
                                   {
                                       new SearchProjectsD365Model {ProjectName = "AT-10000"},
                                       new SearchProjectsD365Model {ProjectName = "AT-10001"}
                                   },
                                   CurrentPage = 1,
                                   TotalPages = 3
                               }
                           });

            //Set up mapper to return slim mapped objecs
            var mapper = new Mock<IMapper<SearchProjectsD365PageModel, SearchProjectsPageModel>>();
            mapper.Setup(m => m.Map(It.Is<SearchProjectsD365PageModel>(s => s.CurrentPage == 1 && s.TotalPages == 3 && s.Projects.Count == 2)))
                  .Returns(new SearchProjectsPageModel
                  {
                      Projects = new List<SearchProjectsModel>
                                   {
                                       new SearchProjectsModel {ProjectName = "Mapped AT-10000"},
                                       new SearchProjectsModel{ProjectName = "Mapped AT-10001"}
                                   },
                      CurrentPage = 1,
                      TotalPages = 3
                  });


            var controller = new ProjectsController(projectRepoMock.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, mapper.Object, _repositoryErrorHandler, _config);

            //Execute
            var result = controller.SearchProjects("searchQuery", ProjectStatusEnum.Completed, true, 2, 1);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be a 200OK wrapped around what the mapper returns
            Assert.Equal(1, ((SearchProjectsPageModel)castedResult.Value).CurrentPage);
            Assert.Equal(3, ((SearchProjectsPageModel)castedResult.Value).TotalPages);
            Assert.Equal(2, ((SearchProjectsPageModel)castedResult.Value).Projects.Count);
            Assert.Equal("Mapped AT-10000", ((SearchProjectsPageModel)castedResult.Value).Projects[0].ProjectName);
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
            var projectRepository = new Mock<IProjectsRepository>();
            projectRepository.Setup(p => p.GetProjectById(projectId))
                             .ReturnsAsync(new RepositoryResult<GetProjectsD365Model>
                             {
                                 Error = new RepositoryResultBase.RepositoryError
                                 {
                                     StatusCode = HttpStatusCode.BadRequest,
                                     ErrorMessage = "Bad request error message"
                                 }
                             });

            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });

            var controller = new ProjectsController(projectRepository.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.GetProjectById(projectId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string)castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void GetProjectById_ProjectNotFound()
        {
            //Arrange
            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");

            //Set up project repository to return null when getting a project by id
            var projectRepository = new Mock<IProjectsRepository>();
            projectRepository.Setup(p => p.GetProjectById(projectId))
                             .ReturnsAsync(new RepositoryResult<GetProjectsD365Model>
                             {
                                 Result = null
                             });

            
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var controller = new ProjectsController(projectRepository.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.GetProjectById(projectId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Repository Error Handler result should not be called
            repositoryErrorHandlerMock.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Final result should be 404 Not Found with the proper message
            Assert.Equal("Project with id '00000003-0000-0ff1-ce00-000000000000' not found", (string)castedResult.Value);
            Assert.Equal(404, castedResult.StatusCode);
        }

        [Fact]
        public void GetProjectById_ProjectFound()
        {
            //Arrange
            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");

            //Set up project repository to return a valid set result
            var projectRepository = new Mock<IProjectsRepository>();
            projectRepository.Setup(p => p.GetProjectById(projectId))
                             .ReturnsAsync(new RepositoryResult<GetProjectsD365Model>
                             {
                                 Result = new GetProjectsD365Model
                                 {
                                     ProjectName = "AT-10000"
                                 }
                             });

            //Set up mapper to return slim objects when called with the expected input
            var projectMapper = new Mock<IMapper<GetProjectsD365Model, GetProjectsResponseModel>>();
            projectMapper.Setup(m => m.Map(It.Is<GetProjectsD365Model>(p => p.ProjectName == "AT-10000")))
                         .Returns(new GetProjectsResponseModel { ProjectName = "Mapped AT-10000" });
                        

            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var controller = new ProjectsController(projectRepository.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, projectMapper.Object,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.GetProjectById(projectId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Repository Error Handler result should not be called
            repositoryErrorHandlerMock.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Final result should be 200 OK wrapped around the mapped result
            Assert.Equal("Mapped AT-10000", ((GetProjectsResponseModel)castedResult.Value).ProjectName);
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
            var projectRepository = new Mock<IProjectsRepository>();
            projectRepository.Setup(p => p.GetProjectById(projectId))
                             .ReturnsAsync(new RepositoryResult<GetProjectsD365Model>
                             {
                                 Error = new RepositoryResultBase.RepositoryError
                                 {
                                     StatusCode = HttpStatusCode.BadRequest,
                                     ErrorMessage = "Bad request error message"
                                 }
                             });

            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });

            var controller = new ProjectsController(projectRepository.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.GetProjectAcademy(projectId, projectAcademyId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string)castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);

        }

        [Fact]
        public void GetProjectAcademyById_ProjectNotFound()
        {
            //Arrange 

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");

            //Set up project repository to return an a null result
            var projectRepository = new Mock<IProjectsRepository>();
            projectRepository.Setup(p => p.GetProjectById(projectId))
                             .ReturnsAsync(new RepositoryResult<GetProjectsD365Model>
                             {
                                 Result = null
                             });

            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var controller = new ProjectsController(projectRepository.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.GetProjectAcademy(projectId, projectAcademyId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Repository Error Handler result should not be called
            repositoryErrorHandlerMock.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Final result should be a what the 404 Not Found with the correct message
            Assert.Equal("Project with id '00000003-0000-0ff1-ce00-000000000000' not found", (string)castedResult.Value);
            Assert.Equal(404, castedResult.StatusCode);

        }

        [Fact]
        public void GetProjectAcademyById_ProjectAcademy_RepoFailure()
        {
            //Arrange 

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");

            //Set up project repository to return a valid set result
            var projectRepository = new Mock<IProjectsRepository>();
            projectRepository.Setup(p => p.GetProjectById(projectId))
                             .ReturnsAsync(new RepositoryResult<GetProjectsD365Model>
                             {
                                 Result = new GetProjectsD365Model { ProjectName = "AT-1000"}
                             });

            //Set up project repo to return a failed result when getting the Project Academy by id
            projectRepository.Setup(p => p.GetProjectAcademyById(projectAcademyId))
                             .ReturnsAsync(new RepositoryResult<AcademyTransfersProjectAcademy>
                             {
                                 Error = new RepositoryResultBase.RepositoryError
                                 {
                                     StatusCode = HttpStatusCode.BadRequest,
                                     ErrorMessage = "Bad request error message"
                                 }
                             });

            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });

            var controller = new ProjectsController(projectRepository.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.GetProjectAcademy(projectId, projectAcademyId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be a what the Error Handler returns
            Assert.Equal("Some error message", (string)castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void GetProjectAcademyById_ProjectAcademy_NotFound()
        {
            //Arrange 

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");

            //Set up project repository to return a valid set found project
            var projectRepository = new Mock<IProjectsRepository>();
            projectRepository.Setup(p => p.GetProjectById(projectId))
                             .ReturnsAsync(new RepositoryResult<GetProjectsD365Model>
                             {
                                 Result = new GetProjectsD365Model { ProjectName = "AT-1000" }
                             });

            //Set up project repo to return a null result when getting the Project Academy by id
            projectRepository.Setup(p => p.GetProjectAcademyById(projectAcademyId))
                             .ReturnsAsync(new RepositoryResult<AcademyTransfersProjectAcademy>
                             {
                                 Result = null
                             });

            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var controller = new ProjectsController(projectRepository.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.GetProjectAcademy(projectId, projectAcademyId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Repository Error Handler result should not be called
            repositoryErrorHandlerMock.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Final result should be a what the 404 Not Found with the correct message
            Assert.Equal("Project Academy with id '00000003-0000-0ff1-ce00-000000000001' not found", (string)castedResult.Value);
            Assert.Equal(404, castedResult.StatusCode);
        }

        [Fact]
        public void GetProjectAcademyById_ProjectAcademy_Found()
        {
            //Arrange 

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");

            //Set up project repository to return a valid set result when getting the project by id
            var projectRepository = new Mock<IProjectsRepository>();
            projectRepository.Setup(p => p.GetProjectById(projectId))
                             .ReturnsAsync(new RepositoryResult<GetProjectsD365Model>
                             {
                                 Result = new GetProjectsD365Model { ProjectName = "AT-1000" }
                             });

            //Set up project repo to return a valid set result when getting the Project Academy by id
            projectRepository.Setup(p => p.GetProjectAcademyById(projectAcademyId))
                             .ReturnsAsync(new RepositoryResult<AcademyTransfersProjectAcademy>
                             {
                                 Result = new AcademyTransfersProjectAcademy { AcademyName = "Project Academy 001"}
                             });

            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            //Set up ProjectAcademy mapper to return slim result
            var projectAcademyMapper = new Mock<IMapper<AcademyTransfersProjectAcademy,
                                 GetProjectsAcademyResponseModel>>();

            projectAcademyMapper.Setup(m => m.Map(It.Is<AcademyTransfersProjectAcademy>(a => a.AcademyName == "Project Academy 001")))
                                .Returns(new GetProjectsAcademyResponseModel { AcademyName = "Mapped Project Academy 001" });

            var controller = new ProjectsController(projectRepository.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                projectAcademyMapper.Object, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.GetProjectAcademy(projectId, projectAcademyId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            repositoryErrorHandlerMock.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Final result should be 200 OK wrapped around the mapped result
            Assert.Equal("Mapped Project Academy 001", ((GetProjectsAcademyResponseModel)castedResult.Value).AcademyName);
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
            var projectRepositoryMock = new Mock<IProjectsRepository>();
            projectRepositoryMock.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                                 .ReturnsAsync(new RepositoryResult<AcademyTransfersProjectAcademy>
                                 {
                                     Error = new RepositoryResultBase.RepositoryError
                                     {
                                         StatusCode = HttpStatusCode.BadRequest,
                                         ErrorMessage = "Bad request error message"
                                     }
                                 });


            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });

            var controller = new ProjectsController(projectRepositoryMock.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.UpdateProjectAcademy(projectId, projectAcademyId, null);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string)castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void UpdateProjectAcademy_NotFound_WhenGettingProjectAcademyById()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");

            //Set up project repository to return a null result when getting a ProjectAcademy by id
            var projectRepositoryMock = new Mock<IProjectsRepository>();
            projectRepositoryMock.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                                 .ReturnsAsync(new RepositoryResult<AcademyTransfersProjectAcademy>
                                 {
                                     Result = null
                                 });

            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var controller = new ProjectsController(projectRepositoryMock.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.UpdateProjectAcademy(projectId, projectAcademyId, null);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Repository Error Handler result should not be called
            repositoryErrorHandlerMock.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Final result should be a 404 Not Found with the correct message
            Assert.Equal("Project Academy with id '00000003-0000-0ff1-ce00-000000000001' not found", (string)castedResult.Value);
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
            var projectRepositoryMock = new Mock<IProjectsRepository>();
            projectRepositoryMock.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                                 .ReturnsAsync(new RepositoryResult<AcademyTransfersProjectAcademy>
                                 {
                                     Result = new AcademyTransfersProjectAcademy { ProjectId = mismatchingProjectId }
                                 });

            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var controller = new ProjectsController(projectRepositoryMock.Object, _academiesRepository, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.UpdateProjectAcademy(projectId, projectAcademyId, null);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Repository Error Handler result should not be called
            repositoryErrorHandlerMock.Verify(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Final result should be an Unprocessable Entity with the correct message
            Assert.Equal("Project Academy with id '00000003-0000-0ff1-ce00-000000000001' not found within project with id '00000003-0000-0ff1-ce00-000000000000'", (string)castedResult.Value);
            Assert.Equal(422, castedResult.StatusCode);
        }

        [Fact]
        public void UpdateProjectAcademy_ReferencedAcademyId_RepoFailure()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");
            var referencedAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000003");

            var request = new PutProjectAcademiesRequestModel { AcademyId = referencedAcademyId };

            //Set up project repository to return a ProjectAcademy with a matching ProjectId
            var projectRepositoryMock = new Mock<IProjectsRepository>();
            projectRepositoryMock.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                                 .ReturnsAsync(new RepositoryResult<AcademyTransfersProjectAcademy>
                                 {
                                     Result = new AcademyTransfersProjectAcademy { ProjectId = projectId }
                                 });

            //Set up the academies repository to return bad request when verifying the referenced academy
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(m => m.GetAcademyById(referencedAcademyId))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Error = new RepositoryResultBase.RepositoryError
                                       {
                                           StatusCode = HttpStatusCode.BadRequest,
                                           ErrorMessage = "Bad request error message"
                                       }
                                   });


            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });

            var controller = new ProjectsController(projectRepositoryMock.Object, academiesRepositoryMock.Object, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.UpdateProjectAcademy(projectId, projectAcademyId, request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string)castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void UpdateProjectAcademy_ReferencedAcademyId_NotFound()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");
            var referencedAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000003");

            var request = new PutProjectAcademiesRequestModel { AcademyId = referencedAcademyId };

            //Set up project repository to return a ProjectAcademy with a matching ProjectId
            var projectRepositoryMock = new Mock<IProjectsRepository>();
            projectRepositoryMock.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                                 .ReturnsAsync(new RepositoryResult<AcademyTransfersProjectAcademy>
                                 {
                                     Result = new AcademyTransfersProjectAcademy { ProjectId = projectId }
                                 });

            //Set up the academies repository to return a null result when verifying the referenced academy
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(m => m.GetAcademyById(referencedAcademyId))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = null
                                   });


            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var controller = new ProjectsController(projectRepositoryMock.Object, academiesRepositoryMock.Object, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.UpdateProjectAcademy(projectId, projectAcademyId, request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be an Unprocessable Entity
            Assert.Equal($"No academy found with the id of: {referencedAcademyId}", (string)castedResult.Value);
            Assert.Equal(422, castedResult.StatusCode);
        }

        [Fact]
        public void UpdateProjectAcademy_WhenUpdatingEntity_RepositoryFailure()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");
            var referencedAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000003");

            var request = new PutProjectAcademiesRequestModel { AcademyId = referencedAcademyId };

            //Set up project repository to return a ProjectAcademy with a matching ProjectId
            var projectRepositoryMock = new Mock<IProjectsRepository>();
            projectRepositoryMock.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                                 .ReturnsAsync(new RepositoryResult<AcademyTransfersProjectAcademy>
                                 {
                                     Result = new AcademyTransfersProjectAcademy { ProjectId = projectId }
                                 });

            //Set up the projects repository to fail when updating the Project Academy entity
            projectRepositoryMock.Setup(r => r.UpdateProjectAcademy(It.IsAny<Guid>(), It.IsAny<PatchProjectAcademiesD365Model>()))
                                 .ReturnsAsync(new RepositoryResult<Guid?>
                                 {
                                     Error = new RepositoryResultBase.RepositoryError
                                     {
                                         StatusCode = HttpStatusCode.BadRequest,
                                         ErrorMessage = "Bad request error message"
                                     }
                                 });

            //Set up the academies repository to return a null result when verifying the referenced academy
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(m => m.GetAcademyById(referencedAcademyId))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = referencedAcademyId }
                                   });

            //Set up the mapper to return a slimmed result
            var putProjectAcademyMapper = new Mock<IMapper<PutProjectAcademiesRequestModel, PatchProjectAcademiesD365Model>>();
            putProjectAcademyMapper.Setup(m => m.Map(It.Is<PutProjectAcademiesRequestModel>(p => p.AcademyId == referencedAcademyId)))
                                   .Returns(new PatchProjectAcademiesD365Model { AcademyId = referencedAcademyId.ToString() });

            

            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });


            var controller = new ProjectsController(projectRepositoryMock.Object, academiesRepositoryMock.Object, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, putProjectAcademyMapper.Object, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.UpdateProjectAcademy(projectId, projectAcademyId, request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string)castedResult.Value);
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

            var request = new PutProjectAcademiesRequestModel { AcademyId = referencedAcademyId };

            //Set up project repository to return a ProjectAcademy with a matching ProjectId
            var projectRepositoryMock = new Mock<IProjectsRepository>();
            projectRepositoryMock.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                                 .ReturnsAsync(new RepositoryResult<AcademyTransfersProjectAcademy>
                                 {
                                     Result = new AcademyTransfersProjectAcademy { ProjectId = projectId }
                                 });

            //Set up project repository to fail when refreshing the ProjectAcademy entity
            projectRepositoryMock.Setup(r => r.GetProjectAcademyById(alternativeProjectAcademyIdForFailingRepo))
                                 .ReturnsAsync(new RepositoryResult<AcademyTransfersProjectAcademy>
                                 {
                                     Error = new RepositoryResultBase.RepositoryError
                                     {
                                         StatusCode = HttpStatusCode.BadRequest,
                                         ErrorMessage = "Bad request error message"
                                     }
                                 });

            //Set up the projects repository to return a valid set result when updating the entity
            projectRepositoryMock.Setup(r => r.UpdateProjectAcademy(It.IsAny<Guid>(), It.IsAny<PatchProjectAcademiesD365Model>()))
                                 .ReturnsAsync(new RepositoryResult<Guid?>
                                 {
                                     Result = alternativeProjectAcademyIdForFailingRepo
                                 });

            //Set up the academies repository to return a null result when verifying the referenced academy
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(m => m.GetAcademyById(referencedAcademyId))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = referencedAcademyId }
                                   });

            //Set up the mapper to return a slimmed result
            var putProjectAcademyMapper = new Mock<IMapper<PutProjectAcademiesRequestModel, PatchProjectAcademiesD365Model>>();
            putProjectAcademyMapper.Setup(m => m.Map(It.Is<PutProjectAcademiesRequestModel>(p => p.AcademyId == referencedAcademyId)))
                                   .Returns(new PatchProjectAcademiesD365Model { AcademyId = referencedAcademyId.ToString() });


            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });


            var controller = new ProjectsController(projectRepositoryMock.Object, academiesRepositoryMock.Object, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                _getProjectAcademyMapper, putProjectAcademyMapper.Object, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.UpdateProjectAcademy(projectId, projectAcademyId, request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be a what the Error Handler returns
            Assert.Equal("Some error message", (string)castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public void UpdateProjectAcademy_Success()
        {
            //Arrange

            var projectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000");
            var projectAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000001");
            var referencedAcademyId = Guid.Parse("00000003-0000-0ff1-ce00-000000000003");

            var request = new PutProjectAcademiesRequestModel { AcademyId = referencedAcademyId };

            //Set up project repository to return a ProjectAcademy with a matching ProjectId
            var projectRepositoryMock = new Mock<IProjectsRepository>();
            projectRepositoryMock.Setup(r => r.GetProjectAcademyById(projectAcademyId))
                                 .ReturnsAsync(new RepositoryResult<AcademyTransfersProjectAcademy>
                                 {
                                     Result = new AcademyTransfersProjectAcademy { ProjectId = projectId }
                                 });


            //Set up the projects repository to return a valid set result when updating the entity
            projectRepositoryMock.Setup(r => r.UpdateProjectAcademy(It.IsAny<Guid>(), It.IsAny<PatchProjectAcademiesD365Model>()))
                                 .ReturnsAsync(new RepositoryResult<Guid?>
                                 {
                                     Result = projectAcademyId
                                 });

            //Set up the academies repository to return a valid set result when verifying the referenced academy
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(m => m.GetAcademyById(referencedAcademyId))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = referencedAcademyId }
                                   });

            //Set up the mapper to return a slimmed result
            var putProjectAcademyMapper = new Mock<IMapper<PutProjectAcademiesRequestModel, PatchProjectAcademiesD365Model>>();
            putProjectAcademyMapper.Setup(m => m.Map(It.Is<PutProjectAcademiesRequestModel>(p => p.AcademyId == referencedAcademyId)))
                                   .Returns(new PatchProjectAcademiesD365Model { AcademyId = referencedAcademyId.ToString() });



            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });

            //Set up mapper to return final result back
            var getProjectAcademyMapper = new Mock<IMapper<AcademyTransfersProjectAcademy,
                                 GetProjectsAcademyResponseModel>>();
            getProjectAcademyMapper.Setup(m => m.Map(It.Is<AcademyTransfersProjectAcademy>(a => a.ProjectId == projectId)))
                                   .Returns(new GetProjectsAcademyResponseModel { ProjectAcademyId = projectAcademyId });

            var controller = new ProjectsController(projectRepositoryMock.Object, academiesRepositoryMock.Object, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
                getProjectAcademyMapper.Object, putProjectAcademyMapper.Object, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.UpdateProjectAcademy(projectId, projectAcademyId, request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be a 200 OK wrapped around the output of the final mapper
            Assert.Equal(projectAcademyId, ((GetProjectsAcademyResponseModel)castedResult.Value).ProjectAcademyId);
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
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
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
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(r => r.GetAcademyById(It.IsAny<Guid>()))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Error = new RepositoryResultBase.RepositoryError
                                       {
                                           StatusCode = HttpStatusCode.BadRequest,
                                           ErrorMessage = "Bad request error message"
                                       }
                                   });

            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });

            var controller = new ProjectsController(_projectsRepository, academiesRepositoryMock.Object, _trustsRepository, _postProjectsMapper, _getProjectsMapper,
               _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.InsertProject(request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string)castedResult.Value);
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
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
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
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(r => r.GetAcademyById(It.IsAny<Guid>()))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel()
                                   });

            //Set up the trust id checks to return a bad request
            var trustsRepository = new Mock<ITrustsRepository>();
            trustsRepository.Setup(r => r.GetTrustById(It.IsAny<Guid>()))
                            .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> 
                            {
                                Error = new RepositoryResultBase.RepositoryError
                                {
                                    StatusCode = HttpStatusCode.BadRequest,
                                    ErrorMessage = "Bad request error message"
                                }
                            });


            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });

            var controller = new ProjectsController(_projectsRepository, academiesRepositoryMock.Object, trustsRepository.Object, _postProjectsMapper, _getProjectsMapper,
               _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.InsertProject(request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string)castedResult.Value);
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
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
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
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(r => r.GetAcademyById(Guid.Parse("00000003-0000-0ff1-ce00-000000000000")))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000")}
                                   });

            academiesRepositoryMock.Setup(r => r.GetAcademyById(Guid.Parse("10000003-0000-0ff1-ce00-000000000001")))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000") }
                                   });
            //This is the one academy that won't be found
            academiesRepositoryMock.Setup(r => r.GetAcademyById(Guid.Parse("20000003-0000-0ff1-ce00-000000000002")))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = null 
                                   });

            //Set up the trust id checks to all pass
            var trustsRepository = new Mock<ITrustsRepository>();
            trustsRepository.Setup(r => r.GetTrustById(Guid.Parse("30000003-0000-0ff1-ce00-000000000003")))
                            .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> { Result = new GetTrustsD365Model { Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003") } });
            trustsRepository.Setup(r => r.GetTrustById(Guid.Parse("40000003-0000-0ff1-ce00-000000000004")))
                            .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> { Result = new GetTrustsD365Model { Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003") } });

            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var controller = new ProjectsController(_projectsRepository, academiesRepositoryMock.Object, trustsRepository.Object, _postProjectsMapper, _getProjectsMapper,
               _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.InsertProject(request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be an Unprocessable Entity
            Assert.Equal("No academy found with the id of: 20000003-0000-0ff1-ce00-000000000002", (string)castedResult.Value);
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
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
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
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();

            //All academies will fail
            academiesRepositoryMock.Setup(r => r.GetAcademyById(It.IsAny<Guid>()))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = null
                                   });

            //Set up the trust id checks to all pass
            var trustsRepository = new Mock<ITrustsRepository>();
            trustsRepository.Setup(r => r.GetTrustById(Guid.Parse("30000003-0000-0ff1-ce00-000000000003")))
                            .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> { Result = new GetTrustsD365Model { Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003") } });

            trustsRepository.Setup(r => r.GetTrustById(Guid.Parse("40000003-0000-0ff1-ce00-000000000004")))
                            .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> { Result = new GetTrustsD365Model { Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003") } });

            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var controller = new ProjectsController(_projectsRepository, academiesRepositoryMock.Object, trustsRepository.Object, _postProjectsMapper, _getProjectsMapper,
               _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.InsertProject(request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be an Unprocessable Entity
            Assert.Equal("No academy found with the id of: 00000003-0000-0ff1-ce00-000000000000. No academy found with the id of: 10000003-0000-0ff1-ce00-000000000001. No academy found with the id of: 20000003-0000-0ff1-ce00-000000000002", (string)castedResult.Value);
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
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
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
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(r => r.GetAcademyById(Guid.Parse("00000003-0000-0ff1-ce00-000000000000")))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000") }
                                   });
            academiesRepositoryMock.Setup(r => r.GetAcademyById(Guid.Parse("10000003-0000-0ff1-ce00-000000000001")))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = Guid.Parse("10000003-0000-0ff1-ce00-000000000001") }
                                   });
            academiesRepositoryMock.Setup(r => r.GetAcademyById(Guid.Parse("20000003-0000-0ff1-ce00-000000000002")))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = Guid.Parse("20000003-0000-0ff1-ce00-000000000002") }
                                   });

            //Set up the trust id checks to all pass but one
            var trustsRepository = new Mock<ITrustsRepository>();
            trustsRepository.Setup(r => r.GetTrustById(Guid.Parse("30000003-0000-0ff1-ce00-000000000003")))
                            .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> { Result = new GetTrustsD365Model { Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003") } });

            //This trust check will fail
            trustsRepository.Setup(r => r.GetTrustById(Guid.Parse("40000003-0000-0ff1-ce00-000000000004")))
                            .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> { Result = null });

            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var controller = new ProjectsController(_projectsRepository, academiesRepositoryMock.Object, trustsRepository.Object, _postProjectsMapper, _getProjectsMapper,
               _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.InsertProject(request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be an Unprocessable Entity
            Assert.Equal("No trust found with the id of: 40000003-0000-0ff1-ce00-000000000004", (string)castedResult.Value);
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
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
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
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(r => r.GetAcademyById(Guid.Parse("00000003-0000-0ff1-ce00-000000000000")))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000") }
                                   });
            academiesRepositoryMock.Setup(r => r.GetAcademyById(Guid.Parse("10000003-0000-0ff1-ce00-000000000001")))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = Guid.Parse("10000003-0000-0ff1-ce00-000000000001") }
                                   });
            academiesRepositoryMock.Setup(r => r.GetAcademyById(Guid.Parse("20000003-0000-0ff1-ce00-000000000002")))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = Guid.Parse("20000003-0000-0ff1-ce00-000000000002") }
                                   });

            //Set up the trust id checks to all fail
            var trustsRepository = new Mock<ITrustsRepository>();
            trustsRepository.Setup(r => r.GetTrustById(It.IsAny<Guid>()))
                            .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> { Result = null });

            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var controller = new ProjectsController(_projectsRepository, academiesRepositoryMock.Object, trustsRepository.Object, _postProjectsMapper, _getProjectsMapper,
               _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.InsertProject(request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be an Unprocessable Entity with the trust messages concatenated
            Assert.Equal("No trust found with the id of: 30000003-0000-0ff1-ce00-000000000003. No trust found with the id of: 40000003-0000-0ff1-ce00-000000000004", (string)castedResult.Value);
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
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
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
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(r => r.GetAcademyById(It.IsAny<Guid>()))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000") }
                                   });

            //Set up the trust id checks to all pass
            var trustsRepository = new Mock<ITrustsRepository>();
            trustsRepository.Setup(r => r.GetTrustById(It.IsAny<Guid>()))
                            .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> { Result = new GetTrustsD365Model { Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003") } });

            //Set up mapper to return a slim mock result
            var postProjectsMapper = new Mock<IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model>>();
            postProjectsMapper.Setup(m => m.Map(It.IsAny<PostProjectsRequestModel>()))
                              .Returns(new PostAcademyTransfersProjectsD365Model
                              {
                                  ProjectInitiatorFullName = "Joe Bloggs"
                              });

            //Set up project repository to return a bad request when inserting a project
            var projectsRepository = new Mock<IProjectsRepository>();
            projectsRepository.Setup(r => r.InsertProject(It.IsAny<PostAcademyTransfersProjectsD365Model>()))
                              .ReturnsAsync(new RepositoryResult<Guid?>
                              {
                                  Error = new RepositoryResultBase.RepositoryError
                                  {
                                      StatusCode = HttpStatusCode.BadRequest,
                                      ErrorMessage = "Bad request error message"
                                  }
                              });


            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });

            var controller = new ProjectsController(projectsRepository.Object, academiesRepositoryMock.Object, trustsRepository.Object, postProjectsMapper.Object, _getProjectsMapper,
               _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.InsertProject(request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string)castedResult.Value);
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
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
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
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(r => r.GetAcademyById(It.IsAny<Guid>()))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000") }
                                   });

            //Set up the trust id checks to all pass
            var trustsRepository = new Mock<ITrustsRepository>();
            trustsRepository.Setup(r => r.GetTrustById(It.IsAny<Guid>()))
                            .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> { Result = new GetTrustsD365Model { Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003") } });

            //Set up mapper to return a slim mock result
            var postProjectsMapper = new Mock<IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model>>();
            postProjectsMapper.Setup(m => m.Map(It.IsAny<PostProjectsRequestModel>()))
                              .Returns(new PostAcademyTransfersProjectsD365Model
                              {
                                  ProjectInitiatorFullName = "Joe Bloggs"
                              });


            //Set up project repository to a valid set result when inserting a project
            var projectsRepository = new Mock<IProjectsRepository>();
            projectsRepository.Setup(r => r.InsertProject(It.IsAny<PostAcademyTransfersProjectsD365Model>()))
                              .ReturnsAsync(new RepositoryResult<Guid?>
                              {
                                  Result = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")
                              });

            //Set up project repo to fail when obtaining the full project from the repo
            projectsRepository.Setup(r => r.GetProjectById(It.IsAny<Guid>()))
                              .ReturnsAsync(new RepositoryResult<GetProjectsD365Model>
                              {
                                  Error = new RepositoryResultBase.RepositoryError
                                  {
                                      StatusCode = HttpStatusCode.BadRequest,
                                      ErrorMessage = "Bad request error message"
                                  }
                              });

            //Set up repository error handler to handle the bad request result described above
            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            repositoryErrorHandlerMock.Setup(h => h.LogAndCreateResponse(It.Is<RepositoryResultBase>(e => e.Error.StatusCode == HttpStatusCode.BadRequest)))
                                      .Returns(new ObjectResult("Some error message")
                                      {
                                          StatusCode = 499
                                      });

            var controller = new ProjectsController(projectsRepository.Object, academiesRepositoryMock.Object, trustsRepository.Object, postProjectsMapper.Object, _getProjectsMapper,
               _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.InsertProject(request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be what the Error Handler returns
            Assert.Equal("Some error message", (string)castedResult.Value);
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
                    new PostProjectsAcademiesModel {AcademyId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
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
            var academiesRepositoryMock = new Mock<IAcademiesRepository>();
            academiesRepositoryMock.Setup(r => r.GetAcademyById(It.IsAny<Guid>()))
                                   .ReturnsAsync(new RepositoryResult<GetAcademiesModel>
                                   {
                                       Result = new GetAcademiesModel { Id = Guid.Parse("00000003-0000-0ff1-ce00-000000000000") }
                                   });

            //Set up the trust id checks to all pass
            var trustsRepository = new Mock<ITrustsRepository>();
            trustsRepository.Setup(r => r.GetTrustById(It.IsAny<Guid>()))
                            .ReturnsAsync(new RepositoryResult<GetTrustsD365Model> { Result = new GetTrustsD365Model { Id = Guid.Parse("30000003-0000-0ff1-ce00-000000000003") } });

            //Set up mapper to return a slim mock result
            var postProjectsMapper = new Mock<IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model>>();
            postProjectsMapper.Setup(m => m.Map(It.IsAny<PostProjectsRequestModel>()))
                              .Returns(new PostAcademyTransfersProjectsD365Model
                              {
                                  ProjectInitiatorFullName = "Joe Bloggs"
                              });


            //Set up project repository to a valid set result when inserting a project
            var projectsRepository = new Mock<IProjectsRepository>();
            projectsRepository.Setup(r => r.InsertProject(It.IsAny<PostAcademyTransfersProjectsD365Model>()))
                              .ReturnsAsync(new RepositoryResult<Guid?>
                              {
                                  Result = Guid.Parse("40000003-0000-0ff1-ce00-000000000004")
                              });

            //Set up project repo to fail when obtaining the full project from the repo
            projectsRepository.Setup(r => r.GetProjectById(It.IsAny<Guid>()))
                              .ReturnsAsync(new RepositoryResult<GetProjectsD365Model>
                              {
                                  Result = new GetProjectsD365Model { ProjectId = Guid.Parse("80000003-0000-0ff1-ce00-000000000008") }
                              });

            //Set up get projects mapper to return mock slim object
            var getProjectAcademyMapper = new Mock<IMapper<GetProjectsD365Model, GetProjectsResponseModel>>();
            getProjectAcademyMapper.Setup(m => m.Map(It.IsAny<GetProjectsD365Model>()))
                                   .Returns(new GetProjectsResponseModel { ProjectId = Guid.Parse("80000003-0000-0ff1-ce00-000000000008") });

            var repositoryErrorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var controller = new ProjectsController(projectsRepository.Object, academiesRepositoryMock.Object, trustsRepository.Object, postProjectsMapper.Object, getProjectAcademyMapper.Object,
               _getProjectAcademyMapper, _putProjectAcademiesMapper, _searchProjectsMapper, repositoryErrorHandlerMock.Object, _config);

            //Execute
            var result = controller.InsertProject(request);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Final result should be a 200OK wrapped around what the final mapper returns
            Assert.Equal(Guid.Parse("80000003-0000-0ff1-ce00-000000000008"), ((GetProjectsResponseModel)castedResult.Value).ProjectId);
            Assert.Equal(201, castedResult.StatusCode);
        }

        #endregion

    }
}