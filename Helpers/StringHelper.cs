using System.Globalization;

namespace Helpers
{
    public static class StringHelper
    {
        public static string ToHtmlName(this string propertyName)
        {
            return propertyName.Replace('.', '_')
                .Replace('[','_')
                .Replace(']','_');
        } 
        
        public static string ToTitleCase(this string str)
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(str.ToLower());
        }
    }
}