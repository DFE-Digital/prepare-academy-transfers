using AutoFixture;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Pages.Projects.ProjectAssignment;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.ProjectAssignment
{
	public class ProjectAssignmentTests
	{
		public class GetTests : ProjectAssignmentTests
		{
			private readonly IndexModel _subject;
			private readonly Mock<IUserRepository> _userRepository;
			private readonly Mock<IProjects> _projectRepository;
			private readonly Fixture _fixture = new Fixture();

			public GetTests()
			{
				_userRepository = new Mock<IUserRepository>();
				_projectRepository = new Mock<IProjects>();
				_subject = new IndexModel(_userRepository.Object, _projectRepository.Object);
			}

			[Fact]
			public async Task Should_get_deliveryofficers()
			{
				var deliveryOfficers = _fixture.CreateMany<User>();
				_userRepository.Setup(m => m.GetAllUsers()).ReturnsAsync(deliveryOfficers);

				_projectRepository.Setup(m => m.GetByUrn(It.IsAny<string>())).ReturnsAsync(_fixture.Create<RepositoryResult<Project>>());

				var result = await _subject.OnGetAsync("12345");

				Assert.Multiple(
				   () => Assert.IsType<PageResult>(result),
				   () => Assert.Equivalent(deliveryOfficers, _subject.DeliveryOfficers));
			}

			[Fact]
			public async Task Should_get_project_fields()
			{
				var project = _fixture.Create<RepositoryResult<Project>>();
				_projectRepository.Setup(m => m.GetByUrn(It.IsAny<string>())).ReturnsAsync(project);

				var urn = "12345";
				await _subject.OnGetAsync(urn);

				Assert.Multiple(
					() => Assert.Equal(urn, _subject.Urn),
					() => Assert.Equal(project.Result.IncomingTrustName, _subject.IncomingTrustName),
					() => Assert.Equal(project.Result.AssignedUser.FullName, _subject.SelectedDeliveryOfficer)
				);
			}
		}

		public class PostTests : ProjectAssignmentTests
		{
			private readonly IndexModel _subject;
			private readonly Mock<IUserRepository> _userRepository;
			private readonly Mock<IProjects> _projectRepository;
			private readonly Fixture _fixture = new Fixture();
			private readonly Mock<ITempDataDictionary> _tempDataDictionary;

			public PostTests()
			{
				_userRepository = new Mock<IUserRepository>();
				_projectRepository = new Mock<IProjects>();
				_tempDataDictionary = new Mock<ITempDataDictionary>();
				_subject = new IndexModel(_userRepository.Object, _projectRepository.Object) { TempData = _tempDataDictionary.Object };
			}

			[Fact]
			public async Task Should_assign_delivery_officer()
			{
				var expectedResult = _fixture.Create<RepositoryResult<Project>>();
				_projectRepository.Setup(m => m.GetByUrn(It.IsAny<string>())).ReturnsAsync(expectedResult);

				var users = _fixture.Create<List<User>>();
				_userRepository.Setup(m => m.GetAllUsers()).ReturnsAsync(users);

				await _subject.OnPostAsync("12345", users.First().FullName);

				_projectRepository.Verify(m => m.AssignUser(It.Is<Project>(p =>
					p.AssignedUser.Id == users.First().Id &&
					p.AssignedUser.EmailAddress == users.First().EmailAddress &&
					p.AssignedUser.FullName == users.First().FullName)), Times.Once);
			}

			[Fact]
			public async Task Should_redirect_to_tasklist()
			{
				var expectedResult = _fixture.Create<RepositoryResult<Project>>();
				_projectRepository.Setup(m => m.GetByUrn(It.IsAny<string>())).ReturnsAsync(expectedResult);

				var users = _fixture.Create<List<User>>();
				_userRepository.Setup(m => m.GetAllUsers()).ReturnsAsync(users);
				var urn = "12345";

				var result = await _subject.OnPostAsync(urn, users.First().FullName);

				RedirectToPageResult redirect = null;

				Assert.Multiple(
					() => redirect = Assert.IsType<RedirectToPageResult>(result),
					() => Assert.Equal("/Projects/Index", redirect.PageName),
					() => Assert.Equal(urn, redirect.RouteValues["urn"])
				);
			}

			[Fact]
			public async Task Should_assign_delivery_officer_and_notify()
			{
				var expectedResult = _fixture.Create<RepositoryResult<Project>>();
				_projectRepository.Setup(m => m.GetByUrn(It.IsAny<string>())).ReturnsAsync(expectedResult);

				var users = _fixture.Create<List<User>>();
				_userRepository.Setup(m => m.GetAllUsers()).ReturnsAsync(users);

				await _subject.OnPostAsync("12345", users.First().FullName);

				_tempDataDictionary.VerifySet(m => m["Success.Message"] = "Project is assigned");
				_tempDataDictionary.VerifySet(m => m["Success.Title"] = "Done");
			}

			[Fact]
			public async Task Should_unassign_delivery_officer()
			{
				var expectedResult = _fixture.Create<RepositoryResult<Project>>();
				_projectRepository.Setup(m => m.GetByUrn(It.IsAny<string>())).ReturnsAsync(expectedResult);

				var users = _fixture.Create<List<User>>();
				_userRepository.Setup(m => m.GetAllUsers()).ReturnsAsync(users);

				var project = expectedResult.Result;
				project.AssignedUser = null;

				await _subject.OnPostAsync("12345", "", true);

				_projectRepository.Verify(m => m.AssignUser(It.Is<Project>(p =>
					p.AssignedUser.Id == Guid.Empty.ToString() &&
					p.AssignedUser.EmailAddress == string.Empty &&
					p.AssignedUser.FullName == string.Empty)), Times.Once);
			}

			[Fact]
			public async Task Should_unassign_delivery_officer_and_notify()
			{
				var expectedResult = _fixture.Create<RepositoryResult<Project>>();
				_projectRepository.Setup(m => m.GetByUrn(It.IsAny<string>())).ReturnsAsync(expectedResult);

				var users = _fixture.Create<List<User>>();
				_userRepository.Setup(m => m.GetAllUsers()).ReturnsAsync(users);

				await _subject.OnPostAsync("12345", "", true);

				_tempDataDictionary.VerifySet(m => m["Success.Message"] = "Project is unassigned");
				_tempDataDictionary.VerifySet(m => m["Success.Title"] = "Done");
			}
		}
	}
}
