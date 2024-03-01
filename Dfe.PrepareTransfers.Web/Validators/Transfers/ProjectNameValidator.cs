using FluentValidation;
using Dfe.PrepareTransfers.Web.Pages.Projects.AcademyAndTrustInformation;

namespace Dfe.PrepareTransfers.Web.Validators.Transfers
{
    public class ProjectNameValidator : AbstractValidator<ProjectNameModel>
    {
        public ProjectNameValidator()
        {
            RuleFor(x => x.ProjectName)
                .NotEmpty()
                .WithMessage("Enter the Project name");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}