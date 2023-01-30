using Dfe.PrepareTransfers.Web.Services;
using Dfe.PrepareTransfers.Web.Services.Responses;
using Moq;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ServicesTests
{
    public class CreateProjectTemplateTests
    {
        private readonly CreateProjectTemplate _subject;
        private readonly Mock<IGetProjectTemplateModel> _getHtbDocumentForProject;
        private readonly string _projectUrn = "projectId";

        public CreateProjectTemplateTests()
        {
            _getHtbDocumentForProject = new Mock<IGetProjectTemplateModel>();
            _subject = new CreateProjectTemplate(_getHtbDocumentForProject.Object);
        }

        public class ExecuteTests : CreateProjectTemplateTests
        {
            [Fact]
            public async void GivenGetHtbDocumentForProjectReturnsNotFound_ReturnsCorrectError()
            {
                _getHtbDocumentForProject.Setup(r => r.Execute(It.IsAny<string>())).ReturnsAsync(
                    new GetProjectTemplateModelResponse()
                    {
                        ResponseError = new ServiceResponseError
                        {
                            ErrorCode = ErrorCode.NotFound,
                            ErrorMessage = "Error message"
                        }
                    });

                var result = await _subject.Execute(_projectUrn);

                Assert.False(result.IsValid);
                Assert.Equal(ErrorCode.NotFound, result.ResponseError.ErrorCode);
                Assert.Equal("Not found", result.ResponseError.ErrorMessage);
            }

            [Fact]
            public async void GivenGetHtbDocumentForProjectReturnsServiceError_ReturnsCorrectError()
            {
                _getHtbDocumentForProject.Setup(r => r.Execute(It.IsAny<string>())).ReturnsAsync(
                    new GetProjectTemplateModelResponse()
                    {
                        ResponseError = new ServiceResponseError
                        {
                            ErrorCode = ErrorCode.ApiError,
                            ErrorMessage = "Error message"
                        }
                    });

                var result = await _subject.Execute(_projectUrn);

                Assert.False(result.IsValid);
                Assert.Equal(ErrorCode.ApiError, result.ResponseError.ErrorCode);
                Assert.Equal("API has encountered an error", result.ResponseError.ErrorMessage);
            }
        }
    }
}
