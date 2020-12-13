using API.Models.D365;
using System.Collections.Generic;

namespace API.Mapping
{
    public interface IODataModelHelper<T> where T : BaseD365Model
    {
        List<string> GetSelectClause();

        string GetExpandClause();

        string GetPropertyAnnotation(string propertyName);
    }
}
