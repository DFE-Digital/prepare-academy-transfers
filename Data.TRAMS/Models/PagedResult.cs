using System.Collections.Generic;
using System.Linq;

namespace Data.TRAMS.Models
{
   public class PagedResult<T>
   {
      public PagedResult(IEnumerable<T> results = null, int totalCount = 0)
      {
         Results = results ?? Enumerable.Empty<T>();
         TotalCount = totalCount;
      }

      public IEnumerable<T> Results { get; set; }
      public int TotalCount { get; set; }
   }
}
