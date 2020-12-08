using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API.HttpHelpers
{
    public static class ODataUrlBuilder
    {
        public static string BuildFilterUrl(string route, List<string> fields, List<string> filters)
        {
            //A route must be set
            if(string.IsNullOrEmpty(route))
            {
                throw new ArgumentException("The route must not be null or empty");
            }

            var urlSegments = new StringBuilder();

            urlSegments.Append($"{route}");

            var nextQueryParamSymbol = "?";

            if(fields!= null && fields.Any())
            {
                urlSegments.Append($"{nextQueryParamSymbol}$select={string.Join(',', fields)}");
                nextQueryParamSymbol = "&";
            }

            if (filters!= null && filters.Any())
            {
                urlSegments.Append($"{nextQueryParamSymbol}$filter={string.Join(" ", filters)}");
            }
            
            var url = urlSegments.ToString();

            return url;
        }

        public static string BuildRetrieveOneUrl(string route, Guid id, List<string> fields)
        {
            //A route must be set
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentException("The route must not be null or empty");
            }

            var url = $"{route}({id})";

            if (fields != null && fields.Any())
            {
                url += $"?$select={string.Join(',', fields)}";
            }

            return url;
        }
        
        public static string BuildInFilter(string fieldName, List<string> allowedValues)
        {
            if (allowedValues == null || allowedValues.Count == 0)
            {
                throw new ArgumentException("The filter requires at least one allowed value");
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("The field name must not be null or empty");
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
            if (fieldNames == null || fieldNames.Count == 0)
            {
                throw new ArgumentException("The filter requires at least one field name");
            }
                
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException("The query must not be null or empty");
            }

            var individualItems = fieldNames.Select(f => $"contains({f},'{query}') or")
                                            .SkipLast(1)
                                            .ToList();

            individualItems.Add($"contains({fieldNames.Last()},'{query}')");

            var outerQuery = $"({string.Join(" ", individualItems)})";

            return outerQuery;
        }
    }
}
