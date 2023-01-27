using System.Net;
using System.Net.Http;
using Moq;
using Newtonsoft.Json;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests.Dfe.PrepareTransfers.Helpers
{
    public static class HttpClientTestHelpers
    {
        public static void SetupGet<T>(Mock<ITramsHttpClient> client, T responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(responseContent)),
                StatusCode = statusCode
            });
        }
    }
}