using System;
using System.Threading.Tasks;
using Data;
using Data.Mock;
using Data.Models;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Features;
using Frontend.Tests.Helpers;
using Frontend.Validators.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.Features
{
    public class TypeTests : PageTests
    {
        private readonly Pages.Projects.Features.Type _subject;

        public TypeTests()
        {
            _subject = new Pages.Projects.Features.Type(ProjectRepository.Object)
            {
                Urn = ProjectUrn0001
            };
        }

        public class TypeGetTests : TypeTests
        {
            [Fact]
            public async void GivenUrn_GetsProject()
            {
                await _subject.OnGetAsync();
                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                _subject.Urn = ProjectErrorUrn;
                var response = await _subject.OnGetAsync();
                var viewResult = Assert.IsType<ViewResult>(response);
                var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);

                Assert.Equal(ErrorPageName, viewResult.ViewName);
                Assert.Equal(ErrorMessage, viewModel);
            }
        }

        public class TypePostTests : TypeTests
        {
            [Theory]
            [InlineData(TransferFeatures.TransferTypes.MatClosure)]
            [InlineData(TransferFeatures.TransferTypes.TrustsMerging)]
            public async void GivenNonOtherType_UpdatesTheProject(TransferFeatures.TransferTypes transferType)
            {
                _subject.FeaturesTypeViewModel.TypeOfTransfer = transferType;
                
                await _subject.OnPostAsync();

                ProjectRepository.Verify(
                    r => r.Update(It.Is<Project>(project => project.Features.TypeOfTransfer == transferType)),
                    Times.Once);
            }

            [Fact]
            public async void GivenOtherTypeAndText_UpdatesTheProject()
            {
                _subject.FeaturesTypeViewModel.TypeOfTransfer = TransferFeatures.TransferTypes.Other;
                _subject.FeaturesTypeViewModel.OtherType = "Other";
            
                await _subject.OnPostAsync();

                ProjectRepository.Verify(
                    r => r.Update(It.Is<Project>(project =>
                        project.Features.TypeOfTransfer == TransferFeatures.TransferTypes.Other &&
                        project.Features.OtherTypeOfTransfer == "Other")),
                    Times.Once);
            }

            [Fact]
            public async void GivenTypeOfTransfer_RedirectsToIndex()
            {
                _subject.FeaturesTypeViewModel.TypeOfTransfer = TransferFeatures.TransferTypes.SatClosure;
                
                var result = await _subject.OnPostAsync();
                ControllerTestHelpers.AssertResultRedirectsToPage(result, "/Projects/Features/Index",
                    new RouteValueDictionary(new {Urn = ProjectUrn0001}));
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                _subject.Urn = ProjectErrorUrn;
                _subject.FeaturesTypeViewModel.TypeOfTransfer = TransferFeatures.TransferTypes.SatClosure;

                var response = await _subject.OnPostAsync();
                var viewResult = Assert.IsType<ViewResult>(response);
                var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);

                Assert.Equal(ErrorPageName, viewResult.ViewName);
                Assert.Equal(ErrorMessage, viewModel);
            }

            [Fact]
            public async void GivenReturnToPreview_RedirectsToPreviewPage()
            {
                _subject.FeaturesTypeViewModel.TypeOfTransfer = TransferFeatures.TransferTypes.MatClosure;
                _subject.FeaturesTypeViewModel.OtherType = "Other type";
                _subject.ReturnToPreview = true;
                
                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(
                    response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new {id = ProjectUrn0001})
                );
            }

            [Fact]
            public async void GivenUpdateReturnsError_DisplayErrorPage()
            {
                _subject.FeaturesTypeViewModel.TypeOfTransfer = TransferFeatures.TransferTypes.SatClosure;

                ProjectRepository.Setup(r => r.Update(It.IsAny<Project>()))
                    .ReturnsAsync(new RepositoryResult<Project>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            StatusCode = System.Net.HttpStatusCode.NotFound,
                            ErrorMessage = "Project not found"
                        }
                    });

                var response = await _subject.OnPostAsync();
                var viewResult = Assert.IsType<ViewResult>(response);
                var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);

                Assert.Equal(ErrorPageName, viewResult.ViewName);
                Assert.Equal(ProjectNotFound, viewModel);
            }
        }
    }
}