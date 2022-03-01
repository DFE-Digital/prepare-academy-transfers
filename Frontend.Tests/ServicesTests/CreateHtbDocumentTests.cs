using Frontend.Services;
using Frontend.Services.Responses;
using Moq;
using Frontend.Services.Interfaces;
using Xunit;

namespace Frontend.Tests.ServicesTests
{
    public class CreateHtbDocumentTests
    {
        private readonly CreateHtbDocument _subject;
        private readonly Mock<IGetHtbDocumentForProject> _getHtbDocumentForProject;
        private readonly string _projectUrn = "projectId";

        public CreateHtbDocumentTests()
        {
            _getHtbDocumentForProject = new Mock<IGetHtbDocumentForProject>();
            _subject = new CreateHtbDocument(_getHtbDocumentForProject.Object);
        }

        public class ExecuteTests : CreateHtbDocumentTests
        {
            [Fact]
            public async void GivenGetHtbDocumentForProjectReturnsNotFound_ReturnsCorrectError()
            {
                _getHtbDocumentForProject.Setup(r => r.Execute(It.IsAny<string>())).ReturnsAsync(
                    new GetProjectTemplateResponse()
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
                    new GetProjectTemplateResponse()
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
