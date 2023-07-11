using Dfe.PrepareTransfers.Data.Models;
using FluentAssertions;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.TRAMS.ExtensionMethods;
using Xunit;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace Dfe.PrepareTransfers.Web.Integration.Tests.Pages.Projects.LegalRequirements
{
    public class DiocesanConsentTests : BaseIntegrationTests
    {
        private readonly IntegrationTestingWebApplicationFactory _factory;
        public DiocesanConsentTests(IntegrationTestingWebApplicationFactory factory) : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Should_successfully_load_radio_buttons()
        {
            var project = GetProject(p => p.LegalRequirements.DiocesanConsent = ThreeOptions.No.ToDescription());

            await OpenUrlAsync($"/project/{project.ProjectUrn}/legalrequirements/diocesan-consent");

            Document.QuerySelector<IHtmlElement>("[id=No]").IsChecked().Should().BeTrue();
            Document.BaseUri.Should()
                .EndWith($"/project/{project.ProjectUrn}/legalrequirements/diocesan-consent");
        }

        [Fact]
        public async Task Should_save_selection()
        {
            var project = GetProject(p => p.LegalRequirements.DiocesanConsent = ThreeOptions.No.ToDescription());
            project.LegalRequirements.DiocesanConsent = ThreeOptions.No.ToDescription();

            _factory.AddAnyPatch($"/academyTransferProject/{project.ProjectUrn}", project);

            await OpenUrlAsync($"/project/{project.ProjectUrn}/legalrequirements/diocesan-consent");

            Document.QuerySelector<IHtmlElement>("[id=No]").IsChecked().Should().BeTrue();

            await Document.QuerySelector<IHtmlButtonElement>("[data-test=submit-btn]").SubmitAsync();

            Document.QuerySelector<IHtmlElement>("h1").Text().Trim().Should()
                .Be("Legal requirements");
            Document.QuerySelector<IHtmlElement>("[data-test=diocesan-consent]").Text().Trim().Should().
                Be("No");
        }
    }
}
