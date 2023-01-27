using System.Net;
using System.Net.Http;
using Xunit;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests
{
    public class TramsApiExceptionTests
    {
        [Fact]
        public void GivenHttpResponse_ShouldSetExceptionPropertiesCorrectly()
        {
            // Arrange
            const HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            var httpResponse = new HttpResponseMessage(statusCode) { Content = new StringContent("content") };

            // Act
            var apiException = new TramsApiException(httpResponse);

            // Assert
            Assert.Equal(statusCode, apiException.StatusCode);
            Assert.Equal($"API encountered an error-({statusCode.ToString()})", apiException.Message);
            Assert.Equal("content", apiException.Data["Content"]);
            Assert.Equal(statusCode, apiException.Data["Sentry:Tag:StatusCode"]);
        }
    }
}