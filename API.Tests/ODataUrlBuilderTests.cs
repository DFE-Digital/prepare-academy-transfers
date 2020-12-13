using API.HttpHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace API.Tests
{
    public class ODataUrlBuilderTests
    {
        #region Build Retrieve Multiple Url Tests

        [Fact]
        public void BuildFilterUrl_NullRouteThrowsException()
        {
            var route = (string)null;
            var fields = new List<string>();
            var filters = new List<string>();
            var expandClause = string.Empty;

            Assert.Throws<ArgumentException>(() =>
            {
                return ODataUrlBuilder.BuildFilterUrl(route, fields, expandClause, filters);
            });
        }

        [Fact]
        public void BuildFilterUrl_EmptyRouteThrowsException()
        {
            var route = string.Empty;
            var fields = new List<string>();
            var filters = new List<string>();
            var expandClause = string.Empty;

            Assert.Throws<ArgumentException>(() =>
            {
                return ODataUrlBuilder.BuildFilterUrl(route, fields, expandClause, filters);
            });
        }

        [Fact]
        public void BuildFilterUrl_NullFieldsNullRouteReturnsRoute()
        {
            var fields = (List<string>)null;
            var filters = (List<string>)null;
            var expandClause = string.Empty;
            var route = "tests";

            var result = ODataUrlBuilder.BuildFilterUrl(route, fields, expandClause, filters);

            Assert.Equal(route, result);
        }

        [Fact]
        public void BuildFilterUrl_EmptyFieldsEmotyRouteReturnsRoute()
        {
            var fields = new List<string>();
            var filters = new List<string>();
            var expandClause = string.Empty;
            var route = "tests";

            var result = ODataUrlBuilder.BuildFilterUrl(route, fields, expandClause, filters);

            Assert.Equal(route, result);
        }

        [Fact]
        public void BuildFilterUrl_HandlesOneFieldNoFilters()
        {
            var fields = new List<string> { "field" };
            var filters = (List<string>)null;
            var expandClause = string.Empty;
            var route = "tests";

            var result = ODataUrlBuilder.BuildFilterUrl(route, fields, expandClause, filters);

            Assert.Equal("tests?$select=field", result);
        }

        [Fact]
        public void BuildFilterUrl_HandlesThreeFieldNoFilters()
        {
            var fields = new List<string> { "field", "another_field", "one_more_field" };
            var filters = (List<string>)null;
            var expandClause = string.Empty;
            var route = "tests";

            var result = ODataUrlBuilder.BuildFilterUrl(route, fields, expandClause, filters);

            Assert.Equal("tests?$select=field,another_field,one_more_field", result);
        }

        [Fact]
        public void BuildFilterUrl_HandlesNoFieldsOneFilter()
        {
            var fields = new List<string>();
            var filters = new List<string>{ "filter"};
            var expandClause = string.Empty;
            var route = "tests";

            var result = ODataUrlBuilder.BuildFilterUrl(route, fields, expandClause, filters);

            Assert.Equal("tests?$filter=filter", result);
        }

        [Fact]
        public void BuildFilterUrl_HandlesNoFieldsMultipleFilters()
        {
            var fields = new List<string>();
            var filters = new List<string> { "(someProp eq 1)", "and (anotherProp eq 2)" };
            var expandClause = string.Empty;
            var route = "tests";

            var result = ODataUrlBuilder.BuildFilterUrl(route, fields, expandClause, filters);

            Assert.Equal("tests?$filter=(someProp eq 1) and (anotherProp eq 2)", result);
        }

        [Fact]
        public void BuildFilterUrl_HandlesOneFieldOneFilter()
        {
            var fields = new List<string> { "field" };
            var filters = new List<string> { "(someProp eq 1)" };
            var expandClause = string.Empty;
            var route = "tests";

            var result = ODataUrlBuilder.BuildFilterUrl(route, fields, expandClause, filters);

            Assert.Equal("tests?$select=field&$filter=(someProp eq 1)", result);
        }

        [Fact]
        public void BuildFilterUrl_HandlesMultipleFieldsOneFilter()
        {
            var fields = new List<string> { "field", "another_field", "one_more_field" };
            var filters = new List<string> { "(someProp eq 1)" };
            var expandClause = string.Empty;
            var route = "tests";

            var result = ODataUrlBuilder.BuildFilterUrl(route, fields, expandClause, filters);

            Assert.Equal("tests?$select=field,another_field,one_more_field&$filter=(someProp eq 1)", result);
        }

        [Fact]
        public void BuildFilterUrl_HandlesMultipleFieldsMultipleFilters()
        {
            var fields = new List<string> { "field", "another_field", "one_more_field" };
            var filters = new List<string> { "(someProp eq 1)", "and (anotherProp eq 2)" };
            var expandClause = string.Empty;
            var route = "tests";

            var result = ODataUrlBuilder.BuildFilterUrl(route, fields, expandClause, filters);

            Assert.Equal("tests?$select=field,another_field,one_more_field&$filter=(someProp eq 1) and (anotherProp eq 2)", result);
        }

        #endregion

        #region Build Url For Retrieve One Tests

        [Fact]
        public void BuildRetrieveOneUrl_NoRouteIdSetThrowsException()
        {
            var route = string.Empty;
            var fields = new List<string>();
            var id = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() =>
            {
                return ODataUrlBuilder.BuildRetrieveOneUrl(route, id, fields);
            });
        }

        [Fact]
        public void BuildRetrieveOneUrl_NullRouteIdSetThrowsException()
        {
            var route = string.Empty;
            var fields = new List<string>();
            var id = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() =>
            {
                return ODataUrlBuilder.BuildRetrieveOneUrl(route, id, fields);
            });
        }

        [Fact]
        public void BuildRetrieveOneUrl_HandlesNullFieldsList()
        {
            var route = "tests";
            var fields = (List<string>)null;
            var id = Guid.Parse("834814f9-b0b8-ea11-a812-000d3a122b89");

            var result = ODataUrlBuilder.BuildRetrieveOneUrl(route, id, fields);

            Assert.Equal("tests(834814f9-b0b8-ea11-a812-000d3a122b89)", result);
        }

        [Fact]
        public void BuildRetrieveOneUrl_HandlesOneField()
        {
            var route = "tests";
            var fields = new List<string> { "field" };
            var id = Guid.Parse("834814f9-b0b8-ea11-a812-000d3a122b89");

            var result = ODataUrlBuilder.BuildRetrieveOneUrl(route, id, fields);

            Assert.Equal("tests(834814f9-b0b8-ea11-a812-000d3a122b89)?$select=field", result);
        }

        [Fact]
        public void BuildRetrieveOneUrl_HandlesThreeFields()
        {
            var route = "tests";
            var fields = new List<string> { "field", "another_field", "one_more_field" };
            var id = Guid.Parse("834814f9-b0b8-ea11-a812-000d3a122b89");

            var result = ODataUrlBuilder.BuildRetrieveOneUrl(route, id, fields);

            Assert.Equal("tests(834814f9-b0b8-ea11-a812-000d3a122b89)?$select=field,another_field,one_more_field", result);
        }

        #endregion

        #region Build In Filter Tests

        [Fact]
        public void BuildInFilter_NullFieldName_ThrowsException()
        {
            var fieldName = (string)null;
            var allowedValues = new List<string> { "value" };

            Assert.Throws<ArgumentException>(() =>
            {
                return ODataUrlBuilder.BuildInFilter(fieldName, allowedValues);
            });
        }

        [Fact]
        public void BuildInFilter_EmptyFieldName_ThrowsException()
        {
            var fieldName = string.Empty;
            var allowedValues = new List<string> { "value" };

            Assert.Throws<ArgumentException>(() =>
            {
                return ODataUrlBuilder.BuildInFilter(fieldName, allowedValues);
            });
        }

        [Fact]
        public void BuildInFilter_NullAllowedValues_ThrowsException()
        {
            var fieldName = "field";
            var allowedValues = (List<string>)null;

            Assert.Throws<ArgumentException>(() =>
            {
                return ODataUrlBuilder.BuildInFilter(fieldName, allowedValues);
            });
        }

        [Fact]
        public void BuildInFilter_EmptyAllowedValues_ThrowsException()
        {
            var fieldName = "field";
            var allowedValues = new List<string>();

            Assert.Throws<ArgumentException>(() =>
            {
                return ODataUrlBuilder.BuildInFilter(fieldName, allowedValues);
            });
        }

        [Fact]
        public void BuildInFilter_Handles_OneAllowedValue()
        {
            var fieldName = "field";
            var allowedFields = new List<string> { "value" };

            var result = ODataUrlBuilder.BuildInFilter(fieldName, allowedFields);

            Assert.Equal("(field eq value)", result);
        }

        [Fact]
        public void BuildInFilter_Handles_TwoAllowedValues()
        {
            var fieldName = "field";
            var allowedFields = new List<string> { "value_one", "value_two" };

            var result = ODataUrlBuilder.BuildInFilter(fieldName, allowedFields);

            Assert.Equal("(field eq value_one or field eq value_two)", result);
        }

        [Fact]
        public void BuildInFilter_Handles_FourAllowedValues()
        {
            var fieldName = "field";
            var allowedFields = new List<string> { "value_one", "value_two", "value_three", "value_four" };

            var result = ODataUrlBuilder.BuildInFilter(fieldName, allowedFields);

            Assert.Equal("(field eq value_one or field eq value_two or field eq value_three or field eq value_four)", result);
        }

        #endregion

        #region BuildOrSearchQuery Tests

        [Fact]
        public void BuildOrSearchQuery_EmptyQuery_ThrowsException()
        {
            var query = string.Empty;
            var fieldNames = new List<string> { "field" };

            Assert.Throws<ArgumentException>(() =>
            {
                return ODataUrlBuilder.BuildInFilter(query, fieldNames);
            });
        }

        [Fact]
        public void BuildOrSearchQuery_NullQuery_ThrowsException()
        {
            var query = (string)null;
            var fieldNames = new List<string> { "field" };

            Assert.Throws<ArgumentException>(() =>
            {
                return ODataUrlBuilder.BuildInFilter(query, fieldNames);
            });
        }

        [Fact]
        public void BuildOrSearchQuery_NullFieldNames_ThrowsException()
        {
            var query = "query";
            var fieldNames = (List<string>)null;

            Assert.Throws<ArgumentException>(() =>
            {
                return ODataUrlBuilder.BuildInFilter(query, fieldNames);
            });
        }

        [Fact]
        public void BuildOrSearchQuery_EmptyFieldNames_ThrowsException()
        {
            var query = "query";
            var fieldNames = new List<string>();

            Assert.Throws<ArgumentException>(() =>
            {
                return ODataUrlBuilder.BuildInFilter(query, fieldNames);
            });
        }

        [Fact]
        public void BuildOrSearchQuery_Handles_OneFieldName()
        {
            var query = "query";
            var fieldNames = new List<string>{ "field" };

            var result = ODataUrlBuilder.BuildOrSearchQuery(query, fieldNames);

            Assert.Equal("(contains(field,'query'))", result);
        }

        [Fact]
        public void BuildOrSearchQuery_Handles_TwoFieldNames()
        {
            var query = "query";
            var fieldNames = new List<string> { "field", "another_field" };

            var result = ODataUrlBuilder.BuildOrSearchQuery(query, fieldNames);

            Assert.Equal("(contains(field,'query') or contains(another_field,'query'))", result);
        }

        [Fact]
        public void BuildOrSearchQuery_Handles_FourFieldNames()
        {
            var query = "query";
            var fieldNames = new List<string> { "field1", "field2", "field3", "field4" };

            var result = ODataUrlBuilder.BuildOrSearchQuery(query, fieldNames);

            Assert.Equal("(contains(field1,'query') or contains(field2,'query') or contains(field3,'query') or contains(field4,'query'))", result);
        }

        #endregion
    }
}
