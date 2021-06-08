using System.Collections.Generic;
using Data.Models;
using Data.Models.Projects;

namespace Data.TRAMS.Tests.TestFixtures
{
    public static class Projects
    {
        public static Project CompletedProject()
        {
            return new Project
            {
                Benefits = new TransferBenefits
                {
                    IntendedBenefits = new List<TransferBenefits.IntendedBenefit>
                    {
                        TransferBenefits.IntendedBenefit.ImprovingOfstedRating,
                        TransferBenefits.IntendedBenefit.Other
                    },
                    OtherIntendedBenefit = "Other intended benefit",
                    OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
                    {
                        {TransferBenefits.OtherFactor.HighProfile, "High profile"}
                    }
                },
                Dates = new TransferDates(),
                Features = new TransferFeatures(),
                Name = "Project name",
                Rationale = new TransferRationale(),
                State = "State",
                Status = "Status",
                Urn = "12345",
                TransferringAcademies = new List<TransferringAcademies>(),
                OutgoingTrustName = "Outgoing trust name",
                OutgoingTrustUkprn = "Outgoing trust Ukprn"
            };
        }
    }
}