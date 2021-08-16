using System.Collections.Generic;
using System.Linq;
using Data.Models.KeyStagePerformance;
using Frontend.Helpers;
using Microsoft.AspNetCore.Html;
using Xunit;

namespace Frontend.Tests.HelpersTests
{
    public class PerformanceDataHelpersTests
    {
        public class GetFormattedResultTests
        {
            [Theory]
            [InlineData(null, "no data")]
            [InlineData("", "no data")]
            [InlineData("3.45", "3.45")]
            [InlineData("3.00", "3")]
            [InlineData("3", "3")]
            [InlineData("some text", "some text")]
            public void GivenUnformattedValue_ReturnsFormattedValue(string result, string expectedResult)
            {
                var formattedResult = PerformanceDataHelpers.GetFormattedResult(result);

                Assert.Equal(expectedResult, formattedResult);
            }

            [Theory]
            [InlineData(null, null, "no data")]
            [InlineData("", "", "no data")]
            [InlineData("3.45", null, "3.45<br>(disadvantaged no data)")]
            [InlineData("3.45", "2.44", "3.45<br>(disadvantaged 2.44)")]
            [InlineData(null, "3.44", "no data<br>(disadvantaged 3.44)")]
            public void GivenUnformattedDisadvantagedPupilResult_ReturnsFormattedHtmlValue(
                string nonDisadvantagePupilResult, string disadvantagedPupilResult, string expectedResult)
            {
                var pupilResult = new DisadvantagedPupilsResult
                    {NotDisadvantaged = nonDisadvantagePupilResult, Disadvantaged = disadvantagedPupilResult};

                var expectedResultAsHtml = new HtmlString(expectedResult);

                var formattedResult = PerformanceDataHelpers.GetFormattedHtmlResult(pupilResult);

                Assert.Equal(expectedResultAsHtml.ToString(), formattedResult.ToString());
            }

            [Theory]
            [InlineData(null, null, "no data")]
            [InlineData("", "", "no data")]
            [InlineData("3.45", null, "3.45\n(disadvantaged no data)")]
            [InlineData("3.45", "2.44", "3.45\n(disadvantaged 2.44)")]
            [InlineData(null, "3.44", "no data\n(disadvantaged 3.44)")]
            public void GivenUnformattedDisadvantagedPupilResult_ReturnsFormattedValue(
                string nonDisadvantagePupilResult, string disadvantagedPupilResult, string expectedResult)
            {
                var pupilResult = new DisadvantagedPupilsResult
                    {NotDisadvantaged = nonDisadvantagePupilResult, Disadvantaged = disadvantagedPupilResult};

                var formattedResult = PerformanceDataHelpers.GetFormattedStringResult(pupilResult);

                Assert.Equal(expectedResult, formattedResult.ToString());
            }
        }

        public class GetFormattedConfidenceIntervalTests
        {
            [Fact]
            public void GivenConfidenceIntervals_ShouldFormatCorrectly()
            {
                var result = PerformanceDataHelpers.GetFormattedConfidenceInterval(1.2M, 2.4M);

                Assert.Equal("1.2 to 2.4", result);
            }

            [Fact]
            public void GivenNullConfidenceIntervals_ShouldReturnNoData()
            {
                var result = PerformanceDataHelpers.GetFormattedConfidenceInterval(null, null);

                Assert.Equal("no data", result);
            }
        }

        public class GetFormattedYearTests
        {
            [Theory]
            [InlineData(null, null)]
            [InlineData("", "")]
            [InlineData("2018-2019", "2018 to 2019")]
            [InlineData("2018 - 2019", "2018 to 2019")]
            [InlineData("randomness", "randomness")]
            public void GivenYear_ShouldFormatCorrectly(string unformattedYear, string expectedFormattedYear)
            {
                var result = PerformanceDataHelpers.GetFormattedYear(unformattedYear);

                Assert.Equal(expectedFormattedYear, result);
            }
        }

        public class HasKeyStage2PerformanceDataTests
        {
            [Fact]
            public void GivenNull_ReturnFalse()
            {
                Assert.False(PerformanceDataHelpers.HasKeyStage2PerformanceInformation(null));
            }

