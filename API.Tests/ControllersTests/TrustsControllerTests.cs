using API.Controllers;
using API.Mapping;
using API.Models.D365;
using API.Models.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace API.Tests.ControllersTests
{
    public class TrustsControllerTests
    {
        //These defaults mock are passed to the cosntructor in tests for methods that don't use them
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
                           .ReturnsAsync(new RepositoryResult<GetTrustsD365Model>
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
                                                        mapperMock.Object,
                                                        _getAcademiesMapper.Object,
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

        #endregion
    }
}
