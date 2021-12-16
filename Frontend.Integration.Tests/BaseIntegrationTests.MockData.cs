using System;
using System.Collections.Generic;
using System.Linq;
using Data.TRAMS.Models;
using Frontend.Integration.Tests.Fixtures;

namespace Frontend.Integration.Tests
{
    public partial class BaseIntegrationTests
    {
        private AcademiesApiFixtures _fixtures =new AcademiesApiFixtures();
        protected IEnumerable<TramsProjectSummary> GetProjects(Action<TramsProjectSummary> postSetup = null)
        {
            var projects = _fixtures.Projects();
            if (postSetup != null)
            {
                postSetup(projects.First());
            }
            
            foreach (var project in projects)
            {
                _factory.AddGetWithJsonResponse($"/trust/{project.TransferringAcademies[0].IncomingTrustUkprn}",  AcademiesApiFixtures.Trust());
                _factory.AddGetWithJsonResponse(
                    $"/establishment/{project.TransferringAcademies[0].OutgoingAcademyUkprn}", AcademiesApiFixtures.Establishment());
            }

            _factory.AddGetWithJsonResponse("/academyTransferProject", projects);
            return projects;
        }

        protected TramsProject GetProject(Action<TramsProject> postSetup = null)
        {
            var project = AcademiesApiFixtures.Project();
            if (postSetup != null)
            {
                postSetup(project);
            }

            _factory.AddGetWithJsonResponse($"/academyTransferProject/{project.ProjectUrn}", project);
            _factory.AddGetWithJsonResponse($"/trust/{project.TransferringAcademies[0].IncomingTrustUkprn}",  AcademiesApiFixtures.Trust());
            _factory.AddGetWithJsonResponse($"/trust/{project.OutgoingTrustUkprn}",  AcademiesApiFixtures.Trust());
            _factory.AddGetWithJsonResponse($"/educationPerformance/{project.TransferringAcademies[0].OutgoingAcademy.Urn}",  AcademiesApiFixtures.EducationPerformance());
            _factory.AddGetWithJsonResponse(
                $"/establishment/{project.TransferringAcademies[0].OutgoingAcademyUkprn}", AcademiesApiFixtures.Establishment());

            return project;
        }
    }
}