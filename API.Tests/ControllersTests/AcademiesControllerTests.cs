using API.Controllers;
using API.Mapping;
using API.Models.D365;
using API.Models.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using Xunit;

namespace API.Tests.ControllersTests
{
    public class AcademiesControllerTests
    {
        #region GetAcademyById Tests

        [Fact]
        public async void GetAcademyById_InvalidRepoResult_CallsRepositoryErrorHandler()
        {
            //Arrange
            var academyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");
            var academyRepoMock = new Mock<IAcademiesRepository>();

            //Set up mock repository to return a failure
            academyRepoMock.Setup(m => m.GetAcademyById(It.IsAny<Guid>()))
                           .ReturnsAsync(new RepositoryResult<GetAcademiesD365Model>
                           {
                               Error = new RepositoryResultBase.RepositoryError
                               {
                                   StatusCode = HttpStatusCode.BadRequest,
                                   ErrorMessage = "Bad Request"
                               }
                           });

            var mapperMock = new Mock<IMapper<GetAcademiesD365Model, GetAcademiesModel>>();

            var errorHandler = new Mock<IRepositoryErrorResultHandler>();

            //Set up error handler to return a random result
            errorHandler.Setup(r => r.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()))
                        .Returns(new ObjectResult("Some error message")
                        {
                            StatusCode = 499
                        });

            var academiesController = new AcademiesController(academyRepoMock.Object,
                                                              mapperMock.Object,
                                                              errorHandler.Object);

            //Execute
            var result = await academiesController.GetAcademyById(academyId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should be called with result from repository
            errorHandler.Verify(h => h.LogAndCreateResponse(
                It.Is<RepositoryResultBase>(r => 
                    r.Error.ErrorMessage == "Bad Request" && 
                    r.Error.StatusCode == HttpStatusCode.BadRequest)), 
                Times.Once);
            //Mapper not to be called when repo fails
            mapperMock.Verify(m => m.Map(It.IsAny<GetAcademiesD365Model>()), Times.Never);

            //Final result should be what the error handler returns
            Assert.Equal("Some error message", castedResult.Value);
            Assert.Equal(499, castedResult.StatusCode);
        }

        [Fact]
        public async void GetAcademyById_NotFound_ReturnsNotFoundResult()
        {
            //Arrange
            var academyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");
            var academyRepoMock = new Mock<IAcademiesRepository>();

            //Set up mock repository to return a null result (not found)
            academyRepoMock.Setup(m => m.GetAcademyById(It.IsAny<Guid>()))
                           .ReturnsAsync(new RepositoryResult<GetAcademiesD365Model>
                           {
                               Result = null
                           });

            var mapperMock = new Mock<IMapper<GetAcademiesD365Model, GetAcademiesModel>>();
            var errorHandler = new Mock<IRepositoryErrorResultHandler>();

            var academiesController = new AcademiesController(academyRepoMock.Object,
                                                              mapperMock.Object,
                                                              errorHandler.Object);

            //Execute
            var result = await academiesController.GetAcademyById(academyId);
            var castedResult = (ObjectResult)result.Result;

            //Assert

            //Error handler should not be called when repo returns valid result
            errorHandler.Verify(h => h.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Mapper not to be called when repo returns null but valid result
            mapperMock.Verify(m => m.Map(It.IsAny<GetAcademiesD365Model>()), Times.Never);

            //Final result should be a 404 - Not Found with the correct message
            Assert.Equal("The academy with the id: 'a16e9020-9123-4420-8055-851d1b672fa9' was not found", castedResult.Value);
            Assert.Equal(404, castedResult.StatusCode);
        }

        [Fact]
        public async void GetAcademyById_AcademyFound_Returns200OkResult()
        {
            //Arrange
            var referenceAcademyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");
            var academyRepoMock = new Mock<IAcademiesRepository>();

            //Set up mock repository to return a valid and set result
            academyRepoMock.Setup(m => m.GetAcademyById(referenceAcademyId))
                           .ReturnsAsync(new RepositoryResult<GetAcademiesD365Model>
                           {
                               Result = new GetAcademiesD365Model
                               {
                                   Id = referenceAcademyId
                               }
                           });

            var mapperMock = new Mock<IMapper<GetAcademiesD365Model, GetAcademiesModel>>();

            //Setup mapper to return a slim result when called with the expected input
            mapperMock.Setup(m => m.Map(It.Is<GetAcademiesD365Model>(p => p.Id == referenceAcademyId)))
                      .Returns(new GetAcademiesModel
                      {
                          Id = referenceAcademyId,
                          Address = "Some address"
                      });

            var errorHandler = new Mock<IRepositoryErrorResultHandler>();

            var academiesController = new AcademiesController(academyRepoMock.Object,
                                                              mapperMock.Object,
                                                              errorHandler.Object);

            //Execute
            var result = await academiesController.GetAcademyById(referenceAcademyId);
            var castedResult = (ObjectResult)result.Result;

            //Error handler should not be called when repo returns valid result
            errorHandler.Verify(h => h.LogAndCreateResponse(It.IsAny<RepositoryResultBase>()), Times.Never);
            //Mapper to be called once with the expected input
            mapperMock.Verify(m => m.Map(It.Is<GetAcademiesD365Model>(p => p.Id == referenceAcademyId)), 
                              Times.Once);

            //Final result should be a 200 OK with the result of the mapping operation
            Assert.Equal(referenceAcademyId, ((GetAcademiesModel)castedResult.Value).Id);
            Assert.Equal("Some address", ((GetAcademiesModel)castedResult.Value).Address);
            Assert.Equal(200, castedResult.StatusCode);
        }

        #endregion
    }
}
