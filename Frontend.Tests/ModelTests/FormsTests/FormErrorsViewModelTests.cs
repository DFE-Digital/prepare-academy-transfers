using Frontend.Models.Forms;
using Xunit;

namespace Frontend.Tests.ModelTests.FormsTests
{
    public class FormErrorsViewModelTests
    {
        private const string PageTitle = "pageTitle";
        private FormErrorsViewModel Sut { get; } = new FormErrorsViewModel();

        [Fact]
        public void WhenThereAreNoErrors_ShouldProcessPageTitleCorrectly() =>
            Assert.Equal(PageTitle, Sut.ProcessPageTitleIfThereIsAnError(PageTitle));

        [Fact]
        public void WhenThereAreErrors_ShouldProcessPageTitleCorrectly()
        {
            Sut.AddError("id", "field", "errorMessage");

            var result = Sut.ProcessPageTitleIfThereIsAnError(PageTitle);
            
            Assert.Equal("Error: pageTitle", result);
        }
    }
}