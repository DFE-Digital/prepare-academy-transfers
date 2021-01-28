using API.Models.Downstream.D365;

namespace API.ODataHelpers
{
    public interface ID365ModelHelper<T> where T : BaseD365Model
    {
        public D365ModelRepresentation ExtractModelRepresentation();

        public string BuildSelectAndExpandClauses(D365ModelRepresentation representation);
    }
}
