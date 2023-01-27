using FluentValidation;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Validators;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ValidatorTests
{
    public class LoginValidatorTests
    {
        private readonly LoginValidator _validator;
        private const string ErrorMessage = "Incorrect username and password";
        private const string Username = "username";
        private const string Password = "password";
        
        public LoginValidatorTests()
        {
            _validator = new LoginValidator();
        }

        [Fact]
        public async void WhenUsernameIsIncorrect_ShouldError()
        {
            var vm = new LoginViewModel()
            {
                Username = "wrong",
                Password = "password"
            };
            var validationContext = new ValidationContext<LoginViewModel>(vm)
            {
                RootContextData =
                {
                    ["ConfigUsername"] = Username,
                    ["ConfigPassword"] = Password
                }
            };
            var result = await _validator.ValidateAsync(validationContext);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorMessage, result.ToString());
        }

        [Fact]
        public async void WhenPasswordIsIncorrect_ShouldError()
        {
            var vm = new LoginViewModel()
            {
                Username = Username,
                Password = "wrong"
            };
            var validationContext = new ValidationContext<LoginViewModel>(vm)
            {
                RootContextData =
                {
                    ["ConfigUsername"] = Username,
                    ["ConfigPassword"] = Password
                }
            };
            var result = await _validator.ValidateAsync(validationContext);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            //Error message same as username to not indicate detail of the error
            Assert.Equal(ErrorMessage, result.ToString());
        }
        
        [Fact]
        public async void WhenUsernameAndPasswordCorrect_ShouldNotError()
        {
            var vm = new LoginViewModel()
            {
                Username = Username,
                Password = Password
            };
            var validationContext = new ValidationContext<LoginViewModel>(vm)
            {
                RootContextData =
                {
                    ["ConfigUsername"] = Username,
                    ["ConfigPassword"] = Password
                }
            };
            var result = await _validator.ValidateAsync(validationContext);
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}