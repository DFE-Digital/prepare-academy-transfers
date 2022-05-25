using Data.Models.Academies;
using FluentAssertions;
using Xunit;

namespace Data.Tests.Models
{
    public class LatestOfstedJudgementTests
    {
        [Theory]
        [InlineData("No data", false)]
        [InlineData("N/A", false)]
        [InlineData("Outstanding", true)]
        public void EarlyYearsProvisionApplicable_ReturnsCorrectly(string earlyYearsProvision, bool expectedApplicableValue)
        {
            var latestOfstedJudgement = new LatestOfstedJudgement
            {
                EarlyYearsProvision = earlyYearsProvision
            };

            latestOfstedJudgement.EarlyYearsProvisionApplicable.Should().Be(expectedApplicableValue);
        }

        [Theory]
        [InlineData("No data", false)]
        [InlineData("N/A", false)]
        [InlineData("Outstanding", true)]
        public void SixthFormProvisionApplicable_ReturnsCorrectly(string sixthFormProvision, bool expectedApplicableValue)
        {
            var latestOfstedJudgement = new LatestOfstedJudgement
            {
                SixthFormProvision = sixthFormProvision
            };

            latestOfstedJudgement.SixthFormProvisionApplicable.Should().Be(expectedApplicableValue);
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData(null, "12/05/2016", false)]
        [InlineData("12/05/2016", null, true)]
        [InlineData("12/05/2016", "12/05/2016", false)]
        [InlineData("11/05/2016", "12/05/2016", false)]
        [InlineData("12/05/2016", "11/05/2016", true)]
        public void LatestInspectionIsSection8_ReturnsCorrectly(string latestSection8, string latestFull, bool expectedValue)
        {
            var latestOfstedJudgement = new LatestOfstedJudgement
            {
                DateOfLatestSection8Inspection = latestSection8,
                InspectionEndDate = latestFull
            };

            latestOfstedJudgement.LatestInspectionIsSection8.Should().Be(expectedValue);
        }
    }
}
