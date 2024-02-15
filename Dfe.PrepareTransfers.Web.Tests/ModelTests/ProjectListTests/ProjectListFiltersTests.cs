using System.Collections.Generic;
using Dfe.PrepareTransfers.Web.Models.ProjectList;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ModelTests.ProjectListTests.ProjectFilterListTests
{
    public class ProjectListFiltersTests
    {
        [Fact]
        public void PopulateFrom_ClearsFilters_WhenClearQueryParameterExists()
        {
            // Arrange
            var filters = new ProjectListFilters();
            var store = new Dictionary<string, object>
            {
                { ProjectListFilters.FilterTitle, new string[] { "Bishop" } }
            };

            filters.PersistUsing(store);

            var queryParameters = new List<KeyValuePair<string, StringValues>>
            {
                new("clear", new StringValues(new[] { "true" }))
            };

            // Act
            filters.PopulateFrom(queryParameters);

            // Assert
            Assert.False(filters.IsFiltered);
        }

        [Fact]
        public void PersistUsing_SetsTitle_WhenStoreContainsTitle()
        {
            // Arrange
            var filters = new ProjectListFilters();
            var store = new Dictionary<string, object>
            {
                { ProjectListFilters.FilterTitle, new string[] { "Bishop" } }
            };

            // Act
            filters.PersistUsing(store);

            // Assert
            Assert.Equal("Bishop", filters.Title);
        }

        [Fact]
        public void ClearFiltersFrom_RemovesAllFiltersFromStore()
        {
            // Arrange
            var store = new Dictionary<string, object>
            {
                { ProjectListFilters.FilterTitle, new string[] { "Bishop" } }
            };

            // Act
            ProjectListFilters.ClearFiltersFrom(store);

            // Assert
            Assert.Empty(store);
        }
    }
}