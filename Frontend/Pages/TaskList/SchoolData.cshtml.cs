using Frontend.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.TaskList
{
    public class SchoolData : CommonPageModel
    {
        public PageResult OnGet()
        {
            return Page();
        }
    }
}