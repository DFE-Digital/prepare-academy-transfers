using System.Collections.Generic;
using Dfe.Academies.Contracts.V4;
using Dfe.Academies.Contracts.V4.Trusts;
using Dfe.PrepareTransfers.Data.TRAMS.Models;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests.TestFixtures
{
    public class Trusts
    {
        public static TrustDto GetSingleTrust()
        {
            return new TrustDto
            {
                    Ukprn = "00001",
                    CompaniesHouseNumber = "1234567",
                    Address = new AddressDto(),
                    ReferenceNumber = "123",
                    Name = "Group Name"
            };
        }
    }
}