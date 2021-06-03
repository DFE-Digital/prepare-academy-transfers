using Data.TRAMS.Models;

namespace Data.TRAMS.Tests.TestFixtures
{
    public static class Establishments
    {
        public static TramsEstablishment SingleEstablishment()
        {
            return new TramsEstablishment
            {
                Ukprn = "12345",
                EstablishmentName = "Academy name"
            };
        }
    }
}