using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Dfe.PrepareTransfers.Data.Models;
using FluentAssertions;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.TRAMS.ExtensionMethods;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Integration.Tests.Pages.Projects.TaskList
{
    public class PreviewTests : BaseIntegrationTests
    {
        private readonly IntegrationTestingWebApplicationFactory _factory;
        public PreviewTests(IntegrationTestingWebApplicationFactory factory) : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Should_successfully_load_legal_requirements()
        {
            var project = GetProject(p =>
            {
                p.LegalRequirements.DiocesanConsent = ThreeOptions.No.ToDescription();
                p.LegalRequirements.IncomingTrustAgreement = ThreeOptions.No.ToDescription();
                p.LegalRequirements.OutgoingTrustConsent = ThreeOptions.No.ToDescription();
            });
            
            await OpenUrlAsync($"/project/{project.ProjectUrn}/advisory-board/preview?");
            Document.QuerySelector<IHtmlParagraphElement>("[data-test=diocesan-consent]").Text().Should().Be("No");
            Document.QuerySelector<IHtmlElement>("[data-test=incoming-trust-agreement]").Text().Should().Be("No");
            Document.QuerySelector<IHtmlElement>("[data-test=outgoing-trust-consent]").Text().Should().Be("No");
        }
    }
}
