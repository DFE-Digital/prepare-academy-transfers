using Data;
using Frontend.ExtensionMethods;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Frontend.ExtensionMethods.TempDataExtensions;

namespace Frontend.Pages.Projects.ProjectAssignment
{
 public class IndexModel : PageModel
    {
		private readonly IUserRepository _userRepository;
		private readonly IProjects _projectRepository;
		public IndexModel(IUserRepository userRepository, IProjects projectRepository)
		{
			_projectRepository = projectRepository;
			_userRepository = userRepository;
		}

		public string SchoolName { get; private set; }
		public string Id { get; set; }
		public IEnumerable<User> DeliveryOfficers { get; set; }
		public string SelectedDeliveryOfficer { get; set; }

		public async Task<IActionResult> OnGet(string urn)
		{
			var projectResponse = await _projectRepository.GetByUrn(urn);
			Id = urn;
			SchoolName = projectResponse.Result.IncomingTrustName;
			SelectedDeliveryOfficer = projectResponse.Result?.AssignedUser?.FullName;

			DeliveryOfficers = await _userRepository.GetAllUsers();

			return Page();
		}

		public async Task<IActionResult> OnPost(string urn, string selectedName, bool unassignDeliveryOfficer)
		{
			var project = (await _projectRepository.GetByUrn(urn)).Result;

			if (unassignDeliveryOfficer)
			{
				project.AssignedUser = new User(Guid.Empty.ToString(), string.Empty, string.Empty);

				await _projectRepository.Update(project);

				TempData.SetNotification("Done", "Project is unassigned");
			}
			else if (!string.IsNullOrEmpty(selectedName))
			{
				var deliveryOfficers = await _userRepository.GetAllUsers();

				project.AssignedUser = deliveryOfficers.SingleOrDefault(u => u.FullName == selectedName);

				await _projectRepository.Update(project);

				TempData.SetNotification("Done", "Project is assigned");
			}

			return RedirectToPage(Links.Project.Index.PageName, new { urn });
		}
	}
}
