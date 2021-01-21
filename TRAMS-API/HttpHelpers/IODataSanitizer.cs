namespace API.HttpHelpers
{
    public interface IODataSanitizer
    {
        /// <summary>
        /// Sanitizes search strings for use in OData filters
        /// </summary>
        /// <param name="input">The input search string</param>
        /// <returns></returns>
        public string Sanitize(string input);
    }
}
