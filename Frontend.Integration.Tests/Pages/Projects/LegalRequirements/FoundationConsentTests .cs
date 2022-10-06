using Data.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.TRAMS.ExtensionMethods;
using Xunit;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace Frontend.Integration.Tests.Pages.Projects.LegalRequirements
{
    public class FoundationConsentTests : BaseIntegrationTests
    {
        public FoundationConsentTests(IntegrationTestingWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_successfully_load_radio_buttons()
        {
            var project = GetProject(p => p.LegalRequirements.FoundationConsent = ThreeOptions.No.ToDescription());

            await OpenUrlAsync($"/project/{project.ProjectUrn}/legalrequirements/foundation-consent");


            Document.QuerySelector<IHtmlElement>("[id=No]").IsChecked().Should().BeTrue();
            Document.BaseUri.Should()
                .EndWith($"/project/{project.ProjectUrn}/legalrequirements/foundation-consent");
        }
    }
}
