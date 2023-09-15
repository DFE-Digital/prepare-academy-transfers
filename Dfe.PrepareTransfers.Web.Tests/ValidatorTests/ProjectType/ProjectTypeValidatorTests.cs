using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Validators.ProjectType;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests.ProjectType
{
    public class ProjectTypeValidatorTests
    {
        private readonly ProjectTypeValidator _validator;
        public ProjectTypeValidatorTests() => _validator = new ProjectTypeValidator();

        [Fact]
        public async void GivenNoSelection_InvalidWithErrorMessage()
        {
            var vm = new ProjectTypeViewModel();
            var result = await _validator.TestValidateAsync(vm);
            result.ShouldHaveValidationErrorFor(v => v.Type)
                .WithErrorMessage("Select a project type");
        }
        
        [Theory]
        [InlineData(ProjectTypes.Conversion)]
        [InlineData(ProjectTypes.Transfer)]
        public async void GivenSelection_ValidWithoutErrorMessage(ProjectTypes projectType)
        {
            var vm = new ProjectTypeViewModel
            {
                Type = projectType
            };
            var result = await _validator.TestValidateAsync(vm);
            result.ShouldNotHaveValidationErrorFor(v => v.Type);
        }
    }
}
