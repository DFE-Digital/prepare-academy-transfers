using Dfe.Academisation.CorrelationIdMiddleware;
using Dfe.PrepareTransfers.Data.TRAMS.Tests.Helpers;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Xunit;
using System;
using System.Net.Http;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests
{
    public class TramsHttpClientTests
    {
        [Fact]
        public void CorrelationId_Headers_Should_Be_Added_To_Requests()
        {
            var factory = new FakeClientFactory();

            var correlationContext = new CorrelationContext();
            correlationContext.SetContext(Guid.NewGuid());

            var sut = new TramsHttpClient(factory, correlationContext);

            factory.CreatedHttpClient.DefaultRequestHeaders.Should().ContainSingle(x => x.Key == "x-correlationId");
        }

        private class FakeClientFactory : IHttpClientFactory
        {
            public HttpClient CreatedHttpClient { get; private set; }

            public HttpClient CreateClient(string name)
            {
                IHttpClientFactory innerFactory = new MockHttpClientFactory(new MockHttpMessageHandler());
                CreatedHttpClient = innerFactory.CreateClient();
                return CreatedHttpClient;
            }
        }
    }
}
