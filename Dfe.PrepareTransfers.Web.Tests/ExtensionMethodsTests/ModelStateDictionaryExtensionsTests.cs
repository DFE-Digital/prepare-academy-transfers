using Dfe.PrepareTransfers.Web.ExtensionMethods;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

// ReSharper disable ClassNeverInstantiated.Global

namespace Dfe.PrepareTransfers.Web.Tests.ExtensionMethodsTests;

public class ModelStateDictionaryExtensionsTests
{
   private const string Something = nameof(Something);
   private const string Another = nameof(Another);

   public class TitleGeneration
   {
      [Fact]
      public void Should_prefix_the_page_title_if_the_page_has_validation_errors()
      {
         var modelState = new ModelStateDictionary();

         modelState.IsValid.Should().BeTrue();
         modelState.BuildPageTitle("Title").Should().NotContain("Error:");

         modelState.AddModelError(Something, "Is wrong!");
         modelState.IsValid.Should().BeFalse();

         modelState.BuildPageTitle("Title").Should().StartWith("Error: ");
      }
   }

   public class FormValidation
   {
      [Fact]
      public void Should_generate_form_group_css_if_the_page_has_validation_errors()
      {
         var modelState = new ModelStateDictionary();

         modelState.IsValid.Should().BeTrue();
         modelState.GetFormGroupErrorStyle().Should().BeEmpty();

         modelState.AddModelError(Something, "is wrong");

         modelState.IsValid.Should().BeFalse();
         modelState.GetFormGroupErrorStyle().Should().NotBeEmpty();
      }

      [Fact]
      public void Should_generate_form_field_css_if_the_page_has_validation_errors()
      {
         var modelState = new ModelStateDictionary();

         modelState.IsValid.Should().BeTrue();
         modelState.GetFormFieldErrorStyle(Something).Should().BeEmpty();

         modelState.AddModelError(Something, "is wrong");

         modelState.IsValid.Should().BeFalse();
         modelState.GetFormFieldErrorStyle(Something).Should().NotBeEmpty();
      }

      [Fact]
      public void Should_only_generate_form_field_css_if_the_specified_field_has_validation_errors()
      {
         var modelState = new ModelStateDictionary();

         modelState.IsValid.Should().BeTrue();
         modelState.GetFormFieldErrorStyle(Something).Should().BeEmpty();

         modelState.AddModelError(Another, "thing is wrong");

         modelState.IsValid.Should().BeFalse();
         modelState.GetFormFieldErrorStyle(Something).Should().BeEmpty();
      }

      [Fact]
      public void Should_generate_form_field_css_if_any_of_the_specified_field_names_match()
      {
         var modelState = new ModelStateDictionary();

         modelState.IsValid.Should().BeTrue();
         modelState.GetFormFieldErrorStyle(Something, Another).Should().BeEmpty();

         modelState.AddModelError(Another, "is wrong");

         modelState.IsValid.Should().BeFalse();
         modelState.GetFormFieldErrorStyle(Something, Another).Should().NotBeEmpty();
      }
   }
}
