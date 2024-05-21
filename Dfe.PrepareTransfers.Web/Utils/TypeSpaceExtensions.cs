using Microsoft.AspNetCore.Html;
using System;
using System.Text.RegularExpressions;

namespace Dfe.PrepareTransfers.Web.Utils;

public static class TypespaceExtensions
{
   private static readonly Regex NotAlphaNumeric = new("[^[a-z0-9-_]", RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2));

   public static HtmlString Stub(this string input)
   {
      return new HtmlString(NotAlphaNumeric.Replace(input.ToLowerInvariant(), "-"));
   }
}
