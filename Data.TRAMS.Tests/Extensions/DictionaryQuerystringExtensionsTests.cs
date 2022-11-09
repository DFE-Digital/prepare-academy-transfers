using System.Collections.Generic;
using Data.TRAMS.ExtensionMethods;
using FluentAssertions;
using Xunit;

namespace Data.TRAMS.Tests.Extensions
{
   public class DictionaryQuerystringExtensionsTests
   {
      private static readonly IDictionary<string, string> Parameters = new Dictionary<string, string>
      {
         { "first", "v1" },
         { "second", "v2" }
      };

      [Fact]
      public void Should_convert_string_pairs_to_a_querystring()
      {
         Parameters.ToQueryString().Should().Be("?first=v1&second=v2");
      }

      [Fact]
      public void Should_optionally_prefix_with_a_question_mark()
      {
         Parameters.ToQueryString(prefix: false).StartsWith('?').Should().BeFalse();
      }

      [Fact]
      public void Should_produce_an_empty_string_if_no_parameters_are_provided()
      {
         new Dictionary<string, string>().ToQueryString().Should().BeEmpty();
      }

      [Fact]
      public void Should_cope_with_null_values()
      {
         var withNullValue = new Dictionary<string, string>
         {
            { "key", null }
         }.ToQueryString();

         withNullValue.Should().Be("?key=");
      }

      [Fact]
      public void Should_optionally_skip_parameters_with_null_values()
      {
         var skippedEmpty = new Dictionary<string, string>(Parameters)
         {
            { "key", null }
         }.ToQueryString(keepEmpty: false);

         skippedEmpty.Should().NotContain("key=");
         skippedEmpty.Should().Be("?first=v1&second=v2");
      }
   }
}
