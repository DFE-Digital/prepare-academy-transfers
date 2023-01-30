using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Dfe.PrepareTransfers.Helpers
{
    public static class StringHelper
    {
        public static string ToHtmlName(this string propertyName)
        {
            return propertyName.Replace('.', '_')
                .Replace('[', '_')
                .Replace(']', '_');
        }

        public static string ToTitleCase(this string str)
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(str.ToLower());
        }

        public static string ToHyphenated(this string str)
        {
            var whitespaceRegex = new Regex(@"\s+", RegexOptions.None, TimeSpan.FromSeconds(1));
            return whitespaceRegex.Replace(str, "-");
        }

        public static string RemoveNonAlphanumericOrWhiteSpace(this string str)
        {
            var notAlphanumericWhiteSpaceOrHyphen = new Regex(@"[^\w\s-]", RegexOptions.None, TimeSpan.FromSeconds(1));
            return notAlphanumericWhiteSpaceOrHyphen.Replace(str, string.Empty);
        }
    }
}