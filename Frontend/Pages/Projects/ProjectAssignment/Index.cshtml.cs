using Data;
using Data.Models;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
			if (unassignDeliveryOfficer)
			{
				var updatedProject = new Project
				{
					Urn = urn,
					AssignedUser = new User(Guid.Empty.ToString(), string.Empty, string.Empty)
				};

				await _projectRepository.Update(updatedProject);
				
				//TempData.SetNotification(NotificationType.Success, "Done", "Project is unassigned");
			}
			else if (!string.IsNullOrEmpty(selectedName))
			{
				var deliveryOfficers = await _userRepository.GetAllUsers();

				var updatedProject = new Project
				{
					AssignedUser = deliveryOfficers.SingleOrDefault(u => u.FullName == selectedName)
				};

				await _projectRepository.Update(updatedProject);
				
				//TempData.SetNotification(NotificationType.Success, "Done", "Project is assigned");
			}

			return RedirectToPage(Links.Project.Index.PageName, new { urn });
		}
	}
}
