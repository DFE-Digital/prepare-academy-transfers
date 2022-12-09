﻿using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using FluentAssertions;
using Frontend.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Frontend.Integration.Tests.Pages.Projects.ProjectAssignment
{
	public class ProjectAssignmentTests : BaseIntegrationTests
	{
		private readonly IntegrationTestingWebApplicationFactory _factory;

		public ProjectAssignmentTests(IntegrationTestingWebApplicationFactory factory) : base(factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task Should_unassign_a_project()
		{
			var project = GetProject();
			_factory.AddAnyPatch($"/academyTransferProject/{project.ProjectUrn}", project);

			await OpenUrlAsync($"/project-assignment/{project.ProjectUrn}");

			Document.QuerySelector<IHtmlInputElement>("#UnassignDeliveryOfficer").Value = "true";
			await Document.QuerySelector<IHtmlFormElement>("form").SubmitAsync();

			Document.Url.Should().EndWith($"project/{project.ProjectUrn}");
		}

		[Fact]
		public async Task Should_assign_a_project()
		{
			var project = GetProject();
			_factory.AddAnyPatch($"/academyTransferProject/{project.ProjectUrn}", project);

			await OpenUrlAsync($"/project-assignment/{project.ProjectUrn}");

			var fullName = "Bob 1";

			Document.QuerySelector<IHtmlOptionElement>($"[value='{fullName}']").IsSelected = true;
			await Document.QuerySelector<IHtmlFormElement>("form").SubmitAsync();

			Document.Url.Should().EndWith($"project/{project.ProjectUrn}");
		}

		[Fact]
		public async Task Should_display_assigned_user()
		{
			var fullName = "Bob Bob";
			var project = GetProject(p => p.AssignedUser = new User(Guid.NewGuid().ToString(), "", fullName));
			await OpenUrlAsync($"/project/{project.ProjectUrn}");

			Document.QuerySelector<IHtmlElement>("[data-id=assigned-user]")!.TextContent.Trim().Should()
				.Be(fullName);
		}

		[Fact]
		public async Task Should_display_unassigned_user()
		{
			var project = GetProject(p => p.AssignedUser = new User(Guid.Empty.ToString(), string.Empty, string.Empty));
			await OpenUrlAsync($"/project/{project.ProjectUrn}");

			Document.QuerySelector<IHtmlElement>("[data-id=assigned-user]")!.TextContent.Trim().Should()
				.Be("Empty");
		}
	}
}