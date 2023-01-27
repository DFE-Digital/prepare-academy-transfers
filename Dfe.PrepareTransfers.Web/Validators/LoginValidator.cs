using System.Drawing;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Dfe.PrepareTransfers.Web.Validators
{
    public class LoginValidator : AbstractValidator<LoginViewModel>
    {
        public LoginValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Username)
                .Custom((username, context) =>
                {
                    var loginViewModel = context.InstanceToValidate;
                    var configUsername = context.RootContextData["ConfigUsername"];
                    var configPassword = context.RootContextData["ConfigPassword"];
                    if (username != (string) configUsername || loginViewModel.Password != (string) configPassword)
                    {
                        context.AddFailure($"{nameof(loginViewModel)}.{nameof(username)}","Incorrect username and password");
                    }
                });
        }
    }
}