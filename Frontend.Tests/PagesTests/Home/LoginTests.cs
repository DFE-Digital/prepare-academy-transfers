using System;
using Castle.Core.Configuration;
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
    public class LoginTests : PageTests
    {
        private readonly Login _subject;
        private const string Username = "username";
        private const string Password = "password";
        public LoginTests()
        {
            
            var configuration = new Mock<IConfiguration>();
            configuration.SetupGet(x => x[It.Is<string>(s=>s == Username)]).Returns(Username);
            configuration.SetupGet(x => x[It.Is<string>(s => s == Password)]).Returns(Password);
            _subject = new Login(configuration.Object);
        }
        
        [Fact]
        public async void GivenCorrectLoginDetailsOnPostRedirectToIndex()
        {
            //Arrange
            var authenticationService = new Mock<IAuthenticationService>();
             var sp = new Mock<IServiceProvider>();
            sp.Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(() => authenticationService.Object);
            _subject.PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            _subject.PageContext.HttpContext.RequestServices = sp.Object;
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(s => s.IsLocalUrl(It.IsAny<string>())).Returns(false);
            _subject.Url = new Mock<IUrlHelper>().Object;
            _subject.LoginViewModel = new LoginViewModel()
            {
                Username = Username,
                Password = Password
            };
                
            //Action
            var result = await _subject.OnPostAsync();
            
            //Assert
            ControllerTestHelpers.AssertResultRedirectsToPage(result,"/Home/Index");
        }
    }
}