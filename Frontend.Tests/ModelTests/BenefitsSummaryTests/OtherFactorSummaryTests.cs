using System.Collections.Generic;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Benefits;
using Helpers;
using Xunit;

namespace Frontend.Tests.ModelTests.BenefitsSummaryTests
{
    public class OtherFactorSummaryTests
    {
        [Fact]
        public void GivenNoOtherFactors_ReturnEmptyList()
        {
            var vm = new BenefitsSummaryViewModel(null, null, new List<OtherFactorsItemViewModel>(), null, null);
            var summary = vm.OtherFactorsSummary();
            Assert.Empty(summary);
        }

        [Fact]
        public void GivenOtherFactorsWithNoDescriptions_ReturnDisplayValues()
        {
            var otherFactors = new List<OtherFactorsItemViewModel>()
            {
                new OtherFactorsItemViewModel
                {
                    OtherFactor = TransferBenefits.OtherFactor.HighProfile,
                },
                new OtherFactorsItemViewModel
                {
                    OtherFactor = TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues,
                },
                new OtherFactorsItemViewModel
                {
                    OtherFactor = TransferBenefits.OtherFactor.FinanceAndDebtConcerns,
                }
            };
            var vm = new BenefitsSummaryViewModel(null, null, otherFactors, null, null);

            var summary = vm.OtherFactorsSummary();

            Assert.Equal(3, summary.Count);
            Assert.Equal(EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(otherFactors[0].OtherFactor),
                summary[0][0]);
            Assert.Null(summary[0][1]);
            Assert.Equal(EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(otherFactors[1].OtherFactor),
                summary[1][0]);
            Assert.Null(summary[1][1]);
            Assert.Equal(EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(otherFactors[2].OtherFactor),
                summary[2][0]);
            Assert.Null(summary[2][1]);
        }
    }
}