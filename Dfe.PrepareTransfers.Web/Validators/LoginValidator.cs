using Dfe.PrepareTransfers.Web.Models;
using FluentValidation;

namespace Dfe.PrepareTransfers.Web.Validators;

public class LoginValidator : AbstractValidator<LoginViewModel>
{
   public LoginValidator()
   {
      ClassLevelCascadeMode = CascadeMode.Stop;
      RuleFor(x => x.Username)
         .Custom((username, context) =>
         {
            LoginViewModel loginViewModel = context.InstanceToValidate;
            var configUsername = context.RootContextData["ConfigUsername"];
            var configPassword = context.RootContextData["ConfigPassword"];
            if (username != (string)configUsername || loginViewModel.Password != (string)configPassword)
            {
                context.AddFailure($"{nameof(loginViewModel)}.{nameof(username)}", "Incorrect username and password");
            }
         });
   }
}
