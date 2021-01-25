using API.Models.Downstream.D365;
using API.ODataHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API.HttpHelpers
{
    public class ODataUrlBuilder<T> : IOdataUrlBuilder<T> where T : BaseD365Model
    {
        private readonly ID365ModelHelper<T> _modelHelper;
        private readonly Type _type;

        public ODataUrlBuilder(ID365ModelHelper<T> modelHelper)
        {
            _modelHelper = modelHelper;
            _type = typeof(T);
        }

        public string BuildFilterUrl(string route, List<string> filters)
        {
            //A route must be set
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentException("The route must not be null or empty");
            }

            var urlSegments = new StringBuilder();

            urlSegments.Append($"{route}");

            var modelRepresentation = _modelHelper.ExtractModelRepresentation();

            var selectAndExpand = _modelHelper.BuildSelectAndExpandClauses(modelRepresentation);

            urlSegments.Append($"?{selectAndExpand}");

            if (filters != null && filters.Any())
            {
                urlSegments.Append($"&$filter={string.Join(" ", filters)}");
            }

            var url = urlSegments.ToString();

            return url;
        }

        public string BuildRetrieveOneUrl(string route, Guid id)
        {
            //A route must be set
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentException("The route must not be null or empty");
            }

            var modelRepresentation = _modelHelper.ExtractModelRepresentation();

            var selectAndExpand = _modelHelper.BuildSelectAndExpandClauses(modelRepresentation);

            var url = $"{route}({id})?{selectAndExpand}";

            return url;
        }

        /// <summary>
        /// Gets the JsonProperty annotation of a certain property without any @metadata extensions
        /// Throws InvalidOperationException if the field doesn't have a JsonProperty annotation
        /// </summary>
        /// <param name="propertyName">The property - use nameof to ensure quasi strong type</param>
        /// <returns>The name of the JsonProperty without any @metadata extensions applied</returns>
        public string GetPropertyAnnotation(string propertyName)
        {
            var propertyInfo = _type.GetProperty(propertyName);

            if (propertyInfo == null)
            {
                throw new ArgumentException($"Class does not define a \"{propertyName}\" Property ");
            }

            var propJsonAnnotation = propertyInfo.GetCustomAttributes(typeof(JsonPropertyAttribute), false);

            if (propJsonAnnotation.FirstOrDefault() is JsonPropertyAttribute cast)
            {
                return cast.PropertyName.Split("@").First();
            }

            throw new InvalidOperationException("Property does not define a JsonProperty annotation");
        }

        public string BuildInFilter(string fieldName, List<string> allowedValues)
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

        public string BuildOrSearchQuery(string query, List<string> fieldNames)
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
