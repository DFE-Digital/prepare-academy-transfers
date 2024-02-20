using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Web.Models.ProjectList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Dfe.PrepareTransfers.Web.Pages.Home
{
    public class Index : PageModel
    {
        private const int PageSize = 10;

        private readonly ILogger<Index> _logger;
        private readonly IProjects _projectsRepository;
        private List<ProjectSearchResult> _projects;

        public IReadOnlyList<ProjectSearchResult> Projects => _projects.AsReadOnly();
        public int TotalProjectCount { get; private set; }

        public int SearchCount { get; private set; }
        public Index(IProjects projectsRepository, ILogger<Index> logger)
        {
            _projectsRepository = projectsRepository;
            _logger = logger;
        }

        public int StartingPage { get; private set; } = 1;
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => TotalProjectCount > CurrentPage * PageSize;
        public int PreviousPage => CurrentPage - 1;
        public int NextPage => CurrentPage + 1;
        public int TotalPages => TotalProjectCount % PageSize == 0
            ? TotalProjectCount / PageSize
            : (TotalProjectCount / PageSize) + 1;

        [BindProperty(SupportsGet = true)] public string ReturnUrl { get; set; }
        [BindProperty(SupportsGet = true)] public int CurrentPage { get; set; } = 1;

        [BindProperty] public ProjectListFilters Filters { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Filters.PersistUsing(TempData).PopulateFrom(Request.Query);

            if (RedirectToReturnUrl(out IActionResult actionResult)) return actionResult;

            RepositoryResult<List<ProjectSearchResult>> projects =
                  await _projectsRepository.GetProjects(CurrentPage, Filters.Title, PageSize);

            _projects = new List<ProjectSearchResult>(projects.Result.Where(r => r.Reference is not null));
            SearchCount = projects.Result.Count;
            TotalProjectCount = projects.TotalRecords;

            if (CurrentPage - 5 > 1) StartingPage = CurrentPage - 5;

            _logger.LogInformation("Home page loaded");
            return Page();
        }


        /// <summary>
        ///    If there is a return url, redirects the user to that page after logging in
        /// </summary>
        /// <param name="actionResult">action result to redirect to</param>
        /// <returns>true if redirecting</returns>
        private bool RedirectToReturnUrl(out IActionResult actionResult)
        {
            actionResult = null;
            var decodedUrl = "";
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                decodedUrl = WebUtility.UrlDecode(ReturnUrl);
            }

            if (Url.IsLocalUrl(decodedUrl))
            {
                actionResult = Redirect(ReturnUrl);
                return true;
            }

            return false;
        }

        public async Task<FileStreamResult> OnGetDownload()
        {
            Filters.PersistUsing(TempData).PopulateFrom(Request.Query);
            ApiResponse<FileStreamResult> response = await _projectsRepository.DownloadProjectExport(CurrentPage, PageSize, Filters.Title);

            if (response.Success)
            {
                return response.Body;
            }
            else
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write("");
                writer.Flush();
                stream.Position = 0;

                var fileStreamResult = new FileStreamResult(stream, "text/csv")
                {
                    FileDownloadName = "empty.csv"
                };

                return fileStreamResult;
            }
        }
    }
}
