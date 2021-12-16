using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Frontend.Integration.Tests.Views.Rationale
{
    public class IndexIntegrationTests : BaseIntegrationTests
    {
        private readonly IntegrationTestingWebApplicationFactory _factory;
        
        public IndexIntegrationTests(IntegrationTestingWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenRationale_ShouldBeCompleted()
        {
            var project = GetProject();

            await OpenUrlAsync($"/project/{project.ProjectUrn}");
            
            Document.QuerySelector("#rationale").TextContent.Trim().Should().Be("COMPLETED");
            
            await NavigateAsync("Rationale");
            
            Document.QuerySelector("#main-content > div:nth-child(2) > div > dl > div:nth-child(1) > dd.govuk-summary-list__value > span").TextContent.Should().Be(project.Rationale.ProjectRationale);
            Document.QuerySelector("#main-content > div:nth-child(2) > div > dl > div:nth-child(2) > dd.govuk-summary-list__value > span").TextContent.Should().Be(project.Rationale.TrustSponsorRationale);
        }
        
        [Fact]
        public async Task GivenPartRationale_ShouldBeInProgress()
        {
            var project = GetProject(p => p.Rationale.ProjectRationale = null);

            await OpenUrlAsync($"/project/{project.ProjectUrn}");
            
            Document.QuerySelector("#rationale")!.TextContent.Trim().Should().Be("IN PROGRESS");
            Document.QuerySelector("#rationale")?.ClassName.Should().Contain("blue");
            
            await NavigateAsync("Rationale");
            
            Document.QuerySelector("#main-content > div:nth-child(2) > div > dl > div:nth-child(1) > dd.govuk-summary-list__value > span").TextContent.Should().Be("Empty");
            Document.QuerySelector("#main-content > div:nth-child(2) > div > dl > div:nth-child(2) > dd.govuk-summary-list__value > span").TextContent.Should().Be(project.Rationale.TrustSponsorRationale);
        }
        
        [Fact]
        public async Task GivenNoRationale_ShouldBeNotStarted()
        {
            var project = GetProject(p =>
            {
                p.Rationale.ProjectRationale = null;
                p.Rationale.TrustSponsorRationale = null;
            });

            await OpenUrlAsync($"/project/{project.ProjectUrn}");
            
            Document.QuerySelector("#rationale").TextContent.Trim().Should().Be("NOT STARTED");
            Document.QuerySelector("#rationale").ClassName.Should().Contain("grey");

            await NavigateAsync("Rationale");
            
            Document.QuerySelector("#main-content > div:nth-child(2) > div > dl > div:nth-child(1) > dd.govuk-summary-list__value > span").TextContent.Should().Be("Empty");
            Document.QuerySelector("#main-content > div:nth-child(2) > div > dl > div:nth-child(2) > dd.govuk-summary-list__value > span").TextContent.Should().Be("Empty");
        }
    }
}