using API.Mapping;
using API.Models.D365;
using Newtonsoft.Json;
using System;
using System.Linq;
using Xunit;

namespace API.Tests
{
    public class JsonFieldExtractorTests
    {
        #region GetAllFieldAnnotationsTests

        [Fact]
        public void ClassWithNoAttributesReturnsEmptyList()
        {
            var helper = new ODataModelHelper<ClassWithNoAttributes>();
            var result = helper.GetSelectClause();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r == "statuscode");
            Assert.Contains(result, r => r == "statecode");
        }

        [Fact]
        public void ClassWithNoAnnotatedPropertiesReturnsEmptyList()
        {
            var helper = new ODataModelHelper<ClassWithOneAttributeNoAnnotation>();
            var result = helper.GetSelectClause();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r == "statuscode");
            Assert.Contains(result, r => r == "statecode");
        }

        [Fact]
        public void ClassWithOneAttributeAndAnnotatedReturnsAttributeName()
        {
            var helper = new ODataModelHelper<ClassWithOneAttributeAndAnnotated>();
            var result = helper.GetSelectClause();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, r => r == "statuscode");
            Assert.Contains(result, r => r == "statecode");
            Assert.Contains(result, r => r == "JsonFieldName");
        }

        [Fact]
        public void ClassWithOneAttributeAndAnnotatedAndMetadataRetrunsFieldNameWithNoMetadataSection()
        {
            var helper = new ODataModelHelper<ClassWithOneAttributeAndAnnotatedWithMetadata>();
            var result = helper.GetSelectClause();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, r => r == "statuscode");
            Assert.Contains(result, r => r == "statecode");
            Assert.Contains(result, r => r == "JsonFieldName");
        }

        [Fact]
        public void ClassWithMixedAnnotationsTest()
        {
            var helper = new ODataModelHelper<ClassWithMixedAnnotations1>();
            var result = helper.GetSelectClause();

            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Contains(result, r => r == "AnnotatedNoMetadata");
            Assert.Contains(result, r => r == "AnnotatedWithMetadata");
        }

        #endregion

        #region GetPropertyAnnotationTests

        [Fact]
        public void ExtractingAnnotationFromFieldThrowsException()
        {
            var helper = new ODataModelHelper<ClassWithNoAttributes>();

            Assert.Throws<ArgumentException>(() =>
            {
                return helper.GetPropertyAnnotation("_someInt");
            });
        }

        [Fact]
        public void ExtractingAnnotationWhereNoneExistsThrowsException()
        {
            var helper = new ODataModelHelper<ClassWithOneAttributeNoAnnotation>();

            Assert.Throws<InvalidOperationException>(() =>
            {
                return helper.GetPropertyAnnotation(nameof(ClassWithOneAttributeNoAnnotation.IntProperty));
            });
        }

        [Fact]
        public void CanExtractNonExtensionAttributeFromField()
        {
            var helper = new ODataModelHelper<ClassWithOneAttributeAndAnnotated>();

            var result = helper.GetPropertyAnnotation(nameof(ClassWithOneAttributeAndAnnotated.IntProperty));

            Assert.Equal("JsonFieldName", result);
        }

        [Fact]
        public void CanExtractExtensionAttributeFromField()
        {
            var helper = new ODataModelHelper<ClassWithOneAttributeAndAnnotatedWithMetadata>();

            var result = helper.GetPropertyAnnotation(nameof(ClassWithOneAttributeAndAnnotatedWithMetadata.IntProperty));

            Assert.Equal("JsonFieldName", result);
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
