using System.Collections.Generic;

namespace API.ODataHelpers
{
    public class D365ModelRepresentation
    {
        public string RootExpandName { get; set; }
        public List<string> BaseProperties { get; set; }

        public List<D365ModelRepresentation> ExpandProperties { get; set; } = new List<D365ModelRepresentation>();
    }
}
