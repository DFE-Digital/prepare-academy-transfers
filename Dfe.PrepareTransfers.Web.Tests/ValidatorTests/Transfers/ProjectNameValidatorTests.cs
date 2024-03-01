using Dfe.PrepareTransfers.Data;
using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Validators.Transfers;
using Moq;
using Xunit;
using Dfe.PrepareTransfers.Web.Pages.Projects.AcademyAndTrustInformation;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.Transfers
{
    public class ProjectNameValidatorTests
    {
        private readonly Mock<IProjects> _projectRepository;
        private readonly ProjectNameValidator _validator;

        public ProjectNameValidatorTests()
        {
            _validator = new ProjectNameValidator();
            _projectRepository = new Mock<IProjects>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void WhenProjectNameIsEmpty_ShouldSetError(string projectName)
        {
            var projectSearch = new ProjectNameModel(_projectRepository.Object)
            {
                ProjectName = projectName
            };
            var result = await _validator.TestValidateAsync(projectSearch);

            result.ShouldHaveValidationErrorFor(x => x.ProjectName)
                .WithErrorMessage("Enter the Project name");
        }

        [Fact]
        public async void WhenProjectNameNotEmpty_ShouldNotSetError()
        {
            var projectSearch = new ProjectNameModel(_projectRepository.Object)
            {
                ProjectName = "New Project Name"
            };
            var result = await _validator.TestValidateAsync(projectSearch);

            result.ShouldNotHaveValidationErrorFor(x => x);
        }
    }
}
