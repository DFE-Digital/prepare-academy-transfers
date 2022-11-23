using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using AngleSharp.Io.Network;
using Xunit;

namespace Frontend.Integration.Tests
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
            return await _browsingContext.OpenAsync($"http://localhost{url}");
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

        private IBrowsingContext CreateBrowsingContext(HttpClient httpClient)
        {
            var config = AngleSharp.Configuration.Default
                .WithRequester(new HttpClientRequester(httpClient))
                .WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = true });

            return BrowsingContext.New(config);
        }

        public IDocument Document => _browsingContext.Active;

        public void Dispose()
        {
            _factory.Reset();
        }
    }
}