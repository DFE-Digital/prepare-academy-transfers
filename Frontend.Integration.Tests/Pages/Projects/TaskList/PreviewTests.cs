﻿using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Data.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.TRAMS.ExtensionMethods;
using Xunit;

namespace Frontend.Integration.Tests.Pages.Projects.TaskList
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
                p.LegalRequirements.TrustAgreement = ThreeOptions.No.ToDescription();
                p.LegalRequirements.FoundationConsent = ThreeOptions.No.ToDescription();
            });
            
            await OpenUrlAsync($"/project/{project.ProjectUrn}/advisory-board/preview?");
            Document.QuerySelector<IHtmlParagraphElement>("[data-test=diocesan-consent]").Text().Should().Be("No"); 
            Document.QuerySelector<IHtmlElement>("[data-test=foundation-consent]").Text().Should().Be("No");
            Document.QuerySelector<IHtmlElement>("[data-test=trust-agreement]").Text().Should().Be("No");
        }
    }
}