using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace API.Tests.RepositoriesTests
{
    public class RepositoryErrorResultHandlerTests
    {
        private readonly RepositoryErrorResultHandler _handler;

        public RepositoryErrorResultHandlerTests()
        {
            _handler = new RepositoryErrorResultHandler();
        }

        [Fact]
        public void LogAndCreateResponse_TooManyRequests_ReturnsExpectedStatusCodeAndMessage()
        {
            var repoResult = new RepositoryResultBase
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = HttpStatusCode.TooManyRequests
                }
            };

            var result = (ObjectResult)_handler.LogAndCreateResponse(repoResult);

            Assert.Equal(429, result.StatusCode);
            Assert.Equal("Too many requests, please try again later", result.Value);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.Continue)]
        [InlineData(HttpStatusCode.BadGateway)]
        public void LogAndCreateResponse_Non429StatusCodes_ReturnExpectedStatusCode(HttpStatusCode inputStatusCode)
        {
            var repoResult = new RepositoryResultBase
            {
                Error = new RepositoryResultBase.RepositoryError
                {
                    StatusCode = inputStatusCode
                }
            };

            var result = (ObjectResult)_handler.LogAndCreateResponse(repoResult);

            Assert.Equal(502, result.StatusCode);
            Assert.Equal("Error communicating with downstream server", result.Value);
        }
    }
}
