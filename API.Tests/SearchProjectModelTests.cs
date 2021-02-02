using API.Models.Downstream.D365;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace API.Tests
{
    public class SearchProjectModelTests
    {
        [Fact]
        public void Hashcode_Generation_Test()
        {
            var model = new SearchProjectsD365Model
            {
                ProjectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000"),
                ProjectInitiatorFullName = "Initiator Name",
                ProjectInitiatorUid = "uniqueIdentifier",
                ProjectName = "Project Name",
                ProjectStatus = Models.D365.Enums.ProjectStatusEnum.Completed
            };

            var result = model.GetHashCode();

            Assert.Equal(267452621, result);
        }

        [Fact]
        public void Hashcode_Distinct_Test()
        {
            var reference = new SearchProjectsD365Model
            {
                ProjectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000"),
                ProjectInitiatorFullName = "Initiator Name",
                ProjectInitiatorUid = "uniqueIdentifier",
                ProjectName = "Project Name",
                ProjectStatus = Models.D365.Enums.ProjectStatusEnum.Completed
            };

            var same = new SearchProjectsD365Model
            {
                ProjectId = Guid.Parse("00000003-0000-0ff1-ce00-000000000000"),
                ProjectInitiatorFullName = "Name of initiator",
                ProjectInitiatorUid = "unique identifier of initiator",
                ProjectName = "The name of the project",
                ProjectStatus = Models.D365.Enums.ProjectStatusEnum.Completed
            };

            var different = new SearchProjectsD365Model
            {
                ProjectId = Guid.Parse("20000003-0000-0ff1-ce00-000000000002"),
                ProjectInitiatorFullName = "Initiator Name 2",
                ProjectInitiatorUid = "uniqueIdentifier 2",
                ProjectName = "Project Name 2",
                ProjectStatus = Models.D365.Enums.ProjectStatusEnum.Completed
            };

            var result = new List<SearchProjectsD365Model> { reference, same, different }.Distinct().ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.ProjectId == Guid.Parse("20000003-0000-0ff1-ce00-000000000002"));
            Assert.Contains(result, r => r.ProjectId == Guid.Parse("00000003-0000-0ff1-ce00-000000000000"));
        }
    }
}
