using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.HttpHelpers
{
    public class FetchXmlSanitizer : IFetchXmlSanitizer
    {
        public string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            var output = input.Replace("&", "&amp;")
                              .Replace("<", "&lt;")
                              .Replace(">", "&gt;")
                              .Replace("\"", "&quot;")
                              .Replace("'", "&apos;");

            return output;
        }
    }
}
