using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Frontend.Integration.Tests.Pages.Projects.BenefitsAndRisks
{
    public class EqualitiesImpactAssessmentIntegrationTests : BaseIntegrationTests
    {
        private readonly IntegrationTestingWebApplicationFactory _factory;

        public EqualitiesImpactAssessmentIntegrationTests(IntegrationTestingWebApplicationFactory factory) : base(factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData(true, "Yes")]
        [InlineData(false, "No")]
        public async Task Should_save_selection(bool trueFalse, string yesNo)
        {
            var project = GetProject(p => p.Benefits.EqualitiesImpactAssessmentConsidered = trueFalse);
            project.Benefits.EqualitiesImpactAssessmentConsidered = trueFalse;

            _factory.AddAnyPatch($"/academyTransferProject/{project.ProjectUrn}", project);

            await OpenUrlAsync($"/project/{project.ProjectUrn}/benefits/equalities-impact-assessment");

            Document.QuerySelector<IHtmlInputElement>($"[data-test={trueFalse.ToString().ToLower()}]").IsChecked = true;

            await Document.QuerySelector<IHtmlButtonElement>("[data-test=submit-btn]").SubmitAsync();

            Document.QuerySelector<IHtmlElement>("h1").Text().Trim().Should()
                .Be("Benefits and risks");
            Document.QuerySelector<IHtmlElement>("[data-test=equalities-impact-assessment]").Text().Trim().Should().
                Be(yesNo);
        }

        [Fact]
        public async Task Should_show_error_when_nothing_selected()
        {
            var project = GetProject(p => p.Benefits.EqualitiesImpactAssessmentConsidered = null);

            _factory.AddAnyPatch("/academyTransferProject/001", project);

            await OpenUrlAsync($"/project/{project.ProjectUrn}/benefits/equalities-impact-assessment");

            await Document.QuerySelector<IHtmlButtonElement>("[data-test=submit-btn]").SubmitAsync();

            Document.BaseUri.Should()
                .EndWith($"/project/{project.ProjectUrn}/benefits/equalities-impact-assessment");
            Document.QuerySelector<IHtmlAnchorElement>("[data-qa=error_text]").Text().Trim().Should()
                .Be("Select yes if an Equalities Impact Assessment has been considered");
        }
    }
}