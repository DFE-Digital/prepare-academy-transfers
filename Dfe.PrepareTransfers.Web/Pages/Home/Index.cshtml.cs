using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Data;
using Data.Models;
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
      public List<ProjectSearchResult> Projects;
      public int TotalProjectCount;

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

      [BindProperty(SupportsGet = true)] public string ReturnUrl { get; set; }
      [BindProperty(SupportsGet = true)] public int CurrentPage { get; set; } = 1;
      [BindProperty(SupportsGet = true)] public string TitleFilter { get; set; }

      public bool IsFiltered => string.IsNullOrWhiteSpace(TitleFilter) is false;

      public async Task<IActionResult> OnGetAsync()
      {
         if (RedirectToReturnUrl(out IActionResult actionResult)) return actionResult;

         RepositoryResult<List<ProjectSearchResult>> projects =
            await _projectsRepository.GetProjects(CurrentPage, TitleFilter, PageSize);

         Projects = projects.Result;
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
         if (!string.IsNullOrEmpty(ReturnUrl)) decodedUrl = WebUtility.UrlDecode(ReturnUrl);

         if (Url.IsLocalUrl(decodedUrl))
         {
            actionResult = Redirect(ReturnUrl);
            return true;
         }

         return false;
      }
   }
}
