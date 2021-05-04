using Data.Models;
using Data.TRAMS.Models;

namespace Data.TRAMS.Mappers.Response
{
    public class TramsTrustMapper : IMapper<TramsTrust, Trust>
    {
        public Trust Map(TramsTrust input)
        {
            return new Trust
            {
                Ukprn = input.GiasData.Ukprn,
                Name = input.GiasData.GroupName
            };
        }
    }
}