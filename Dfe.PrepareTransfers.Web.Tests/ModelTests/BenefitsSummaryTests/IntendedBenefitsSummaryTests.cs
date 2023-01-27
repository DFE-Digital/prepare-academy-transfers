using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Benefits;
using Helpers;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ModelTests.BenefitsSummaryTests
{
    public class IntendedBenefitsSummaryTests
    {
        [Fact]
        public void GivenNoIntendedBenefits_ReturnEmptyList()
        {
            var vm = new BenefitsSummaryViewModel(new List<TransferBenefits.IntendedBenefit>(), null, null, null, null);
            var summary = vm.IntendedBenefitsSummary();
            Assert.Empty(summary);
        }

        [Fact]
        public void GivenListOfIntendedBenefits_ReturnSummary()
        {
            var intendedBenefits = new List<TransferBenefits.IntendedBenefit>
            {
                TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
                TransferBenefits.IntendedBenefit.StrongerLeadership,
                TransferBenefits.IntendedBenefit.ImprovingOfstedRating
            };
            var vm = new BenefitsSummaryViewModel(intendedBenefits, null, null, null, null);

            var expectedDisplayValues =
                intendedBenefits.Select(EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue).ToList();

            var summary = vm.IntendedBenefitsSummary();

            Assert.Equal(expectedDisplayValues[0], summary[0]);
            Assert.Equal(expectedDisplayValues[1], summary[1]);
            Assert.Equal(expectedDisplayValues[2], summary[2]);
        }

        [Fact]
        public void GivenListOfIntendedBenefitsIncludingOther_FormatOtherSummaryCorrectly()
        {
            var intendedBenefits = new List<TransferBenefits.IntendedBenefit>
            {
                TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
                TransferBenefits.IntendedBenefit.StrongerLeadership,
                TransferBenefits.IntendedBenefit.Other
            };
            var vm = new BenefitsSummaryViewModel(intendedBenefits, "Can create cat sanctuary", null, null, null);

            var expectedDisplayValues =
                intendedBenefits
                    .FindAll(EnumHelpers<TransferBenefits.IntendedBenefit>.HasDisplayValue)
                    .Select(EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue).ToList();

            expectedDisplayValues.Add("Other: Can create cat sanctuary");

            var summary = vm.IntendedBenefitsSummary();

            Assert.Equal(expectedDisplayValues[0], summary[0]);
            Assert.Equal(expectedDisplayValues[1], summary[1]);
            Assert.Equal(expectedDisplayValues[2], summary[2]);
        }
    }
}