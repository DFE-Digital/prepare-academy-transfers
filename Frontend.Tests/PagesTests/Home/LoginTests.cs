using System;
using Frontend.Models;
using Frontend.Pages.Home;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Frontend.Tests.PagesTests.Home
{
    public class LoginTests : BaseTests
    {
        private readonly Login _subject;
        private const string Username = "username";
        private const string Password = "password";
        private readonly Mock<IUrlHelper> _urlHelper = new Mock<IUrlHelper>();
        public LoginTests()
        {
            //Arrange
            var configuration = new Mock<IConfiguration>();
            configuration.SetupGet(x => x[It.Is<string>(s=>s == Username)]).Returns(Username);
            configuration.SetupGet(x => x[It.Is<string>(s => s == Password)]).Returns(Password);
            _subject = new Login(configuration.Object);
            var authenticationService = new Mock<IAuthenticationService>();
            var sp = new Mock<IServiceProvider>();
            sp.Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(() => authenticationService.Object);
            _subject.PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            _subject.PageContext.HttpContext.RequestServices = sp.Object;
            _subject.LoginViewModel = new LoginViewModel()
            {
                Username = Username,
                Password = Password
            };
        }
        
        [Fact]
        public async void GivenCorrectLoginDetailsOnPostRedirectToIndex()
        {
            //Arrange
            _urlHelper.Setup(s => s.IsLocalUrl(It.IsAny<string>())).Returns(false);
            _subject.Url = _urlHelper.Object;
            
            //Act
            var result = await _subject.OnPostAsync();
            
            //Assert
            ControllerTestHelpers.AssertResultRedirectsToPage(result,"/Home/Index");
        }
        
        [Fact]
        public async void GivenCorrectLoginDetailsOnPostRedirectToReturnUrl()
        {
            //Arrange
            _subject.ReturnUrl =
                $"/{nameof(Controllers.TransfersController).Replace(nameof(Controller),string.Empty)}/{nameof(Controllers.TransfersController.TrustName)}";
            _urlHelper.Setup(s => s.IsLocalUrl(_subject.ReturnUrl)).Returns(true);
            _subject.Url = _urlHelper.Object;
            
            //Act
            var result = await _subject.OnPostAsync();
            
            //Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(_subject.ReturnUrl, redirectResult.Url);
        }
    }
}