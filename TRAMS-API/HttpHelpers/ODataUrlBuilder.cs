using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API.HttpHelpers
{
    public static class ODataUrlBuilder
    {
        public static string BuildUrl(string route, List<string> fields, List<string> filters)
        {
            var urlSegments = new StringBuilder();

            urlSegments.Append($"{route}?");
            urlSegments.Append($"$select={string.Join(',', fields)}");
            urlSegments.Append($"&$filter={string.Join(" ", filters)}");

            var url = urlSegments.ToString();

            return url;
        }

        
        public static string BuildInFilter(string fieldName, List<string> allowedValues)
        {
            if (allowedValues.Count == 0)
            {
                throw new ArgumentException("The filter requires at least one allowed value");
            }

            var individualItems = allowedValues.Select(v => $"{fieldName} eq {v} or")
                                               .SkipLast(1)
                                               .ToList();

            individualItems.Add($"{fieldName} eq {allowedValues.Last()}");


            var outerQuery = $"({string.Join(" ", individualItems)})";

            return outerQuery;
        }

        public static string BuildOrSearchQuery(string query, List<string> fieldNames)
        {
            if (fieldNames.Count == 0)
                throw new ArgumentException("The filter requires at least one field name");

            var individualItems = fieldNames.Select(f => $"contains({f},'{query}') or")
                                            .SkipLast(1)
                                            .ToList();

            individualItems.Add($"contains({fieldNames.Last()},'{query}')");

            var outerQuery = $"({string.Join(" ", individualItems)})";

            return outerQuery;
        }
    }
}
