using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using FluentAssertions;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.TRAMS.ExtensionMethods;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Integration.Tests.Pages.Projects.LegalRequirements
{
    public class IndexIntegrationTests : BaseIntegrationTests
    {
        public IndexIntegrationTests(IntegrationTestingWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_successfully_load_form()
        {
            var project = GetProject(p => p.LegalRequirements.DiocesanConsent = ThreeOptions.No.ToDescription());

            await OpenUrlAsync($"/project/{project.ProjectUrn}/legalrequirements");

            Document.BaseUri.Should()
                .EndWith($"/project/{project.ProjectUrn}/legalrequirements");
        }

        [Fact]
        public async Task Should_display_diocesan_consent_value()
        {
            var project = GetProject(p => p.LegalRequirements.DiocesanConsent = ThreeOptions.No.ToDescription());

            await OpenUrlAsync($"/project/{project.ProjectUrn}/legalrequirements");

            Document.QuerySelector<IHtmlElement>("[data-test=diocesan-consent]").Text().Trim().Should().
                Be("No");
        }

        [Fact]
        public async Task Should_display_trust_agreement_value()
        {
            var project = GetProject(p => p.LegalRequirements.IncomingTrustAgreement = ThreeOptions.No.ToDescription());

            await OpenUrlAsync($"/project/{project.ProjectUrn}/legalrequirements");

            Document.QuerySelector<IHtmlElement>("[data-test=incoming-trust-agreement]").Text().Trim().Should().
                Be("No");
        }
    }
}
