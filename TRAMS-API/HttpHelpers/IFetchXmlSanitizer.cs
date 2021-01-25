namespace API.HttpHelpers
{
    public interface IFetchXmlSanitizer
    {
        /// <summary>
        /// Sanitizes strings for use with Fetch XML queries
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns></returns>
        public string Sanitize(string input);
    }
}
