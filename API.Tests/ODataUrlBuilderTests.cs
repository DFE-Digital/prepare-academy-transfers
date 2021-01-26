using API.HttpHelpers;
using API.Models.Downstream.D365;
using API.ODataHelpers;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace API.Tests
{
    public class ODataUrlBuilderTests
    {
        private ODataUrlBuilder<BaseD365Model> _urlBuilderWithMockedHelper;

        public ODataUrlBuilderTests()
        {
            var mockedHelper = new Mock<ID365ModelHelper<BaseD365Model>>();

            mockedHelper.Setup(m => m.ExtractModelRepresentation()).Returns(new D365ModelRepresentation());
            mockedHelper.Setup(m => m.BuildSelectAndExpandClauses(It.IsAny<D365ModelRepresentation>())).Returns("$select=somefield&$expand=expand($select=expand_field)");
            _urlBuilderWithMockedHelper = new ODataUrlBuilder<BaseD365Model>(mockedHelper.Object);
        }

        #region Build Filter Url Tests

        [Fact]
        public void BuildFilterUrl_MissingRouteThrowsException()
        {
            var filters = new List<string>();

            Assert.Throws<ArgumentException>(() =>
            {
                return _urlBuilderWithMockedHelper.BuildFilterUrl(string.Empty, filters);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                return _urlBuilderWithMockedHelper.BuildFilterUrl(null, filters);
            });
        }

        [Fact]
        public void BuildFilterUrl_EmptyFieldsReturnsCorrectUrl()
        {
            var route = "accounts";
            var filters = new List<string>();

            var result = _urlBuilderWithMockedHelper.BuildFilterUrl(route, filters);

            Assert.Equal("accounts?$select=somefield&$expand=expand($select=expand_field)", result);
        }

        [Fact]
        public void BuildFilterUrl_OneFilterReturnsCorrectUrl()
        {
            var route = "accounts";
            var filters = new List<string> { "somefield eq 1" };

            var result = _urlBuilderWithMockedHelper.BuildFilterUrl(route, filters);

            Assert.Equal("accounts?$select=somefield&$expand=expand($select=expand_field)&$filter=somefield eq 1", result);
        }

        [Fact]
        public void BuildFilterUrl_ThreeFiltersReturnsCorrectUrl()
        {
            var route = "accounts";
            var filters = new List<string> 
            { 
                "(someProp eq 1)", 
                "and (anotherProp eq 2)",
                "and (yetanotherprop eq 3)"
            };

            var result = _urlBuilderWithMockedHelper.BuildFilterUrl(route, filters);

            Assert.Equal("accounts?$select=somefield&$expand=expand($select=expand_field)&$filter=(someProp eq 1) and (anotherProp eq 2) and (yetanotherprop eq 3)", result);
        }

        #endregion

        #region Retrieve One Url Tests

        [Fact]
        public void RetrieveOneUrl_NoRouteThrowsException()
        {
            var guid = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() =>
            {
                return _urlBuilderWithMockedHelper.BuildRetrieveOneUrl(string.Empty, guid);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                return _urlBuilderWithMockedHelper.BuildRetrieveOneUrl(null, guid);
            });
        }

        [Fact]
        public void RetrieveOneUrl_ReturnsCorrectUrl()
        {
            var route = "accounts";
            var id = Guid.Parse("834814f9-b0b8-ea11-a812-000d3a122b89");

            var result = _urlBuilderWithMockedHelper.BuildRetrieveOneUrl(route, id);

            Assert.Equal("accounts(834814f9-b0b8-ea11-a812-000d3a122b89)?$select=somefield&$expand=expand($select=expand_field)", result);
        }

        #endregion 

        #region GetPropertyAnnotationTests

        [Fact]
        public void ExtractingAnnotationFromFieldThrowsException()
        {
            var helper = new D365ModelHelper<ClassWithNoAttributes>();
            var urlBuilder = new ODataUrlBuilder<ClassWithNoAttributes>(helper);

            Assert.Throws<ArgumentException>(() =>
            {
                return urlBuilder.GetPropertyAnnotation("_someInt");
            });
        }

        [Fact]
        public void ExtractingAnnotationWhereNoneExistsThrowsException()
        {
            var helper = new D365ModelHelper<ClassWithOneAttributeNoAnnotation>();
            var urlBuilder = new ODataUrlBuilder<ClassWithOneAttributeNoAnnotation>(helper);

            Assert.Throws<InvalidOperationException>(() =>
            {
                return urlBuilder.GetPropertyAnnotation(nameof(ClassWithOneAttributeNoAnnotation.IntProperty));
            });
        }

        [Fact]
        public void CanExtractNonExtensionAttributeFromField()
        {
            var helper = new D365ModelHelper<ClassWithOneAttributeAndAnnotated>();
            var urlBuilder = new ODataUrlBuilder<ClassWithOneAttributeAndAnnotated>(helper);

            var result = urlBuilder.GetPropertyAnnotation(nameof(ClassWithOneAttributeAndAnnotated.IntProperty));

            Assert.Equal("JsonFieldName", result);
        }

        [Fact]
        public void CanExtractExtensionAttributeFromField()
        {
            var helper = new D365ModelHelper<ClassWithOneAttributeAndAnnotatedWithMetadata>();
            var urlBuilder = new ODataUrlBuilder<ClassWithOneAttributeAndAnnotatedWithMetadata>(helper);

            var result = urlBuilder.GetPropertyAnnotation(nameof(ClassWithOneAttributeAndAnnotatedWithMetadata.IntProperty));

            Assert.Equal("JsonFieldName", result);
        }

        #endregion

        #region Build In Filter Tests

        [Fact]
        public void BuildInFilter_NoFieldName_ThrowsException()
        {
            var allowedValues = new List<string> { "value" };

            Assert.Throws<ArgumentException>(() =>
            {
                return _urlBuilderWithMockedHelper.BuildInFilter(string.Empty, allowedValues);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                return _urlBuilderWithMockedHelper.BuildInFilter(null, allowedValues);
            });
        }

        [Fact]
        public void BuildInFilter_NoAllowedValues_ThrowsException()
        {
            var fieldName = "field";

            Assert.Throws<ArgumentException>(() =>
            {
                return _urlBuilderWithMockedHelper.BuildInFilter(fieldName, null);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                return _urlBuilderWithMockedHelper.BuildInFilter(fieldName, new List<string>());
            });
        }

        [Fact]
        public void BuildInFilter_Handles_OneAllowedValue()
        {
            var fieldName = "field";
            var allowedFields = new List<string> { "value" };

            var result = _urlBuilderWithMockedHelper.BuildInFilter(fieldName, allowedFields);

            Assert.Equal("(field eq value)", result);
        }

        [Fact]
        public void BuildInFilter_Handles_FourAllowedValues()
        {
            var fieldName = "field";
            var allowedFields = new List<string> { "value_one", "value_two", "value_three", "value_four" };

            var result = _urlBuilderWithMockedHelper.BuildInFilter(fieldName, allowedFields);

            Assert.Equal("(field eq value_one or field eq value_two or field eq value_three or field eq value_four)", result);
        }

        #endregion

        #region Build Or Search Query

        [Fact]
        public void BuildOrSearchQuery_EmptyQuery_ThrowsException()
        {
            var fieldNames = new List<string> { "field" };

            Assert.Throws<ArgumentException>(() =>
            {
                return _urlBuilderWithMockedHelper.BuildInFilter(string.Empty, fieldNames);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                return _urlBuilderWithMockedHelper.BuildInFilter(null, fieldNames);
            });
        }

        [Fact]
        public void BuildOrSearchQuery_NoFieldNames_ThrowsException()
        {
            var query = "query";

            Assert.Throws<ArgumentException>(() =>
            {
                return _urlBuilderWithMockedHelper.BuildInFilter(query, null);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                return _urlBuilderWithMockedHelper.BuildInFilter(query, new List<string>());
            });
        }

        [Fact]
        public void BuildOrSearchQuery_Handles_OneFieldName()
        {
            var query = "query";
            var fieldNames = new List<string> { "field" };

            var result = _urlBuilderWithMockedHelper.BuildOrSearchQuery(query, fieldNames);

            Assert.Equal("(contains(field,'query'))", result);
        }


        [Fact]
        public void BuildOrSearchQuery_Handles_FourFieldNames()
        {
            var query = "query";
            var fieldNames = new List<string> { "field1", "field2", "field3", "field4" };

            var result = _urlBuilderWithMockedHelper.BuildOrSearchQuery(query, fieldNames);

            Assert.Equal("(contains(field1,'query') or contains(field2,'query') or contains(field3,'query') or contains(field4,'query'))", result);
        }

        #endregion
    }

    internal class ClassWithNoAttributes : BaseD365Model
    {
        [JsonProperty("SomAttribute")]
        private int _someInt;
    }

    internal class ClassWithOneAttributeNoAnnotation : BaseD365Model
    {
        public int IntProperty { get; set; }
    }

    internal class ClassWithOneAttributeAndAnnotated : BaseD365Model
    {
        [JsonProperty("JsonFieldName")]
        public int IntProperty { get; set; }
    }

    internal class ClassWithOneAttributeAndAnnotatedWithMetadata : BaseD365Model
    {
        [JsonProperty("JsonFieldName@OData.Community.Display.V1.FormattedValue")]
        public int IntProperty { get; set; }
    }

    internal class ClassWithMixedAnnotations1 : BaseD365Model
    {
        public int NoAnnotation { get; set; }

        [JsonProperty("AnnotatedWithMetadata@OData.Community.Display.V1.FormattedValue")]
        public int AnnotatedWithMetadata { get; set; }

        [JsonProperty("AnnotatedNoMetadata")]
        public int AnnotatedNoMetadata { get; set; }

        [JsonProperty("SomAttribute")]
        private int _someInt;
    }
}