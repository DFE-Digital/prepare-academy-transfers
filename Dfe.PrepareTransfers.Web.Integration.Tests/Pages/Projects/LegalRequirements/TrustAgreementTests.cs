using Dfe.PrepareTransfers.Data.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.TRAMS.ExtensionMethods;
using Xunit;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace Dfe.PrepareTransfers.Web.Integration.Tests.Pages.Projects.LegalRequirements
{
    public class TrustAgreementIntegrationTests : BaseIntegrationTests
    {
        private readonly IntegrationTestingWebApplicationFactory _factory;
        public TrustAgreementIntegrationTests(IntegrationTestingWebApplicationFactory factory) : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Should_successfully_load_radio_buttons()
        {
            var project = GetProject(p => p.LegalRequirements.IncomingTrustAgreement = ThreeOptions.No.ToDescription());

            await OpenUrlAsync($"/project/{project.ProjectUrn}/legalrequirements/incoming-trust-agreement");

            Document.QuerySelector<IHtmlElement>("[id=No]").IsChecked().Should().BeTrue();
            Document.BaseUri.Should()
                .EndWith($"/project/{project.ProjectUrn}/legalrequirements/incoming-trust-agreement");
        }

        [Fact]
        public async Task Should_save_selection()
        {
            var project = GetProject(p => p.LegalRequirements.IncomingTrustAgreement = ThreeOptions.No.ToDescription());
            project.LegalRequirements.IncomingTrustAgreement = ThreeOptions.No.ToDescription();

            _factory.AddAnyPatch($"/academyTransferProject/{project.ProjectUrn}", project);

            await OpenUrlAsync($"/project/{project.ProjectUrn}/legalrequirements/incoming-trust-agreement");

            Document.QuerySelector<IHtmlElement>("[id=No]").IsChecked().Should().BeTrue();

            await Document.QuerySelector<IHtmlButtonElement>("[data-test=submit-btn]").SubmitAsync();

            Document.QuerySelector<IHtmlElement>("h1").Text().Trim().Should()
                .Be("Legal requirements");
            Document.QuerySelector<IHtmlElement>("[data-test=incoming-trust-agreement]").Text().Trim().Should().
                Be("No");
        }
    }
}