            [Fact]
            public void GivenEmptyList_ReturnFalse()
            {
                var model = new List<KeyStage2>();
                Assert.False(PerformanceDataHelpers.HasKeyStage2PerformanceInformation(model));
            }

            [Theory]
            [InlineData("12.5", null, null, null, null, null, null, null, null, null)]
            [InlineData(null, "12.5", null, null, null, null, null, null, null, null)]
            [InlineData(null, null, "12.5", null, null, null, null, null, null, null)]
            [InlineData(null, null, null, "12.5", null, null, null, null, null, null)]
            [InlineData(null, null, null, null, "12.5", null, null, null, null, null)]
            [InlineData(null, null, null, null, null, "12.5", null, null, null, null)]
            [InlineData(null, null, null, null, null, null, "12.5", null, null, null)]
            [InlineData(null, null, null, null, null, null, null, "12.5", null, null)]
            [InlineData(null, null, null, null, null, null, null, null, "12.5", null)]
            [InlineData(null, null, null, null, null, null, null, null, null, "12.5")]
            public void GivenAnyEducationPerformanceData_ReturnTrue(
                string mathsDisadvantaged, string mathsNonDisadvantaged,
                string readDisadvantaged, string readNonDisadvantaged,
                string writeDisadvantaged, string writeNonDisadvantaged,
                string rwmExpectedDisadvantaged, string rwmExpectedNonDisadvantaged,
                string rwmHigherDisadvantaged, string rwmHigherNonDisadvantaged)
            {
                var model = new List<KeyStage2>
                {
                    new KeyStage2
                    {
                        MathsProgressScore = new DisadvantagedPupilsResult
                            {Disadvantaged = mathsDisadvantaged, NotDisadvantaged = mathsNonDisadvantaged},
                        ReadingProgressScore = new DisadvantagedPupilsResult
                            {Disadvantaged = readDisadvantaged, NotDisadvantaged = readNonDisadvantaged},
                        WritingProgressScore = new DisadvantagedPupilsResult
                            {Disadvantaged = writeDisadvantaged, NotDisadvantaged = writeNonDisadvantaged},
                        PercentageMeetingExpectedStdInRWM = new DisadvantagedPupilsResult
                        {
                            Disadvantaged = rwmExpectedDisadvantaged,
                            NotDisadvantaged = rwmExpectedNonDisadvantaged
                        },
                        PercentageAchievingHigherStdInRWM = new DisadvantagedPupilsResult
                        {
                            Disadvantaged = rwmHigherDisadvantaged, NotDisadvantaged = rwmHigherNonDisadvantaged
                        }
                    }
                };
                Assert.True(PerformanceDataHelpers.HasKeyStage2PerformanceInformation(model));
            }
        }

        public class HasKeyStage4PerformanceDataTests
        {
            [Fact]
            public void GivenNull_ReturnFalse()
            {
                Assert.False(PerformanceDataHelpers.HasKeyStage4PerformanceInformation(null));
            }

            [Fact]
            public void GivenEmptyList_ReturnFalse()
            {
                var model = new List<KeyStage4>();
                Assert.False(PerformanceDataHelpers.HasKeyStage4PerformanceInformation(model));
            }

