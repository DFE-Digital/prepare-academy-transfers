using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Frontend.Integration.Tests
{
    public class IntegrationTestingWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private static int _currentPort = 5080;
        private static readonly object _sync = new object();

        private readonly WireMockServer _server;
        private readonly int _port;

        public IntegrationTestingWebApplicationFactory()
        {
            _port = AllocateNext();
            _server = WireMockServer.Start(_port);
            
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                var projectDir = Directory.GetCurrentDirectory();
                var configPath = Path.Combine(projectDir, "appsettings.json");

                config.Sources.Clear();
                config
                    .AddJsonFile(configPath)
                    .AddInMemoryCollection(new Dictionary<string, string>
                        { { "TRAMS_API_BASE", $"http://localhost:{_port}" } })
                    .AddEnvironmentVariables();
            });

            builder.ConfigureServices(services =>
            {
                 services.AddAuthentication("Test");
                 services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();
            });
        }

        public class MockSchemeProvider : AuthenticationSchemeProvider
        {
            public MockSchemeProvider(IOptions<AuthenticationOptions> options)
                : base(options)
            {
            }

            protected MockSchemeProvider(
                IOptions<AuthenticationOptions> options,
                IDictionary<string, AuthenticationScheme> schemes
            )
                : base(options, schemes)
            {
            }

            public override Task<AuthenticationScheme> GetSchemeAsync(string name)
            {
                if (name == "Test")
                {
                    var scheme = new AuthenticationScheme(
                        "Test",
                        "Test",
                        typeof(MockAuthenticationHandler)
                    );
                    return Task.FromResult(scheme);
                }

                return base.GetSchemeAsync(name);
            }
        }

        public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public MockAuthenticationHandler(
                IOptionsMonitor<AuthenticationSchemeOptions> options,
                ILoggerFactory logger,
                UrlEncoder encoder,
                ISystemClock clock
            )
                : base(options, logger, encoder, clock)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, "Name") };
                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "Test");

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
        }


        public void AddGetWithJsonResponse<TResponseBody>(string path, TResponseBody responseBody)
        {
            _server
                .Given(Request.Create()
                    .WithPath(path)
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonConvert.SerializeObject(responseBody)));
        }


        public void AddPatchWithJsonRequest<TRequestBody, TResponseBody>(string path, TRequestBody requestBody,
            TResponseBody responseBody)
        {
            _server
                .Given(Request.Create()
                    .WithPath(path)
                    .WithBody(new JsonMatcher(JsonConvert.SerializeObject(requestBody), true))
                    .UsingPatch())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonConvert.SerializeObject(responseBody)));
        }

        public void AddPostWithJsonRequest<TRequestBody, TResponseBody>(string path, TRequestBody requestBody,
            TResponseBody responseBody)
        {
            _server
                .Given(Request.Create()
                    .WithPath(path)
                    .WithBody(new JsonMatcher(JsonConvert.SerializeObject(requestBody), true))
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonConvert.SerializeObject(responseBody)));
        }

        public void AddErrorResponse(string path, string method)
        {
            _server
                .Given(Request.Create()
                    .WithPath(path)
                    .UsingMethod(method))
                .RespondWith(Response.Create()
                    .WithStatusCode(500));
        }

        public void Reset()
        {
            _server.Reset();
        }

        private static int AllocateNext()
        {
            lock (_sync)
            {
                var next = _currentPort;
                _currentPort++;
                return next;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _server.Stop();
            }
        }
    }
}