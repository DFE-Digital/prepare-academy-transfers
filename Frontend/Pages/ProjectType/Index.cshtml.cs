using Frontend.Models;
using Frontend.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Frontend.Pages.ProjectType
{
	public class IndexModel : PageModel
	{
		private const string ErrorMessage = "Select a project type";
		
		private readonly string _conversionsUrl;

		public IndexModel(IOptions<ServiceLinkOptions> options)
		{
			_conversionsUrl = options.Value.ConversionsUrl;
			ProjectType = new ProjectTypeViewModel();
		}

		[BindProperty] 
		public ProjectTypeViewModel ProjectType { get; set; }
		
		public IActionResult OnPost()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			
			if (ProjectType.Type is ProjectTypes.Conversion)
			{
				return Redirect(_conversionsUrl);
			}
			
			return RedirectToPage(Links.ProjectList.Index.PageName);
			
		}
	}
	
	
}