            [Fact]
            public void GivenAnyEducationPerformanceData_ReturnTrue()
            {
                var model = new List<KeyStage4>
                {
                    new KeyStage4()
                };

                var keyStage4PropertiesToTest = new[]
                {
                    "SipAttainment8score", "SipAttainment8scoreebacc",
                    "SipAttainment8scoreenglish", "SipAttainment8scoremaths", "SipAttainment8score",
                    "SipProgress8ebacc",
                    "SipProgress8english", "SipProgress8maths", "SipProgress8Score", "SipNumberofpupilsprogress8"
                };
                var keyStage4Performance = model[0];
                var keyStage4Properties = keyStage4Performance.GetType().GetProperties()
                    .Where(n => keyStage4PropertiesToTest.Contains(n.Name)).ToList();

                foreach (var property in keyStage4Properties)
                {
                    SetAllToNull();

                    var disadvantagedPupilResult = new DisadvantagedPupilsResult
                    {
                        Disadvantaged = "10.45",
                        NotDisadvantaged = null
                    };
                    property.SetValue(keyStage4Performance, disadvantagedPupilResult);
                    Assert.True(PerformanceDataHelpers.HasKeyStage4PerformanceInformation(model));

                    var notDisadvantagedPupilResult = new DisadvantagedPupilsResult
                    {
                        Disadvantaged = null,
                        NotDisadvantaged = "10.45"
                    };
                    property.SetValue(keyStage4Performance, notDisadvantagedPupilResult);
                    Assert.True(PerformanceDataHelpers.HasKeyStage4PerformanceInformation(model));
                }

                void SetAllToNull() => keyStage4Properties
                    .ForEach(p => p.SetValue(keyStage4Performance, new DisadvantagedPupilsResult()));
            }
        }

        public class HasKeyStage5PerformanceDataTests
        {
            [Fact]
            public void GivenNull_ReturnFalse()
            {
                Assert.False(PerformanceDataHelpers.HasKeyStage5PerformanceInformation(null));
            }

            [Fact]
            public void GivenEmptyList_ReturnFalse()
            {
                var model = new List<KeyStage5>();
                Assert.False(PerformanceDataHelpers.HasKeyStage5PerformanceInformation(model));
            }

            [Fact]
            public void GivenListWithItemWithNoAcademy_ReturnFalse()
            {
                var model = new List<KeyStage5> {new KeyStage5()};

                Assert.False(PerformanceDataHelpers.HasKeyStage5PerformanceInformation(model));
            }

            [Theory]
            [InlineData("12.12", null)]
            [InlineData("12.12", "")]
            [InlineData(null, "12.12")]
            [InlineData("", "12.12")]
            [InlineData("12.12", "12.12")]
            public void GivenListWithItemWithAcademyWithData_ReturnTrue(string academicAverage,
                string appliedGeneralAverage)
            {
                var model = new List<KeyStage5>
                {
                    new KeyStage5
                    {
                        Academy = new KeyStage5Result
                        {
                            AcademicAverage = academicAverage,
                            AppliedGeneralAverage = appliedGeneralAverage
                        }
                    }
                };
                Assert.True(PerformanceDataHelpers.HasKeyStage5PerformanceInformation(model));
            }
        }

        public class GetKeyStage4ResultsTests
        {
            [Theory]
            [InlineData("2018-2019", "2017-2018", "2016-2017")]
            [InlineData("2017-2018", "2016-2017", "2018-2019")]
            [InlineData("2016-2017", "2017-2018", "2018-2019")]
            [InlineData("2018-2019", "2017-2018", null)]
            [InlineData(null, "2018-2019", "2017-2018")]
            [InlineData("2018-2019", null, "2017-2018")]
            [InlineData("2018-2019", null, null)]
            [InlineData(null, "2018-2019", null)]
            [InlineData(null, null, "2018-2019")]
            public void GiveDataWithMissingYears_ShouldReturnCorrectMissingYearDataSet(string year1, string year2, string year3)
            {
                var ks4Results = new List<KeyStage4>();
                if (year1 != null)
                    ks4Results.Add(new KeyStage4 { Year = year1 });
                if (year2 != null)
                    ks4Results.Add(new KeyStage4 { Year = year2 });
                if (year3 != null)
                    ks4Results.Add(new KeyStage4 { Year = year3 });

                var result = PerformanceDataHelpers.GetKeyStage4Results(ks4Results);
                
                Assert.Equal(3,result.Count);
                Assert.Equal("2018-2019", result[0].Year);
                Assert.Equal("2017-2018", result[1].Year);
                Assert.Equal("2016-2017", result[2].Year);
            }

            [Fact]
            public void GivenNoYearData_ShouldNotErrorAndReturnEmptyData()
            {
                var result = PerformanceDataHelpers.GetKeyStage4Results(new List<KeyStage4>());
                
                Assert.Equal(3,result.Count);
                Assert.All(result, ks4Result => Assert.Null(ks4Result.Year));
            }
        }
    }
}