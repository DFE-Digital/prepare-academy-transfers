using System;
using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Dfe.PrepareTransfers.Web.Integration.Tests.Fixtures;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Dfe.PrepareTransfers.Web.Integration.Tests
{
	public partial class BaseIntegrationTests
	{
		protected IEnumerable<TramsProjectSummary> SetUpMockGetProjectCalls(Action<TramsProjectSummary> postSetup = null)
		{
			IEnumerable<TramsProjectSummary> projects = AcademiesApiFixtures.Projects();
			if (postSetup != null)
			{
				postSetup(projects.First());
			}

            var searchModel = new GetProjectSearchModel(1, 10, null, Enumerable.Empty<string>(), Enumerable.Empty<string>());

			ApiV2Wrapper<IEnumerable<TramsProjectSummary>> projectResults = new ApiV2Wrapper<IEnumerable<TramsProjectSummary>>() 
			{ 
				Data = projects,
				Paging = new ApiV2PagingInfo() { RecordCount = 1 }
			};
			_factory.AddPostWithJsonRequest("/transfer-project/GetTransferProjects", searchModel, projectResults);

            _factory.AddGetWithJsonResponse("/legacy/projects/status", new ProjectFilterParameters() { 
				Statuses = Enumerable.Empty<string>().ToList(), 
				AssignedUsers = Enumerable.Empty<string>().ToList(),
            });

            return projects;
		}

		protected AcademisationProject GetProject(Action<AcademisationProject> postSetup = null)
		{
			var project = AcademiesApiFixtures.Project();
			if (postSetup != null)
			{
				postSetup(project);
			}

			_factory.AddGetWithJsonResponse($"/transfer-project/{project.ProjectUrn}", project);
			_factory.AddGetWithJsonResponse($"/v4/trust/{project.TransferringAcademies[0].IncomingTrustUkprn}", AcademiesApiFixtures.Trust());
			_factory.AddGetWithJsonResponse($"/v4/trust/{project.OutgoingTrustUkprn}", AcademiesApiFixtures.Trust());
			_factory.AddGetWithJsonResponse($"/educationPerformance/{project.TransferringAcademies[0].OutgoingAcademy.Urn}", AcademiesApiFixtures.EducationPerformance());
			_factory.AddGetWithJsonResponse(
				$"/v4/establishment/{project.TransferringAcademies[0].OutgoingAcademyUkprn}", AcademiesApiFixtures.Establishment());

			return project;
		}
	}
}