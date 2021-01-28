using API.Models.Downstream.D365;
using System;
using System.Collections.Generic;

namespace API.HttpHelpers
{
    public interface IOdataUrlBuilder<T> where T : BaseD365Model
    {
        public string BuildFilterUrl(string route, List<string> filters);

        public string BuildRetrieveOneUrl(string route, Guid id);

        public string BuildInFilter(string fieldName, List<string> allowedValues);

        public string BuildOrSearchQuery(string query, List<string> fieldNames);

        public string GetPropertyAnnotation(string propertyName);
    }
}
