using API.Mapping;
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
            var result = JsonFieldExtractor.GetAllFieldAnnotations(typeof(ClassWithNoAttributes));

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ClassWithNoAnnotatedPropertiesReturnsEmptyList()
        {
            var result = JsonFieldExtractor.GetAllFieldAnnotations(typeof(ClassWithOneAttributeNoAnnotation));

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ClassWithOneAttributeAndAnnotatedReturnsAttributeName()
        {
            var result = JsonFieldExtractor.GetAllFieldAnnotations(typeof(ClassWithOneAttributeAndAnnotated));

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("JsonFieldName", result.First());
        }

        [Fact]
        public void ClassWithOneAttributeAndAnnotatedNoMetadataRetrunsFieldNameWithNoMetadataSection()
        {
            var result = JsonFieldExtractor.GetAllFieldAnnotations(typeof(ClassWithOneAttributeAndAnnotatedWithMetadata));

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("JsonFieldName", result.First());
        }

        [Fact]
        public void ClassWithMixedAnnotationsTest()
        {
            var result = JsonFieldExtractor.GetAllFieldAnnotations(typeof(ClassWithMixedAnnotations1));

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r == "AnnotatedNoMetadata");
            Assert.Contains(result, r => r == "AnnotatedWithMetadata");
        }

        #endregion

        #region GetPropertyAnnotationTests

        [Fact]
        public void ExtractingAnnotationFromFieldThrowsException()
        {

            Assert.Throws<ArgumentException>(() =>
            {
                return JsonFieldExtractor
                       .GetPropertyAnnotation(typeof(ClassWithNoAttributes),
                                                     "_someInt");
            });
        }

        [Fact]
        public void ExtractingAnnotationWhereNoneExistsThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                return JsonFieldExtractor
                       .GetPropertyAnnotation(typeof(ClassWithOneAttributeNoAnnotation),
                                              nameof(ClassWithOneAttributeNoAnnotation.IntProperty));
            });
        }

        [Fact]
        public void CanExtractNonExtensionAttributeFromField()
        {
            var result = JsonFieldExtractor
                         .GetPropertyAnnotation(typeof(ClassWithOneAttributeAndAnnotated),
                                                nameof(ClassWithOneAttributeAndAnnotated.IntProperty));

            Assert.Equal("JsonFieldName", result);
        }

        [Fact]
        public void CanExtractExtensionAttributeFromField()
        {
            var result = JsonFieldExtractor
                         .GetPropertyAnnotation(typeof(ClassWithOneAttributeAndAnnotatedWithMetadata),
                                                nameof(ClassWithOneAttributeAndAnnotatedWithMetadata.IntProperty));

            Assert.Equal("JsonFieldName", result);
        }

        #endregion 
    }

    internal class ClassWithNoAttributes
    {
        [JsonProperty("SomAttribute")]
        private int _someInt;
    }

    internal class ClassWithOneAttributeNoAnnotation
    {
        public int IntProperty { get; set; }
    }

    internal class ClassWithOneAttributeAndAnnotated
    {
        [JsonProperty("JsonFieldName")]
        public int IntProperty { get; set; }
    }

    internal class ClassWithOneAttributeAndAnnotatedWithMetadata
    {
        [JsonProperty("JsonFieldName@OData.Community.Display.V1.FormattedValue")]
        public int IntProperty { get; set; }
    }

    internal class ClassWithMixedAnnotations1
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
