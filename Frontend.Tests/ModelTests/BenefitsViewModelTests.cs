using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.Projects;
using DocumentFormat.OpenXml.Spreadsheet;
using Frontend.Helpers;
using Frontend.Models;
using Helpers;
using Xunit;

namespace Frontend.Tests.ModelTests
{
    // public class BenefitsViewModelTests
    // {
    //     private readonly BenefitsSummaryViewModel _model;
    //
    //     public BenefitsViewModelTests()
    //     {
    //         _model = new BenefitsSummaryViewModel() {Project = new Project()};
    //     }
    //
    //     public class IntendedBenefitSummaryTests : BenefitsViewModelTests
    //     {
    //         [Fact]
    //         public void GivenNoIntendedBenefits_ReturnEmptyList()
    //         {
    //             var summary = _model.IntendedBenefitsSummary();
    //             Assert.Empty(summary);
    //         }
    //
    //         [Fact]
    //         public void GivenListOfIntendedBenefits_ReturnSummary()
    //         {
    //             var intendedBenefits = new List<TransferBenefits.IntendedBenefit>
    //             {
    //                 TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
    //                 TransferBenefits.IntendedBenefit.StrongerLeadership,
    //                 TransferBenefits.IntendedBenefit.ImprovingOfstedRating
    //             };
    //
    //             _model.Project.Benefits.IntendedBenefits = intendedBenefits;
    //
    //             var expectedDisplayValues =
    //                 intendedBenefits.Select(EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue).ToList();
    //
    //             var summary = _model.IntendedBenefitsSummary();
    //
    //             Assert.Equal(expectedDisplayValues[0], summary[0]);
    //             Assert.Equal(expectedDisplayValues[1], summary[1]);
    //             Assert.Equal(expectedDisplayValues[2], summary[2]);
    //         }
    //
    //         [Fact]
    //         public void GivenListOfIntendedBenefitsIncludingOther_FormatOtherSummaryCorrectly()
    //         {
    //             var intendedBenefits = new List<TransferBenefits.IntendedBenefit>
    //             {
    //                 TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
    //                 TransferBenefits.IntendedBenefit.StrongerLeadership,
    //                 TransferBenefits.IntendedBenefit.Other
    //             };
    //
    //             _model.Project.Benefits.IntendedBenefits = intendedBenefits;
    //             _model.Project.Benefits.OtherIntendedBenefit = "Can create cat sanctuary";
    //
    //             var expectedDisplayValues =
    //                 intendedBenefits
    //                     .FindAll(EnumHelpers<TransferBenefits.IntendedBenefit>.HasDisplayValue)
    //                     .Select(EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue).ToList();
    //
    //             expectedDisplayValues.Add("Other: Can create cat sanctuary");
    //
    //             var summary = _model.IntendedBenefitsSummary();
    //
    //             Assert.Equal(expectedDisplayValues[0], summary[0]);
    //             Assert.Equal(expectedDisplayValues[1], summary[1]);
    //             Assert.Equal(expectedDisplayValues[2], summary[2]);
    //         }
    //     }
    //
    //     public class OtherFactorSummaryTests : BenefitsViewModelTests
    //     {
    //         [Fact]
    //         public void GivenNoOtherFactors_ReturnEmptyList()
    //         {
    //             var summary = _model.OtherFactorsSummary();
    //             Assert.Empty(summary);
    //         }
    //
    //         [Fact]
    //         public void GivenOtherFactorsWithNoDescriptions_ReturnDisplayValues()
    //         {
    //             var otherFactors = new Dictionary<TransferBenefits.OtherFactor, string>()
    //             {
    //                 {TransferBenefits.OtherFactor.HighProfile, ""},
    //                 {TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues, ""},
    //                 {TransferBenefits.OtherFactor.FinanceAndDebtConcerns, ""}
    //             };
    //
    //             _model.Project.Benefits.OtherFactors = otherFactors;
    //
    //             var expectedSummary = otherFactors.Keys
    //                 .Select(EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue)
    //                 .Select(value => new[] {value, ""})
    //                 .ToList();
    //
    //             var summary = _model.OtherFactorsSummary();
    //
    //             Assert.Equal(3, summary.Count);
    //             Assert.Equal(expectedSummary[0][0], summary[0][0]);
    //             Assert.Equal("", summary[0][1]);
    //             Assert.Equal(expectedSummary[1][0], summary[1][0]);
    //             Assert.Equal("", summary[1][1]);
    //             Assert.Equal(expectedSummary[2][0], summary[2][0]);
    //             Assert.Equal("", summary[2][1]);
    //         }
    //     }
    // }
}