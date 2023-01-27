using System.Collections.Generic;
using Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Benefits;
using Helpers;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ModelTests.BenefitsSummaryTests
{
    public class OtherFactorSummaryTests
    {
        [Fact]
        public void GivenNoOtherFactors_ReturnEmptyList()
        {
            var vm = new BenefitsSummaryViewModel(null, null, new List<OtherFactorsItemViewModel>(), null, null);
            Assert.Empty(vm.OtherFactorsItems);
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

            var summary = vm.OtherFactorsItems;

            Assert.Equal(3, summary.Count);
            Assert.Equal(EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(otherFactors[0].OtherFactor),
                EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(summary[0].OtherFactor));
            Assert.Null(summary[0].Description);
            Assert.Equal(EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(otherFactors[1].OtherFactor),
                EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(summary[1].OtherFactor));
            Assert.Null(summary[1].Description);
            Assert.Equal(EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(otherFactors[2].OtherFactor),
                EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(summary[2].OtherFactor));
            Assert.Null(summary[2].Description);
        }
    }
}