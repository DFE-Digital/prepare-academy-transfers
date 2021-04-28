using System.Collections.Generic;
using Data.TRAMS.Models;

namespace Data.TRAMS.Tests.TestFixtures
{
    public class Trusts
    {
        public static TramsTrust GetSingleTrust()
        {
            return new TramsTrust
            {
                Academies = new List<TramsAcademy>(),
                GiasData = new TramsTrustGiasData
                {
                    Ukprn = "00001",
                    CompaniesHouseNumber = "1234567",
                    GroupContactAddress = new GroupContactAddress(),
                    GroupId = "123",
                    GroupName = "Group Name"
                }
            };
        }
    }
}