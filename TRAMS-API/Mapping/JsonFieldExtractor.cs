using API.Models.GET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace API.Mapping
{
    public static class JsonFieldExtractor
    {
        /// <summary>
        /// Generates a list of all JsonProperty annotations set against a type's properties. Will not include @metadata extensions
        /// </summary>
        /// <param name="modelType">The type to extract the JsonProperty annotations for</param>
        /// <returns>A list of the JsonProperty annotation - excludes any @metadata extensions</returns>
        public static List<string> GetAllFieldAnnotations(Type modelType)
        {
            var jsonProps = modelType.GetProperties()
                         .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null)
                         .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName.Split("@").First())
                         .ToList();

            return jsonProps;
        }

        /// <summary>
        /// Gets the JsonProperty annotation of a certain property without any @metadata extensions
        /// Throws InvalidOperationException if the field doesn't have a JsonProperty annotation
        /// </summary>
        /// <param name="t">The type the property is attached to</param>
        /// <param name="propertyName">The property - use nameof to ensure quasi strong type</param>
        /// <returns>The name of the JsonProperty without any @metadata extensions applied</returns>
        public static string GetPropertyAnnotation(Type t, string propertyName)
        {
            var propertyInfo = t.GetProperty(propertyName);

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
