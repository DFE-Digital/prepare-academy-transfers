using API.Models.D365;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace API.ODataHelpers
{
    public class D365ModelHelper<T> : ID365ModelHelper<T> where T : BaseD365Model
    {
        private readonly Type _type;

        public D365ModelHelper()
        {
            _type = typeof(T);
        }

        /// <summary>
        /// Extracts the model representation of a class to enable building the OData Select and Expand clauses.
        /// </summary>
        /// <returns>The <see cref="D365ModelRepresentation"/> of a D365Model</returns>
        public D365ModelRepresentation ExtractModelRepresentation()
        {
            var basicProperties = GetBasicProperties(_type).Distinct().ToList();

            var levelOneTypeProperties = GetTypeProperties(_type);

            var modelRepresentation = new D365ModelRepresentation
            {
                RootExpandName = string.Empty,
                BaseProperties = basicProperties
            };

            foreach (var levelOneTypeProp in levelOneTypeProperties)
            {
                modelRepresentation.ExpandProperties.Add(BuildModelRepresentationLevel(levelOneTypeProp));
            }

            return modelRepresentation;
        }

        /// <summary>
        /// Builds the Select and Expand OData query clauses from a given <see cref="D365ModelRepresentation"/>
        /// </summary>
        /// <param name="representation">The model representation, usually returned by the ExtractModelRepresentation method</param>
        /// <returns>The select and expand clauses (if needed) joined together</returns>
        public string BuildSelectAndExpandClauses(D365ModelRepresentation representation)
        {
            var select = $"$select={string.Join(',', representation.BaseProperties)}";

            var individualExpands = new List<string>();

            foreach (var expandProp in representation.ExpandProperties)
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

        private D365ModelRepresentation BuildModelRepresentationLevel(PropertyInfo property)
        {
            var basicProperties = GetBasicProperties(property.PropertyType).Distinct().ToList();
            var typeProperties = GetTypeProperties(property.PropertyType);

            var representation = new D365ModelRepresentation
            {
                RootExpandName = ExtractD365PropertyName(property),
                BaseProperties = basicProperties
            };

            foreach (var typeProperty in typeProperties)
            {
                representation.ExpandProperties.Add(BuildModelRepresentationLevel(typeProperty));
            }

            return representation;
        }

        private string BuildNestedLevelSelectAndExpand(D365ModelRepresentation representation)
        {
            var select = $"$select={string.Join(',', representation.BaseProperties)}";

            var individualExpands = new List<string>();

            foreach (var expandProp in representation.ExpandProperties)
            {
                var individualExpand = $"{expandProp.RootExpandName}({BuildNestedLevelSelectAndExpand(expandProp)})";
                individualExpands.Add(individualExpand);
            }

            if (!representation.ExpandProperties.Any())
            {
                return select;
            }

            return $"{select};$expand={string.Join(',', individualExpands)}";
        }

        /// <summary>
        /// Generates a list of all JsonProperty annotations set against a type's properties. Will not include @metadata extensions
        /// Will exclude any nested classes and their annotations
        /// </summary>
        /// <returns>A list of the JsonProperty annotation - excludes any @metadata extensions</returns>
        private List<string> GetBasicProperties(Type type)
        {
            if (type.IsGenericType)
            {
                return type.GenericTypeArguments[0].GetProperties().Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null && IsSystemType(p))
                                .Select(p => ExtractD365PropertyName(p))
                                .ToList();
            }

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

        private List<PropertyInfo> GetTypeProperties(Type type)
        {
            var d365Type = type.IsGenericType ? type.GenericTypeArguments[0] : type;

            var typeProperties = d365Type.GetProperties()
                                     .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null && !IsSystemType(p))
                                     .ToList();

            return typeProperties;
        }

        /// <summary>
        /// Helper method to identify if the field is a system type
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static bool IsSystemType(PropertyInfo p)
        {
            return p.PropertyType.FullName.Contains("System.") && 
                  !p.PropertyType.FullName.Contains("System.Collections.Generic.List") ||
                   p.PropertyType.IsEnum;
        }
    }
}
