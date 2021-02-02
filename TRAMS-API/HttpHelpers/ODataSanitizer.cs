namespace API.HttpHelpers
{
    public class ODataSanitizer : IODataSanitizer
    {
        public string Sanitize(string input)
        {
            var output = input?.Replace("'", "''")
                               .Replace("&", "%26")
                               .Replace("+", "%2B")
                               .Replace("?", "%3F")
                               .Replace("#", "%23");

            return output;
        }
    }
}
