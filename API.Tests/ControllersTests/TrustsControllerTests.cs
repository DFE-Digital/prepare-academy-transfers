using API.Controllers;
using API.Mapping;
using API.Models.Downstream.D365;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace API.Tests.ControllersTests
{
    public class TrustsControllerTests
    {
        //These default mocks are passed to the constructor in tests for methods that don't use them
        private readonly Mock<ITrustsRepository> _trustRepostiory;
        private readonly Mock<IAcademiesRepository> _academiesRepository;
        private readonly Mock<IMapper<GetTrustsD365Model, GetTrustsModel>> _getTrustMapper;
        private readonly Mock<IMapper<GetAcademiesD365Model, GetAcademiesModel>> _getAcademiesMapper;
        private readonly Mock<IRepositoryErrorResultHandler> _repositoryErrorHandler;

        public TrustsControllerTests()
        {
            _trustRepostiory = new Mock<ITrustsRepository>();
            _academiesRepository = new Mock<IAcademiesRepository>();
            _getTrustMapper = new Mock<IMapper<GetTrustsD365Model, GetTrustsModel>>();
            _getAcademiesMapper = new Mock<IMapper<GetAcademiesD365Model, GetAcademiesModel>>();
            _repositoryErrorHandler = new Mock<IRepositoryErrorResultHandler>();
        }

        #region GetById Tests

        [Fact]
        public async void GetById_InvalidRepoResult_CallsRepositoryErrorHandler()
        {
            //Arrange
            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");

            //Set up trusts repo to return a failure
            var trustsRepoMock = new Mock<ITrustsRepository>();
            trustsRepoMock.Setup(m => m.GetTrustById(trustId))
                          .ReturnsAsync(new RepositoryResult<GetTrustsModel>
                          {
                              Error = new RepositoryResultBase.RepositoryError
                              {
                                  StatusCode = HttpStatusCode.BadRequest,
                                  ErrorMessage = "Bad Request"
                              }
                          });

            var mapperMock = new Mock<IMapper<GetTrustsD365Model, GetTrustsModel>>();

            //Set up error handler to return a random result
            var errorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            errorHandlerMock.Setup(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()))
                            .Returns(new ObjectResult("Some error message")
                            {
                                StatusCode = 499
                            });

            var trustsController = new TrustsController(trustsRepoMock.Object,
                                                        _academiesRepository.Object,
                                                        errorHandlerMock.Object);


            //Execute
            var result = await trustsController.GetById(trustId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should be called with result from repository
            errorHandlerMock.Verify(h => h.LogAndCreateResponse(
                It.Is<RepositoryResultBase>(r =>
                    r.Error.ErrorMessage == "Bad Request" &&
                    r.Error.StatusCode == HttpStatusCode.BadRequest)),
                Times.Once);
            //Mapper not to be called when repo fails
            mapperMock.Verify(m => m.Map(It.IsAny<GetTrustsD365Model>()), Times.Never);

            //Final result should be what the error handler returns
            Assert.Equal("Some error message", castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public async void GetById_NotFound_ReturnsNotFoundResult()
        {
            //Arrange
            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");

            //Set up mock repository to return a null result (not found)
            var trustsRepoMock = new Mock<ITrustsRepository>();
            trustsRepoMock.Setup(m => m.GetTrustById(trustId))
                          .ReturnsAsync(new RepositoryResult<GetTrustsModel>
                          {
                              Result = null
                          });

            var mapperMock = new Mock<IMapper<GetTrustsD365Model, GetTrustsModel>>();
            var errorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var trustsController = new TrustsController(trustsRepoMock.Object,
                                                        _academiesRepository.Object,
                                                        errorHandlerMock.Object);


            //Execute
            var result = await trustsController.GetById(trustId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should not be called when repo returns valid result
            errorHandlerMock.Verify(h => h.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Mapper not to be called when repo returns null but valid result
            mapperMock.Verify(m => m.Map(It.IsAny<GetTrustsD365Model>()), Times.Never);

            //Final result should be a 404 - Not Found with the correct message
            Assert.Equal("The trust with the id: 'a16e9020-9123-4420-8055-851d1b672fa9' was not found", castedResult.Value);
            Assert.Equal(404, castedResult.StatusCode);
        }

        [Fact]
        public async void GetById_TrustFound_Returns200OkResult()
        {
            //Arrange
            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");

            //Set up mock repository to return a valid and set result
            var trustsRepoMock = new Mock<ITrustsRepository>(); 
            trustsRepoMock.Setup(m => m.GetTrustById(trustId))
                          .ReturnsAsync(new RepositoryResult<GetTrustsModel>
                          {
                              Result = new GetTrustsModel
                              {
                                  Id = trustId
                              }
                          });

            var errorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var trustsController = new TrustsController(trustsRepoMock.Object,
                                                        _academiesRepository.Object,
                                                        errorHandlerMock.Object);


            //Execute
            var result = await trustsController.GetById(trustId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should not be called when repo returns valid result
            errorHandlerMock.Verify(h => h.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Final result should be a 200 OK with the result of the mapping operation
            Assert.Equal(trustId, ((GetTrustsModel)castedResult.Value).Id);
            Assert.Equal(200, castedResult.StatusCode);
        }

        #endregion

        #region Search Projects Tests

        [Fact]
        public async void SearchTrusts_InvalidRepoResult()
        {
            var searchTerm = "Some search term";

            //Set up mock repository to return a failed D365 call
            var trustsRepoMock = new Mock<ITrustsRepository>();
            trustsRepoMock.Setup(m => m.SearchTrusts(searchTerm))
                           .ReturnsAsync(new RepositoryResult<List<GetTrustsModel>>
                           {
                               Error = new RepositoryResultBase.RepositoryError
                               {
                                   StatusCode = HttpStatusCode.BadRequest,
                                   ErrorMessage = "Bad Request"
                               }
                           });

            var mapperMock = new Mock<IMapper<GetTrustsD365Model, GetTrustsModel>>();

            //Set up error handler to return a random result
            var errorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            errorHandlerMock.Setup(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()))
                        .Returns(new ObjectResult("Some error message")
                        {
                            StatusCode = 499
                        });

            var trustsController = new TrustsController(trustsRepoMock.Object,
                                                        _academiesRepository.Object,
                                                        errorHandlerMock.Object);

            //Execute
            var result = await trustsController.SearchTrusts(searchTerm);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should be called with repository result
            errorHandlerMock.Verify(h => h.LogAndCreateResponse(
                It.Is<RepositoryResultBase>(r =>
                    r.Error.ErrorMessage == "Bad Request" &&
                    r.Error.StatusCode == HttpStatusCode.BadRequest)),
                Times.Once);
            //Mapper not to be called when repo fails
            mapperMock.Verify(m => m.Map(It.IsAny<GetTrustsD365Model>()), Times.Never);

            //Final result should be what the error handler returns
            Assert.Equal("Some error message", castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public async void SearchTrusts_EmptyButValidSearchResult()
        {
            var searchTerm = "Some search term";

            //Set up mock repository to return a failed D365 call
            var trustsRepoMock = new Mock<ITrustsRepository>();
            trustsRepoMock.Setup(m => m.SearchTrusts(searchTerm))
                           .ReturnsAsync(new RepositoryResult<List<GetTrustsModel>>
                           {
                               Result = null
                           });

            var mapperMock = new Mock<IMapper<GetTrustsD365Model, GetTrustsModel>>();
            var errorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var trustsController = new TrustsController(trustsRepoMock.Object,
                                                        _academiesRepository.Object,
                                                        errorHandlerMock.Object);

            //Execute
            var result = await trustsController.SearchTrusts(searchTerm);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should not be called when repo returns valid result
            errorHandlerMock.Verify(h => h.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Mapper not to be called becasue there are no results
            mapperMock.Verify(m => m.Map(It.IsAny<GetTrustsD365Model>()), Times.Never);

            //Final result should be a 200 OK wrapped around a null list
            Assert.Null((List<GetTrustsModel>)castedResult.Value);
            Assert.Equal(200, castedResult.StatusCode);
        }

        [Fact]
        public async void SearchTrusts_ThreeResults()
        {
            var searchTerm = "Some search term";

            //Set up mock repository to return a failed D365 call
            var trustsRepoMock = new Mock<ITrustsRepository>();
            trustsRepoMock.Setup(m => m.SearchTrusts(searchTerm))
                           .ReturnsAsync(new RepositoryResult<List<GetTrustsModel>>
                           {
                               Result = new List<GetTrustsModel>
                               {
                                   new GetTrustsModel { TrustName = "Trust 001" },
                                   new GetTrustsModel { TrustName = "Trust 002" },
                                   new GetTrustsModel { TrustName = "Trust 003" }
                               }
                           });

            var errorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var trustsController = new TrustsController(trustsRepoMock.Object,
                                                        _academiesRepository.Object,
                                                        errorHandlerMock.Object);

            //Execute
            var result = await trustsController.SearchTrusts(searchTerm);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should not be called when repo returns valid result
            errorHandlerMock.Verify(h => h.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);

            //Final result should be a 200OK wrapped around a a list with three results
            Assert.Equal(3, ((List<GetTrustsModel>)castedResult.Value).Count);
            Assert.Equal("Trust 001", ((List<GetTrustsModel>)castedResult.Value)[0].TrustName);
            Assert.Equal("Trust 002", ((List<GetTrustsModel>)castedResult.Value)[1].TrustName);
            Assert.Equal("Trust 003", ((List<GetTrustsModel>)castedResult.Value)[2].TrustName);
            Assert.Equal(200, castedResult.StatusCode);
        }

        #endregion

        #region Get Trust Academies Tests

        [Fact]
        public async void GetTrustAcademies_TrustRepoFailure()
        {
            //Arrange
            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");

            //Set up trusts repo to return a failure
            var trustsRepoMock = new Mock<ITrustsRepository>();
            trustsRepoMock.Setup(m => m.GetTrustById(trustId))
                          .ReturnsAsync(new RepositoryResult<GetTrustsModel>
                          {
                              Error = new RepositoryResultBase.RepositoryError
                              {
                                  StatusCode = HttpStatusCode.BadRequest,
                                  ErrorMessage = "Bad Request"
                              }
                          });

            var mapperMock = new Mock<IMapper<GetTrustsD365Model, GetTrustsModel>>();

            //Set up error handler to return a random result
            var errorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            errorHandlerMock.Setup(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()))
                            .Returns(new ObjectResult("Some error message")
                            {
                                StatusCode = 499
                            });

            var trustsController = new TrustsController(trustsRepoMock.Object,
                                                        _academiesRepository.Object,
                                                        errorHandlerMock.Object);


            //Execute
            var result = await trustsController.GetTrustAcademies(trustId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should be called with result from repository
            errorHandlerMock.Verify(h => h.LogAndCreateResponse(
                It.Is<RepositoryResultBase>(r =>
                    r.Error.ErrorMessage == "Bad Request" &&
                    r.Error.StatusCode == HttpStatusCode.BadRequest)),
                Times.Once);
            //Mapper not to be called when repo fails
            mapperMock.Verify(m => m.Map(It.IsAny<GetTrustsD365Model>()), Times.Never);

            //Final result should be what the error handler returns
            Assert.Equal("Some error message", castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public async void GetTrustAcademies_TrustNotFound()
        {
            //Arrange
            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");

            //Set up mock trust repository to return a null result (not found)
            var trustsRepoMock = new Mock<ITrustsRepository>();
            trustsRepoMock.Setup(m => m.GetTrustById(trustId))
                          .ReturnsAsync(new RepositoryResult<GetTrustsModel>
                          {
                              Result = null
                          });

            var mapperMock = new Mock<IMapper<GetTrustsD365Model, GetTrustsModel>>();
            var errorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var trustsController = new TrustsController(trustsRepoMock.Object,
                                                        _academiesRepository.Object,
                                                        errorHandlerMock.Object);


            //Execute
            var result = await trustsController.GetTrustAcademies(trustId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should not be called when repo returns valid result
            errorHandlerMock.Verify(h => h.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Mapper not to be called when repo returns null but valid result
            mapperMock.Verify(m => m.Map(It.IsAny<GetTrustsD365Model>()), Times.Never);

            //Final result should be a 404 - Not Found with the correct message
            Assert.Equal("The trust with the id: 'a16e9020-9123-4420-8055-851d1b672fa9' was not found", castedResult.Value);
            Assert.Equal(404, castedResult.StatusCode);
        }

        [Fact]
        public async void GetTrustAcademies_TrustFound_FailedAcademyRepo()
        {
            //Arrange
            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");

            //Set up mock trust repository to return a valid and set result
            var trustsRepoMock = new Mock<ITrustsRepository>();
            trustsRepoMock.Setup(m => m.GetTrustById(trustId))
                          .ReturnsAsync(new RepositoryResult<GetTrustsModel>
                          {
                              Result = new GetTrustsModel
                              {
                                  Id = trustId
                              }
                          });

            //Set up mock academies repo to return a failed result
            var academiesRepoMock = new Mock<IAcademiesRepository>();
            academiesRepoMock.Setup(m => m.GetAcademiesByTrustId(trustId))
                             .ReturnsAsync(new RepositoryResult<List<GetAcademiesModel>>
                             {
                                 Error = new RepositoryResultBase.RepositoryError
                                 {
                                     StatusCode = HttpStatusCode.BadRequest,
                                     ErrorMessage = "Bad Request"
                                 }
                             });

            var mapperMock = new Mock<IMapper<GetTrustsD365Model, GetTrustsModel>>();

            //Set up mapper to return a slim result when called with the expected input
            mapperMock.Setup(m => m.Map(It.Is<GetTrustsD365Model>(p => p.Id == trustId)))
                      .Returns(new GetTrustsModel
                      {
                          Id = trustId,
                          Address = "Some address"
                      });

            //Set up error handler to return a random result
            var errorHandlerMock = new Mock<IRepositoryErrorResultHandler>();
            errorHandlerMock.Setup(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()))
                            .Returns(new ObjectResult("Some error message")
                            {
                                StatusCode = 499
                            });

            var trustsController = new TrustsController(trustsRepoMock.Object,
                                                        academiesRepoMock.Object,
                                                        errorHandlerMock.Object);


            //Execute
            var result = await trustsController.GetTrustAcademies(trustId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should be called when a repository fails
            errorHandlerMock.Verify(h => h.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Once);
            //Final result should be what the error handler returns
            Assert.Equal("Some error message", castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public async void GetTrustAcademies_TrustFound_NoAcademiesFound()
        {
            //Arrange
            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");

            //Set up mock trust repository to return a valid and set result
            var trustsRepoMock = new Mock<ITrustsRepository>();
            trustsRepoMock.Setup(m => m.GetTrustById(trustId))
                          .ReturnsAsync(new RepositoryResult<GetTrustsModel>
                          {
                              Result = new GetTrustsModel
                              {
                                  Id = trustId
                              }
                          });

            //Set up mock academies repo to return a failed result
            var academiesRepoMock = new Mock<IAcademiesRepository>();
            academiesRepoMock.Setup(m => m.GetAcademiesByTrustId(trustId))
                             .ReturnsAsync(new RepositoryResult<List<GetAcademiesModel>>
                             {
                                 Result = null
                             });

            var getAcademiesMapper = new Mock<IMapper<GetAcademiesD365Model, GetAcademiesModel>>();
            var errorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var trustsController = new TrustsController(trustsRepoMock.Object,
                                                        academiesRepoMock.Object,
                                                        errorHandlerMock.Object);


            //Execute
            var result = await trustsController.GetTrustAcademies(trustId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should be called when a repository fails
            errorHandlerMock.Verify(h => h.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);

            //GetAcademiesMapper should not be called when no academies are found
            getAcademiesMapper.Verify(m => m.Map(It.IsAny<GetAcademiesD365Model>()), Times.Never);
            //Final result should be a 200 wrapped around a null list
            Assert.Null((List<GetAcademiesModel>)castedResult.Value);
            Assert.Equal(200, castedResult.StatusCode);
        }

        [Fact]
        public async void GetTrustAcademies_TrustFound_TwoAcademiesFound()
        {
            //Arrange
            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");

            //Set up mock trust repository to return a valid and set result
            var trustsRepoMock = new Mock<ITrustsRepository>();
            trustsRepoMock.Setup(m => m.GetTrustById(trustId))
                          .ReturnsAsync(new RepositoryResult<GetTrustsModel>
                          {
                              Result = new GetTrustsModel
                              {
                                  Id = trustId
                              }
                          });

            //Set up mock academies repo to return a failed result
            var academiesRepoMock = new Mock<IAcademiesRepository>();
            academiesRepoMock.Setup(m => m.GetAcademiesByTrustId(trustId))
                             .ReturnsAsync(new RepositoryResult<List<GetAcademiesModel>>
                             {
                                 Result = new List<GetAcademiesModel>
                                 {
                                     new GetAcademiesModel { AcademyName = "Academy 001" },
                                     new GetAcademiesModel { AcademyName = "Academy 002" }
                                 }
                             });

            var errorHandlerMock = new Mock<IRepositoryErrorResultHandler>();

            var trustsController = new TrustsController(trustsRepoMock.Object,
                                                        academiesRepoMock.Object,
                                                        errorHandlerMock.Object);


            //Execute
            var result = await trustsController.GetTrustAcademies(trustId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should be called when a repository fails
            errorHandlerMock.Verify(h => h.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);

            //Final result should be a 200 wrapped around a null list
            Assert.Equal(2, ((List<GetAcademiesModel>)castedResult.Value).Count);
            Assert.Equal("Academy 001", ((List<GetAcademiesModel>)castedResult.Value)[0].AcademyName);
            Assert.Equal("Academy 002", ((List<GetAcademiesModel>)castedResult.Value)[1].AcademyName);
            Assert.Equal(200, castedResult.StatusCode);
        }

        #endregion 
    }
}
