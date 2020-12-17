using API.Models.D365;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace API.Mapping
{
    public class ExpandODataModelHelper<T> where T : BaseD365Model
    {
        private readonly Type _type;

        public ExpandODataModelHelper()
        {
            _type = typeof(T);
        }

        public D365ModelRepresentation GetSelectAndExpandClauses()
        {
            var basicProperties = GetBasicProperties(_type);

            var levelOneTypeProperties = GetTypeProperties(_type);

            var modelRepresentation = new D365ModelRepresentation
            {
                RootExpandName = string.Empty,
                BaseProperties = basicProperties
            };

            foreach(var levelOneTypeProp in levelOneTypeProperties)
            {
                modelRepresentation.ExpandProperties.Add(BuildModelRepresentationLevel(levelOneTypeProp));
            }

            return modelRepresentation;
        }

        public D365ModelRepresentation BuildModelRepresentationLevel(PropertyInfo property)
        {
            var basicProperties = GetBasicProperties(property.PropertyType);
            var typeProperties = GetTypeProperties(property.PropertyType);

            var representation = new D365ModelRepresentation
            {
                RootExpandName = ExtractD365PropertyName(property),
                BaseProperties = basicProperties
            };

            foreach(var typeProperty in typeProperties)
            {
                representation.ExpandProperties.Add(BuildModelRepresentationLevel(typeProperty));
            }

            return representation;
        }


        /// <summary>
        /// Generates a list of all JsonProperty annotations set against a type's properties. Will not include @metadata extensions
        /// Will exclude any nested classes and their annotations
        /// </summary>
        /// <returns>A list of the JsonProperty annotation - excludes any @metadata extensions</returns>
        public List<string> GetBasicProperties(Type type)
        {
            var jsonProps = type.GetProperties()
                                .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null && IsSystemType(p))
                                .Select(p => ExtractD365PropertyName(p))
                                .ToList();

            return jsonProps;
        }

        private static string ExtractD365PropertyName(PropertyInfo p)
        {
            return p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName.Split("@").First();
        }

        public List<PropertyInfo> GetTypeProperties(Type type)
        {
            var typeProperties = type.GetProperties()
                                     .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null && !IsSystemType(p))
                                     .ToList();

            return typeProperties;
        }


        public string GetExpandClause()
        {
            var properties = _type.GetProperties()
                                   .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null &&
                                              !IsSystemType(p))
                                   .ToList();

            var individualExpandClauses = new List<string>();

            foreach (var property in properties)
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

            var outerClause = individualExpandClauses.Any() ? $"$expand={string.Join(',', individualExpandClauses)}" : string.Empty;

            return outerClause;
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

        /// <summary>
        /// Helper method to identify if the field is a system type
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static bool IsSystemType(PropertyInfo p)
        {
            return p.PropertyType.FullName.Contains("System.");
        }

        public class D365ModelRepresentation
        {
            public string RootExpandName { get; set; }
            public List<string> BaseProperties { get; set; }

            public List<D365ModelRepresentation> ExpandProperties { get; set; } = new List<D365ModelRepresentation>();
        }

        public string BuildTopLevelExpand(D365ModelRepresentation representation)
        {
            var select = $"$select={string.Join(',', representation.BaseProperties)}";
            var expand = string.Empty;

            var individualExpands = new List<string>();

            foreach(var expandProp in representation.ExpandProperties)
            {
                var individualExpand = $"{expandProp.RootExpandName}({BuildNestedLevelSelectAndExpand(expandProp)})";

                individualExpands.Add(individualExpand);
            }

            if (!representation.ExpandProperties.Any())
            {
                return $"{select}";
            }

            return $"{select}&$expand={string.Join(',', individualExpands)}";
        }

        public string BuildNestedLevelSelectAndExpand(D365ModelRepresentation representation)
        {
            var select = $"$select={string.Join(',', representation.BaseProperties)}";

            var individualExpands = new List<string>();

            foreach (var expandProp in representation.ExpandProperties)
            {
                var individualExpand = $"expand={expandProp.RootExpandName}({BuildNestedLevelSelectAndExpand(expandProp)})";
                individualExpands.Add(individualExpand);
            }

            if (!representation.ExpandProperties.Any())
            {
                return select;
            }

            return $"{select};$expand={string.Join(',', representation.ExpandProperties)}";
        }
    }
}
