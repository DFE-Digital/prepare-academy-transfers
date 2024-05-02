using System.Collections.Generic;
using Dfe.PrepareTransfers.Data;
using System.IO;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Models.ProjectList;
using Dfe.PrepareTransfers.Web.Pages.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.PrepareTransfers.Data.Models;

namespace Dfe.PrepareTransfers.Web.Tests.ModelTests.ProjectListTests.ProjectListDownloadTests
{
    public class ProjectListDownloadTests : BaseTests
    {
        private readonly Mock<ILogger<Index>> _logger = new Mock<ILogger<Index>>();
        private readonly Index _subject;

        public ProjectListDownloadTests()
        {
            var tempData = new Mock<ITempDataDictionary>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Query = new QueryCollection();

            var pageContext = new PageContext
            {
                HttpContext = httpContext
            };

            _subject = new Index(ProjectRepository.Object, _logger.Object)
            {
                PageContext = pageContext,
                TempData = tempData.Object
            };
        }

        [Fact]
        public async Task OnGetDownload_SuccessfulResponse_ReturnsFileStreamResult()
        {
            // Setup the mock to return a successful response
            ProjectRepository.Setup(repo => repo.DownloadProjectExport(It.IsAny<GetProjectSearchModel>()))
                .ReturnsAsync(new ApiResponse<FileStreamResult>(HttpStatusCode.OK, new FileStreamResult(new MemoryStream(), "text/csv")));

            // Act
            var result = await _subject.OnGetDownload();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("project_list.xlsx", result.FileDownloadName);
        }

        [Fact]
        public async Task OnGetDownload_UnsuccessfulResponse_ReturnsEmptyCsvFile()
        {
            // Setup the mock to return an unsuccessful response
            ProjectRepository.Setup(repo => repo.DownloadProjectExport(It.IsAny<GetProjectSearchModel>()))
                .ReturnsAsync(new ApiResponse<FileStreamResult>(HttpStatusCode.InternalServerError, null));

            // Act
            var result = await _subject.OnGetDownload();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("empty.csv", result.FileDownloadName);
        }
    }
}