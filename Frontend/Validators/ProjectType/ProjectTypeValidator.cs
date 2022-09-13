using FluentValidation;
using Frontend.Models;

namespace Frontend.Validators.ProjectType
{
	public class ProjectTypeValidator : AbstractValidator<ProjectTypeViewModel>
	{
		public ProjectTypeValidator()
		{
			RuleFor(vm => vm.Type)
				.NotNull()
				.WithMessage("Select a project type");
		}
	}
}
