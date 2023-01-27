using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.TRAMS.Models;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests.TestFixtures
{
    public class Trusts
    {
        public static TramsTrust GetSingleTrust()
        {
            return new TramsTrust
            {
                Establishments = new List<TramsEstablishment>(),
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