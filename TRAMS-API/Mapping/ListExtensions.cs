using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Mapping
{
    public static class ListExtensions
    {
        public static string ToDelimitedString(this IEnumerable<string> list, string delimiter = ",")
        {
            return string.Join(delimiter, list);
        }
    }
}
