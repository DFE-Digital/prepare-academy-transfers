using Dfe.PrepareTransfers.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.HelpersTests
{
    public class KeyStage4DataTagHelperTests
    {
        [Theory, MemberData(nameof(ProvisionalDates))]
        public void Should_return_provisional_status_on_relevant_months(DateTime date)
        {
            var resultingHtml = KeyStage4DataStatusHelper.KeyStageDataTag(date);
            var result = KeyStage4DataStatusHelper.DetermineKeyStageDataStatus(date);
            resultingHtml.Should().Contain("grey").And.Contain("Provisional");
            result.Should().Be("Provisional");
        }

        [Theory, MemberData(nameof(RevisedDates))]
        public void Should_return_revised_status_on_relevant_months(DateTime date)
        {
            var resultingHtml = KeyStage4DataStatusHelper.KeyStageDataTag(date);
            var result = KeyStage4DataStatusHelper.DetermineKeyStageDataStatus(date);
            resultingHtml.Should().Contain("orange").And.Contain("Revised");
            result.Should().Be("Revised");
        }
        [Theory, MemberData(nameof(FinalDates))]
        public void Should_return_final_status_on_relevant_months(DateTime date)
        {
            var resultingHtml = KeyStage4DataStatusHelper.KeyStageDataTag(date);
            var result = KeyStage4DataStatusHelper.DetermineKeyStageDataStatus(date);
            resultingHtml.Should().Contain("green").And.Contain("Final");
            result.Should().Be("Final");
        }
        public static IEnumerable<object[]> ProvisionalDates()
        {
            yield return new object[] { new DateTime(DateTime.Now.Year - 1, 9, 3) };
            yield return new object[] { new DateTime(DateTime.Now.Year - 1, 10, 11) };
            yield return new object[] { new DateTime(DateTime.Now.Year - 1, 11, 21) };
            yield return new object[] { new DateTime(DateTime.Now.Year - 1, 12, 14) };
        }
        public static IEnumerable<object[]> RevisedDates()
        {
            yield return new object[] { new DateTime(DateTime.Now.Year, 1, 3) };
            yield return new object[] { new DateTime(DateTime.Now.Year, 2, 11) };
            yield return new object[] { new DateTime(DateTime.Now.Year, 3, 21) };
            yield return new object[] { new DateTime(DateTime.Now.Year, 4, 14) };
        }
        public static IEnumerable<object[]> FinalDates()
        {
            yield return new object[] { new DateTime(DateTime.Now.Year - 2, 1, 3) };
            yield return new object[] { new DateTime(DateTime.Now.Year - 2, 2, 11) };
            yield return new object[] { new DateTime(DateTime.Now.Year, 5, 21) };
            yield return new object[] { new DateTime(DateTime.Now.Year, 6, 14) };
        }
    }
}
