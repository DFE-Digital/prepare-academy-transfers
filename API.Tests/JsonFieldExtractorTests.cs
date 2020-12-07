using API.Mapping;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using Xunit;

namespace API.Tests
{
    public class JsonFieldExtractorTests
    {
        [Fact]
        public void ClassWithNoAttributesReturnsEmptyList()
        {
            var result = JsonFieldExtractor.GetFields(typeof(ClassWithNoAttributes));

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ClassWithNoAnnotatedPropertiesReturnsEmptyList()
        {
            var result = JsonFieldExtractor.GetFields(typeof(CLassWithOneAttributeNoAnnotation));

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ClassWithOneAttributeAndAnnotatedReturnsAttributeName()
        {
            var result = JsonFieldExtractor.GetFields(typeof(ClassWithOneAttributeAndAnnotated));

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("JsonFieldName", result.First());
        }

        [Fact]
        public void ClassWithOneAttributeAndAnnotatedNoMetadataRetrunsFieldNameWithNoMetadataSection()
        {
            var result = JsonFieldExtractor.GetFields(typeof(ClassWithOneAttributeAndAnnotatedWithMetadata));

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("JsonFieldName", result.First());
        }

        [Fact]
        public void ClassWithMixedAnnotationsTest()
        {
            var result = JsonFieldExtractor.GetFields(typeof(ClassWithMixedAnnotations1));

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r == "AnnotatedNoMetadata");
            Assert.Contains(result, r => r == "AnnotatedWithMetadata");
        }
    }

    internal class ClassWithNoAttributes
    {
        [JsonProperty("SomAttribute")]
        private int _someInt;
    }

    internal class CLassWithOneAttributeNoAnnotation
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
