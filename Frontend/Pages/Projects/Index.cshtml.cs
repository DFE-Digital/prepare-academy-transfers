using System.Threading.Tasks;
using Data;
using Data.Models;
using Frontend.ExtensionMethods;
using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Projects
{
    public class Index : CommonPageModel
    {
        private readonly ITaskListService _taskListService;
        public ProjectTaskListViewModel ProjectTaskListViewModel { get; private set; }
        public Index(ITaskListService taskListService)
        {
            _taskListService = taskListService;
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            ProjectTaskListViewModel = await _taskListService.BuildTaskListStatusesAsync(Urn);
            return Page();
        }
    }
}