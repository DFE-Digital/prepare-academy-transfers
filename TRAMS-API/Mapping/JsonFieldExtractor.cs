using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace API.Mapping
{
    public static class JsonFieldExtractor
    {
        public static List<string> GetFields(Type modelType)
        {
            var jsonProps = modelType.GetProperties()
                         .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName.Split("@").First())
                         .ToList();

            return jsonProps;
        }
    }
}
