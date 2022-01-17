using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Frontend.Integration.Tests
{
    public class BasicTests : BaseIntegrationTests
    {
        public BasicTests(IntegrationTestingWebApplicationFactory factory) : base(factory) { }

        [Theory]
        [InlineData("/")]
        public async Task Should_be_success_result_on_get(string url)
        {
            var projects = GetProjects();

            await OpenUrlAsync(url);

            Document.StatusCode.Should().Be(HttpStatusCode.OK);
            Document.ContentType.Should().Be("text/html");
            Document.CharacterSet.Should().Be("utf-8");
            Document.QuerySelector("h1.govuk-heading-xl")?.TextContent.Trim().Should().Be("Manage an academy transfer");
            Document.QuerySelector("h2.govuk-heading-l")?.TextContent.Trim().Should().Be("Projects");
            Document.QuerySelector("#main-content > div:nth-child(2) > div > div:nth-child(3) > div > h3 > a")
                ?.TextContent
                .Trim().Should().Be(projects.First().TransferringAcademies[0].OutgoingAcademy.Name);
            Document.QuerySelector("a.dfe-sign-out")?.TextContent.Trim().Should().Be("Sign out");
        }
    }
}