using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Integration.Tests.Pages.ProjectType
{
    public class ProjectTypeIntegrationTests : BaseIntegrationTests
    {
        public ProjectTypeIntegrationTests(IntegrationTestingWebApplicationFactory factory) : base(factory) { }


        [Fact]
        public async Task Should_display_labels_for_radio_with_correct_value()
        {
            await OpenUrlAsync("/project-type");

            Document.QuerySelector("label[for=conversion-radio]")!.Text().Should().Contain("Manage an academy conversion");
            Document.QuerySelector("label[for=transfer-radio]")!.Text().Should().Contain("Manage an academy transfer");
        }

        [Fact]
        public async Task Should_not_have_any_values_selected_on_page_load()
        {
            await OpenUrlAsync("/project-type");

            Document.QuerySelector<IHtmlInputElement>("#conversion-radio")!.IsChecked.Should().BeFalse();
            Document.QuerySelector<IHtmlInputElement>("#transfer-radio")!.IsChecked.Should().BeFalse();
        }

        [Fact]
        public async Task Should_display_error_and_not_continue_if_no_option_is_selected()
        {
            await OpenUrlAsync("/project-type");

            Document.QuerySelector<IHtmlInputElement>("#conversion-radio")!.IsChecked = false;
            Document.QuerySelector<IHtmlInputElement>("#transfer-radio")!.IsChecked = false;

            await Document.QuerySelector<IHtmlButtonElement>("#submit-btn")!.SubmitAsync();

            Document.QuerySelector("h1")!.TextContent!.Trim().Should().Be("What do you want to do?");

            Document.QuerySelector(".govuk-error-summary")!.Should().NotBeNull();
            Document.QuerySelector(".govuk-error-summary")!.TextContent.Should().Contain("Select a project type");
        }

        [Fact]
        public async Task Should_navigate_to_home_page_if_transfer_is_selected()
        {
            await OpenUrlAsync("/project-type");

            var baseUrlHost = Document.BaseUrl!.Host;

            Document.QuerySelector<IHtmlInputElement>("#transfer-radio")!.IsChecked = true;

            await Document.QuerySelector<IHtmlButtonElement>("#submit-btn")!.SubmitAsync();

            Document.BaseUrl.Host.Should().Be(baseUrlHost);
            Document.Url.Should().EndWith("home");
        }

        [Fact]
        public async Task Should_navigate_to_external_service_if_conversion_is_selected()
        {
            await OpenUrlAsync("/project-type");

            var baseUrlHost = Document.BaseUrl!.Host;

            Document.QuerySelector<IHtmlInputElement>("#conversion-radio")!.IsChecked = true;

            await Document.QuerySelector<IHtmlButtonElement>("#submit-btn")!.SubmitAsync();

            Document.BaseUrl.Host.Should().NotBe(baseUrlHost);
        }
    }
}
