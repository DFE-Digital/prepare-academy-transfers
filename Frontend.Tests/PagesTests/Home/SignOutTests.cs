using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Frontend.Pages.Home;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Home
{
    public class SignOutTests
    {
        public class OnGetAsync : SignOutTests
        {
            private readonly SignOut _subject;

            public OnGetAsync()
            {
                _subject = new SignOut();
            }

            [Fact]
            public async void WhenUserIsSignedOut_ShowsSignOutPage()
            {
                _subject.PageContext = new PageContext()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                         {
                             new Claim(ClaimTypes.Name, "username")
                         }, null))
                    }
                };

                var result = await _subject.OnGetAsync();

                Assert.IsType<PageResult>(result);
            }

            [Fact]
            public async void WhenUserIsSignedIn_CallsSignOutToRemoveSessionCookieAndRedirect()
            {
                var httpContextMock = new Mock<HttpContext>();
                httpContextMock.Setup(context => context.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "username")
                }, "some-auth-type")));

                var authServiceMock = new Mock<IAuthenticationService>();
                authServiceMock.Setup(authService => authService.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult((object)null));
                var serviceProviderMock = new Mock<IServiceProvider>();
                serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IAuthenticationService))).Returns(authServiceMock.Object);
                var mockUrlFactory = new Mock<IUrlHelperFactory>();

                var mockUrlHelper = new Mock<IUrlHelper>();
                mockUrlHelper.Setup(urlHelper => urlHelper.ActionContext).Returns(new ActionContext()
                {
                    ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(),
                    RouteData = new RouteData()
                });

                httpContextMock.Setup(context => context.RequestServices)
                  .Returns(serviceProviderMock.Object);

                _subject.Url = mockUrlHelper.Object;
                _subject.PageContext = new PageContext()
                {
                    HttpContext = httpContextMock.Object,
                };

                var result = await _subject.OnGetAsync();

                authServiceMock.Verify(authService => authService.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(2));
                Assert.Null(result);
            }
        }
    }
}