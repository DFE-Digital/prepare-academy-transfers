using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;

namespace Data.TRAMS.ExtensionMethods
{
   public static class DictionaryQuerystringExtensions
   {
      /// <summary>
      ///    Converts a <see cref="IDictionary{string, string}" /> to an encoded querystring.
      /// </summary>
      /// <param name="parameters">An <see cref="IDictionary{string, string}" /> containing parameter names (of <see cref="string"/>) and values (of <see cref="string"/>)</param>
      /// <param name="prefix">A <see cref="bool" /> defining whether the querystring should have a '?' prefix (default: true)</param>
      /// <returns>A string representing the parameters combined, UrlEncoded and (optionally) prefixed ready to be used in a URI</returns>
      /// <remarks>
      ///    <paramref name="parameters" /> entries with either a null/empty key or value will be excluded as the framework
      ///    UrlEncoder will throw for null values.
      /// </remarks>
      public static string ToQueryString(this IDictionary<string, string> parameters, bool prefix = true)
      {
         IEnumerable<string> parameterPairs = parameters
            .Where(x => string.IsNullOrWhiteSpace(x.Key) is false && string.IsNullOrWhiteSpace(x.Value) is false)
            .Select(x => $"{UrlEncoder.Default.Encode(x.Key)}={UrlEncoder.Default.Encode(x.Value)}");

         return $"{(prefix ? "?" : string.Empty)}{string.Join("&", parameterPairs)}";
      }
   }
}
