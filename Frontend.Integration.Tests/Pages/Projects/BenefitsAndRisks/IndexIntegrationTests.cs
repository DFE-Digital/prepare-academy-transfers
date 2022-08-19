using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Frontend.Integration.Tests.Pages.Projects.BenefitsAndRisks
{
    public class IndexIntegrationTests : BaseIntegrationTests
    {
        public IndexIntegrationTests(IntegrationTestingWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_link_to_equalities_impact_assesment_form()
        {
            var project = GetProject(p => p.Benefits.EqualitiesImpactAssessmentConsidered = true);
            
            await OpenUrlAsync($"/project/{project.ProjectUrn}/benefits");

            await NavigateAsync("Change", 6);

            Document.BaseUri.Should()
                .EndWith($"/project/{project.ProjectUrn}/benefits/equalities-impact-assessment");            
        }

        [Theory]
        [InlineData(true, "Yes")]
        [InlineData(false, "No")]
        public async Task Should_display_equalities_impact_assesment_value(bool trueFalse, string yesNo)
        {
            var project = GetProject(p => p.Benefits.EqualitiesImpactAssessmentConsidered = trueFalse);

            await OpenUrlAsync($"/project/{project.ProjectUrn}/benefits");            

            Document.QuerySelector<IHtmlElement>("[data-test=equalities-impact-assessment]").Text().Trim().Should().
                Be(yesNo);
        }
    }   
}
