using Dfe.Academisation.ExtensionMethods;
using FluentAssertions;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Integration.Tests
{
    public class BasicTests : BaseIntegrationTests
    {
        public BasicTests(IntegrationTestingWebApplicationFactory factory) : base(factory) { }

        [Theory]
        [InlineData("/home")]
        public async Task Should_be_success_result_on_get(string url)
        {
            var projects = SetUpMockGetProjectCalls();

            await OpenAndConfirmPathAsync(url);

            Document.StatusCode.Should().Be(HttpStatusCode.OK);
            Document.ContentType.Should().Be("text/html");
            Document.CharacterSet.Should().Be("utf-8");
            Document.QuerySelector("h1.govuk-heading-xl").TextContent.Trim().Should().Be("Transfer projects");
            Document.QuerySelector("[data-cy=select-projectlist-filter-count]").TextContent.Trim().Should().Be("1 projects found");
            Document.QuerySelector("[data-id=project-link-001]").TextContent
              .Trim().Should().Be(projects.First().TransferringAcademies[0].IncomingTrustName.ToTitleCase());
        }
    }
}