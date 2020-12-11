using API.HttpHelpers;
using API.Models.D365;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace API.Mapping
{
    public class ODataModelHelper<T> : IODataModelHelper<T> where T : BaseD365Model
    {
        private readonly Type _type;

        public ODataModelHelper()
        {
            _type = typeof(T);
        }
        /// <summary>
        /// Generates a list of all JsonProperty annotations set against a type's properties. Will not include @metadata extensions
        /// Will exclude any nested classes and their annotations
        /// </summary>
        /// <param name="modelType">The type to extract the JsonProperty annotations for</param>
        /// <returns>A list of the JsonProperty annotation - excludes any @metadata extensions</returns>
        public List<string> GetSelectClause()
        {
            var jsonProps = _type.GetProperties()
                                 .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null && p.PropertyType.FullName.Contains("System."))
                                 .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName.Split("@").First())
                                 .ToList();

            var props = _type.GetProperties().ToList();

            foreach(var prop in props)
            {
                var type = prop.GetType();
                var isValueType = type.IsValueType;
            }

            return jsonProps;
        }

        public string GetExpandClause()
        {
            var properties = _type.GetProperties()
                                   .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null && 
                                              !p.PropertyType.FullName.Contains("System."))
                                   .ToList();

            var individualExpandClauses = new List<string>();

            foreach(var property in properties)
            {
                var jsonProps = property.PropertyType.GetProperties()
                                                     .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null && p.PropertyType.FullName.Contains("System."))
                                                     .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName.Split("@").First())
                                                     .ToList();

                var navPropertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>().PropertyName;
                var navPropertyFields = string.Join(',', jsonProps);

                var expandClause = $"{navPropertyAttribute}($select={navPropertyFields})";

                individualExpandClauses.Add(expandClause);
            }

            var outerClause = $"$expand={string.Join(',', individualExpandClauses)}";

            return outerClause;
        }

        /// <summary>
        /// Gets the JsonProperty annotation of a certain property without any @metadata extensions
        /// Throws InvalidOperationException if the field doesn't have a JsonProperty annotation
        /// </summary>
        /// <param name="t">The type the property is attached to</param>
        /// <param name="propertyName">The property - use nameof to ensure quasi strong type</param>
        /// <returns>The name of the JsonProperty without any @metadata extensions applied</returns>
        public string GetPropertyAnnotation(string propertyName)
        {
            var propertyInfo = _type.GetProperty(propertyName);

            if(propertyInfo == null)
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
    }
}
