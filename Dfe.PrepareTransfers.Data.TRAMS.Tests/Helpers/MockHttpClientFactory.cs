namespace Dfe.PrepareTransfers.Data.TRAMS.Tests.Helpers;
using RichardSzalay.MockHttp;
using System;
using System.Net.Http;

public class MockHttpClientFactory : IHttpClientFactory
{
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;

    public MockHttpClientFactory(MockHttpMessageHandler mockHttpMessageHandler)
    {
        _mockHttpMessageHandler = mockHttpMessageHandler;
    }

    public HttpClient CreateClient(string name)
    {
        HttpClient client = _mockHttpMessageHandler.ToHttpClient();
        client.BaseAddress = new Uri("https://localhost/");
        return client;
    }
}