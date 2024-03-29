using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using AngleSharp.Io.Network;
using FluentAssertions;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Integration.Tests
{
    public partial class BaseIntegrationTests : IClassFixture<IntegrationTestingWebApplicationFactory>, IDisposable
    {
        private readonly IntegrationTestingWebApplicationFactory _factory;
        private readonly IBrowsingContext _browsingContext;

        protected BaseIntegrationTests(IntegrationTestingWebApplicationFactory factory)
        {
            _factory = factory;
            var httpClient = factory.CreateClient();
            _browsingContext = CreateBrowsingContext(httpClient);
        }

        protected async Task<IDocument> OpenUrlAsync(string url)
        {
            return await _browsingContext.OpenAsync($"https://localhost{url}");
        }

        protected async Task<IDocument> NavigateAsync(string linkText, int? index = null)
        {
            var anchors = Document.QuerySelectorAll("a");
            var link = (index == null
                    ? anchors.Single(a => a.TextContent.Contains(linkText))
                    : anchors.Where(a => a.TextContent.Contains(linkText)).ElementAt(index.Value))
                as IHtmlAnchorElement;

            return await link.NavigateAsync();
        }

        public async Task NavigateDataTestAsync(string dataTest)
        {
            var anchors = Document.QuerySelectorAll($"[data-test='{dataTest}']").First() as IHtmlAnchorElement;
            await anchors.NavigateAsync();
        }

        private static IBrowsingContext CreateBrowsingContext(HttpClient httpClient)
        {
            var config = Configuration.Default
                .WithRequester(new HttpClientRequester(httpClient))
                .WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = true });

            return BrowsingContext.New(config);
        }

        public IDocument Document => _browsingContext.Active;

        public void Dispose()
        {
            _factory.Reset();
            GC.SuppressFinalize(this);
        }

        private static string BuildRequestAddress(string path)
        {
            return $"https://localhost{(path.StartsWith('/') ? path : $"/{path}")}";
        }

        protected async Task OpenAndConfirmPathAsync(string path, string expectedPath = null, string because = null)
        {
            await _browsingContext.OpenAsync(BuildRequestAddress(path));

            Document.Url.Should().Be(BuildRequestAddress(expectedPath ?? path), because ?? "navigation should be successful");
        }
        
    }
}