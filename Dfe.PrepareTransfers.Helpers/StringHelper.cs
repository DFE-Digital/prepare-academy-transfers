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
    }
}