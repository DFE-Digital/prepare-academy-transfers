using FluentValidation.TestHelper;
using Dfe.PrepareTransfers.Web.Models.Rationale;
using Dfe.PrepareTransfers.Web.Validators.Rationale;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests
{
    public class RationaleProjectValidatorTests
    {
        private readonly RationaleProjectValidator _validator;

        public RationaleProjectValidatorTests()
        {
            _validator = new RationaleProjectValidator();
        }

        [Fact]
        public async void WhenProjectRationaleIsNull_ShouldSetError()
        {
            var vm = new RationaleProjectViewModel
            {
                ProjectRationale = ""
            };

            var result = await _validator.TestValidateAsync(vm);
            result.ShouldHaveValidationErrorFor(r => r.ProjectRationale)
                .WithErrorMessage("Enter the rationale for the project");
        }
        
        [Fact]
        public async void WhenProjectRationaleIsNotNull_ShouldNotSetError()
        {
            var vm = new RationaleProjectViewModel
            {
                ProjectRationale = "test"
            };

            var result = await _validator.TestValidateAsync(vm);
            result.ShouldNotHaveValidationErrorFor(r => r.ProjectRationale);
        }
    }
}