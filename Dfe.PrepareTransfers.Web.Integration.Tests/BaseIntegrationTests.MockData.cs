using System;
using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Dfe.PrepareTransfers.Web.Integration.Tests.Fixtures;

namespace Dfe.PrepareTransfers.Web.Integration.Tests
{
	public partial class BaseIntegrationTests
	{
		protected IEnumerable<TramsProjectSummary> GetProjects(Action<TramsProjectSummary> postSetup = null)
		{
			IEnumerable<TramsProjectSummary> projects = AcademiesApiFixtures.Projects();
			if (postSetup != null)
			{
				postSetup(projects.First());
			}

			PagedResult<TramsProjectSummary> projectResults = new PagedResult<TramsProjectSummary>(projects, projects.Count());
			_factory.AddGetWithJsonResponse("/transfer-project/GetTransferProjects", projectResults);
            return projects;
		}

		protected TramsProject GetProject(Action<TramsProject> postSetup = null)
		{
			var project = AcademiesApiFixtures.Project();
			if (postSetup != null)
			{
				postSetup(project);
			}

			_factory.AddGetWithJsonResponse($"/transfer-project/{project.ProjectUrn}", project);
			_factory.AddGetWithJsonResponse($"/trust/{project.TransferringAcademies[0].IncomingTrustUkprn}", AcademiesApiFixtures.Trust());
			_factory.AddGetWithJsonResponse($"/trust/{project.OutgoingTrustUkprn}", AcademiesApiFixtures.Trust());
			_factory.AddGetWithJsonResponse($"/educationPerformance/{project.TransferringAcademies[0].OutgoingAcademy.Urn}", AcademiesApiFixtures.EducationPerformance());
			_factory.AddGetWithJsonResponse(
				$"/establishment/{project.TransferringAcademies[0].OutgoingAcademyUkprn}", AcademiesApiFixtures.Establishment());

			return project;
		}
	}
}