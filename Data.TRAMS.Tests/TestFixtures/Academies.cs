using Data.TRAMS.Models;

namespace Data.TRAMS.Tests.TestFixtures
{
    public class Academies
    {
        public static TramsAcademy SingleAcademy()
        {
            return new TramsAcademy
            {
                Ukprn = "12345",
                EstablishmentName = "Academy name"
            };
        }
    }
}