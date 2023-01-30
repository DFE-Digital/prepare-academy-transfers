using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;

namespace Dfe.PrepareTransfers.Data.TRAMS.ExtensionMethods
{
   public static class DictionaryQuerystringExtensions
   {
      /// <summary>
      ///    Converts a <see cref="IDictionary{string, string}" /> to an encoded querystring.
      /// </summary>
      /// <param name="parameters">
      ///    An <see cref="IDictionary{string, string}" /> containing parameter names (of
      ///    <see cref="string" />) and values (of <see cref="string" />)
      /// </param>
      /// <param name="prefix">A <see cref="bool" /> defining whether the querystring should have a '?' prefix (default: true)</param>
      /// <param name="keepEmpty">
      ///    A <see cref="bool" /> defining whether keys with null/empty values should be kept (default:
      ///    true)
      /// </param>
      /// <returns>A string representing the parameters combined, UrlEncoded and (optionally) prefixed ready to be used in a URI</returns>
      public static string ToQueryString(this IDictionary<string, string> parameters, bool prefix = true,
         bool keepEmpty = true)
      {
         IList<string> parameterPairs = parameters
            .Where(x => keepEmpty || string.IsNullOrWhiteSpace(x.Value) is false)
            .Select(x => $"{Encode(x.Key)}={Encode(x.Value)}")
            .ToList();

         var prefixContent = prefix ? "?" : string.Empty;

         return parameterPairs.Count > 0
            ? $"{prefixContent}{string.Join("&", parameterPairs)}"
            : string.Empty;

         string Encode(string x)
         {
            return string.IsNullOrWhiteSpace(x) ? string.Empty : UrlEncoder.Default.Encode(x);
         }
      }
   }
}
