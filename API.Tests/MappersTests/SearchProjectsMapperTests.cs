using API.Mapping;
using API.Models.Downstream.D365;
using System;
using Xunit;

namespace API.Tests.MapperTests
{
    public class SearchProjectsMapperTests
    {
        private readonly SearchProjectsItemMapper _mapper;

        public SearchProjectsMapperTests()
        {
            _mapper = new SearchProjectsItemMapper();
        }

        [Fact]
        public void NullInput_Returns_Null()
        {
            var result = _mapper.Map(null);

            Assert.Null(result);
        }

        [Fact]
        public void DefaultProjectId_Returns_DefaultProjectId()
        {
            var searchProjectsD365Model = new SearchProjectsD365Model();

            var result = _mapper.Map(searchProjectsD365Model);

            Assert.Equal((Guid)default, result.ProjectId);
        }

        [Fact]
        public void SetProjectId_Returns_ProjectId()
        {
            var searchProjectsD365Model = new SearchProjectsD365Model()
            {
                ProjectId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1")
            };

            var result = _mapper.Map(searchProjectsD365Model);

            Assert.Equal(Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1"), result.ProjectId);
        }

        [Fact]
        public void NullProjectInitiatorUid_Returns_DefaultProjectInitiatorUid()
        {
            var searchProjectsD365Model = new SearchProjectsD365Model();

            var result = _mapper.Map(searchProjectsD365Model);

            Assert.Null(result.ProjectInitiatorUid);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("ProjectInitiator", "ProjectInitiator")]
        [InlineData("joe@bloggs.com", "joe@bloggs.com")]
        public void SetProjectInitiatorUid_Returns_ProjectInitiatorUid(string input, string expected)
        {
            var searchProjectsD365Model = new SearchProjectsD365Model()
            {
                ProjectInitiatorUid = input
            };

            var result = _mapper.Map(searchProjectsD365Model);

            Assert.Equal(expected, result.ProjectInitiatorUid);
        }

        [Fact]
        public void NullProjectName_Returns_ProjectName()
        {
            var searchProjectsD365Model = new SearchProjectsD365Model();

            var result = _mapper.Map(searchProjectsD365Model);

            Assert.Null(result.ProjectName);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("AT-1003", "AT-1003")]
        [InlineData("abcdefghijklmn", "abcdefghijklmn")]
        [InlineData("Multi Word", "Multi Word")]
        public void SetProjectName_Returns_ProjectName(string input, string expected)
        {
            var searchProjectsD365Model = new SearchProjectsD365Model()
            {
                ProjectName = input
            };

            var result = _mapper.Map(searchProjectsD365Model);

            Assert.Equal(expected, result.ProjectName);
        }

        [Fact]
        public void NullProjectInitiatorName_Returns_NullProjectInitiatorName()
        {
            var searchProjectsD365Model = new SearchProjectsD365Model();

            var result = _mapper.Map(searchProjectsD365Model);

            Assert.Null(result.ProjectInitiatorFullName);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Joe", "Joe")]
        [InlineData("Joe Bloggs", "Joe Bloggs")]
        [InlineData("Joe O'Bloggs", "Joe O'Bloggs")]
        public void SetProjectInitiatorName_Returns_ProjectInitiatorName(string input, string expected)
        {
            var searchProjectsD365Model = new SearchProjectsD365Model()
            {
                ProjectInitiatorFullName = input
            };

            var result = _mapper.Map(searchProjectsD365Model);

            Assert.Equal(expected, result.ProjectInitiatorFullName);
        }

        [Fact]
        public void DefaultProjectStatus_Returns_DefaultProjectStatus()
        {
            var searchProjectsD365Model = new SearchProjectsD365Model();
            
            var result = _mapper.Map(searchProjectsD365Model);

            Assert.Equal(default, result.ProjectStatus);
        }

        [Theory]
        [InlineData(Models.D365.Enums.ProjectStatusEnum.Completed, Models.Upstream.Enums.ProjectStatusEnum.Completed)]
        [InlineData(Models.D365.Enums.ProjectStatusEnum.InProgress, Models.Upstream.Enums.ProjectStatusEnum.InProgress)]
        public void SetProjectStatus_Returns_MappedProjectStatus(Models.D365.Enums.ProjectStatusEnum input, Models.Upstream.Enums.ProjectStatusEnum expected)
        {
            var searchProjectsD365Model = new SearchProjectsD365Model
            {
                ProjectStatus = input
            };

            var result = _mapper.Map(searchProjectsD365Model);

            Assert.Equal(expected, result.ProjectStatus);
        }
    }
}