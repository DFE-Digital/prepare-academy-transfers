using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Data.Models.KeyStagePerformance;
using DocumentGeneration;
using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces;
using Frontend.ExtensionMethods;
using Frontend.Helpers;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;

namespace Frontend.Services
{
    public class CreateHtbDocument : ICreateHtbDocument
    {
        private readonly IGetHtbDocumentForProject _getHtbDocumentForProject;

        public CreateHtbDocument(IGetHtbDocumentForProject getHtbDocumentForProject)
        {
            _getHtbDocumentForProject = getHtbDocumentForProject;
        }

        private MemoryStream CreateMemoryStream(string template)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.Contains(template, StringComparison.OrdinalIgnoreCase));
            using var templateStream = assembly.GetManifestResourceStream(resourceName);
            var ms = new MemoryStream();
            templateStream.CopyTo(ms);
            return ms;
        }

        public async Task<CreateHtbDocumentResponse> Execute(string projectUrn)
        {
            var getHtbDocumentForProject = await _getHtbDocumentForProject.Execute(projectUrn);
            if (!getHtbDocumentForProject.IsValid)
            {
                return CreateErrorResponse(getHtbDocumentForProject.ResponseError);
            }

            var htbDocument = getHtbDocumentForProject.HtbDocument;

            var ms = CreateMemoryStream("htb-template");
            var builder = DocumentBuilder.CreateFromTemplate(ms, htbDocument);

            BuildTitle(builder, htbDocument);
            BuildKeyStage2PerformanceInformation(builder, htbDocument);
            BuildKeyStage4PerformanceInformation(builder, htbDocument);
            BuildKeyStage5PerformanceInformation(builder, htbDocument);

            return new CreateHtbDocumentResponse
            {
                Document = builder.Build()
            };
        }

        private static void BuildTitle(IDocumentBuilder documentBuilder, HtbDocument htbDocument)
        {
            documentBuilder.ReplacePlaceholderWithContent("HtbTemplateTitle", builder =>
            {
                builder.AddHeading(hBuilder =>
                {
                    hBuilder.SetHeadingLevel(HeadingLevel.One);
                    hBuilder.AddText(new TextElement
                    {
                        Value = "Headteacher board (HTB) template for:\n" +
                                $"{htbDocument.SchoolName} - URN {htbDocument.SchoolUrn}\n \n" +
                                $"Outgoing trust - {htbDocument.TrustName.ToTitleCase()} ({htbDocument.TrustReferenceNumber})",
                        Bold = true
                    });
                });
            });
        }

        private static void BuildKeyStage2PerformanceInformation(IDocumentBuilder documentBuilder,
            HtbDocument htbDocument)
        {
            if (!PerformanceDataHelpers.HasKeyStage2PerformanceInformation(htbDocument.KeyStage2Performance))
            {
                documentBuilder.ReplacePlaceholderWithContent("KeyStage2PerformanceSection",
                    b => { b.AddParagraph(string.Empty); });
            }
            else
            {
                documentBuilder.ReplacePlaceholderWithContent("KeyStage2PerformanceSection", builder =>
                {
                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.One);
                        hBuilder.AddText(new TextElement {Value = "Key stage 2 performance tables (KS2)", Bold = true});
                    });

                    foreach (var ks2Result in htbDocument.KeyStage2Performance.OrderByDescending(k => k.Year))
                    {
                        builder.AddHeading(hBuilder =>
                        {
                            hBuilder.SetHeadingLevel(HeadingLevel.Two);
                            hBuilder.AddText($"{PerformanceDataHelpers.GetFormattedYear(ks2Result.Year)} Key stage 2");
                        });

                        builder.AddTable(new[]
                        {
                            new[]
                            {
                                new TextElement {Value = "", Bold = true},
                                new TextElement
                                {
                                    Value = "Percentage meeting expecting standard in reading, writing and maths",
                                    Bold = true
                                },
                                new TextElement
                                {
                                    Value = "Percentage achieving a higher standard in reading, writing and maths",
                                    Bold = true
                                },
                                new TextElement {Value = "Reading progress scores", Bold = true},
                                new TextElement {Value = "Writing progress scores", Bold = true},
                                new TextElement {Value = "Maths progress scores", Bold = true}
                            },

                            new[]
                            {
                                new TextElement {Value = htbDocument.SchoolName, Bold = true},
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.PercentageMeetingExpectedStdInRWM)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.PercentageAchievingHigherStdInRWM)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.ReadingProgressScore)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.WritingProgressScore)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.MathsProgressScore)}"
                                },
                            },
                            new[]
                            {
                                new TextElement {Value = $"{htbDocument.LocalAuthorityName} LA average", Bold = true},
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.LAAveragePercentageMeetingExpectedStdInRWM)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.LAAveragePercentageAchievingHigherStdInRWM)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.LAAverageReadingProgressScore)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.LAAverageWritingProgressScore)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.LAAverageMathsProgressScore)}"
                                },
                            },
                            new[]
                            {
                                new TextElement {Value = "National average", Bold = true},
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.NationalAveragePercentageMeetingExpectedStdInRWM)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.NationalAveragePercentageAchievingHigherStdInRWM)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.NationalAverageReadingProgressScore)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.NationalAverageWritingProgressScore)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedStringResult(ks2Result.NationalAverageMathsProgressScore)}"
                                },
                            }
                        });
                    }

                    builder.AddParagraph(pBuilder => pBuilder.AddText(""));

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "Additional information", Bold = true},
                            new TextElement {Value = htbDocument.KeyStage2AdditionalInformation}
                        }
                    });
                });
            }
        }

        private static void BuildKeyStage4PerformanceInformation(IDocumentBuilder documentBuilder,
            HtbDocument htbDocument)
        {
            if (!PerformanceDataHelpers.HasKeyStage4PerformanceInformation(htbDocument.KeyStage4Performance))
            {
                documentBuilder.ReplacePlaceholderWithContent("KeyStage4PerformanceSection",
                    b => { b.AddParagraph(string.Empty); });
            }
            else
            {
                var ks4Results = htbDocument.KeyStage4Performance.Select(c =>
                {
                    if (c.Year != null)
                    {
                        c.Year = PerformanceDataHelpers.GetFormattedYear(c.Year);
                    }

                    return c;
                }).ToList();

                documentBuilder.ReplacePlaceholderWithContent("KeyStage4PerformanceSection", builder =>
                {
                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.One);
                        hBuilder.AddText(new TextElement {Value = "Key stage 4 performance tables (KS4)", Bold = true});
                    });

                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.Two);
                        hBuilder.AddText("Attainment 8");
                    });

                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.Two);
                        hBuilder.AddText("Attainment 8 Scores");
                    });

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "", Bold = true},
                            new TextElement {Value = ks4Results[0].Year, Bold = true},
                            new TextElement {Value = ks4Results[1].Year, Bold = true},
                            new TextElement {Value = ks4Results[2].Year, Bold = true}
                        },
                        new[]
                        {
                            new TextElement {Value = htbDocument.SchoolName, Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0]
                                    .SipAttainment8score)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1]
                                    .SipAttainment8score)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2]
                                    .SipAttainment8score)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = $"{htbDocument.LocalAuthorityName} LA Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0].LAAverageA8Score)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1].LAAverageA8Score)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2].LAAverageA8Score)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = "National Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[0].NationalAverageA8Score)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[1].NationalAverageA8Score)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[2].NationalAverageA8Score)
                            }
                        }
                    });

                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.Two);
                        hBuilder.AddText("Attainment 8 English");
                    });

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "", Bold = true},
                            new TextElement {Value = ks4Results[0].Year, Bold = true},
                            new TextElement {Value = ks4Results[1].Year, Bold = true},
                            new TextElement {Value = ks4Results[2].Year, Bold = true}
                        },
                        new[]
                        {
                            new TextElement {Value = htbDocument.SchoolName, Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0]
                                    .SipAttainment8scoreenglish)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1]
                                    .SipAttainment8scoreenglish)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2]
                                    .SipAttainment8scoreenglish)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = $"{htbDocument.LocalAuthorityName} LA Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[0].LAAverageA8English)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[1].LAAverageA8English)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[2].LAAverageA8English)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = "National Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0]
                                    .NationalAverageA8English)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1]
                                    .NationalAverageA8English)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2]
                                    .NationalAverageA8English)
                            }
                        }
                    });

                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.Two);
                        hBuilder.AddText("Attainment 8 Maths");
                    });

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "", Bold = true},
                            new TextElement {Value = ks4Results[0].Year, Bold = true},
                            new TextElement {Value = ks4Results[1].Year, Bold = true},
                            new TextElement {Value = ks4Results[2].Year, Bold = true}
                        },
                        new[]
                        {
                            new TextElement {Value = htbDocument.SchoolName, Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0]
                                    .SipAttainment8scoremaths)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1]
                                    .SipAttainment8scoremaths)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2]
                                    .SipAttainment8scoremaths)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = $"{htbDocument.LocalAuthorityName} LA Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0].LAAverageA8Maths)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1].LAAverageA8Maths)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2].LAAverageA8Maths)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = "National Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[0].NationalAverageA8Maths)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[1].NationalAverageA8Maths)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[2].NationalAverageA8Maths)
                            }
                        }
                    });

                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.Two);
                        hBuilder.AddText("Attainment 8 EBacc");
                    });

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "", Bold = true},
                            new TextElement {Value = ks4Results[0].Year, Bold = true},
                            new TextElement {Value = ks4Results[1].Year, Bold = true},
                            new TextElement {Value = ks4Results[2].Year, Bold = true}
                        },
                        new[]
                        {
                            new TextElement {Value = htbDocument.SchoolName, Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0]
                                    .SipAttainment8scoreebacc)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1]
                                    .SipAttainment8scoreebacc)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2]
                                    .SipAttainment8scoreebacc)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = $"{htbDocument.LocalAuthorityName} LA Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0].LAAverageA8EBacc)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1].LAAverageA8EBacc)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2].LAAverageA8EBacc)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = "National Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[0].NationalAverageA8EBacc)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[1].NationalAverageA8EBacc)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[2].NationalAverageA8EBacc)
                            }
                        }
                    });

                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.Two);
                        hBuilder.AddText("Progress 8");
                    });

                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.Two);
                        hBuilder.AddText("Pupils included in P8");
                    });

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "", Bold = true},
                            new TextElement {Value = ks4Results[0].Year, Bold = true},
                            new TextElement {Value = ks4Results[1].Year, Bold = true},
                            new TextElement {Value = ks4Results[2].Year, Bold = true}
                        },
                        new[]
                        {
                            new TextElement {Value = htbDocument.SchoolName, Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0]
                                    .SipNumberofpupilsprogress8)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1]
                                    .SipNumberofpupilsprogress8)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2]
                                    .SipNumberofpupilsprogress8)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = $"{htbDocument.LocalAuthorityName} LA Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0]
                                    .LAAverageP8PupilsIncluded)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1]
                                    .LAAverageP8PupilsIncluded)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2]
                                    .LAAverageP8PupilsIncluded)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = "National Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0]
                                    .NationalAverageP8PupilsIncluded)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1]
                                    .NationalAverageP8PupilsIncluded)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2]
                                    .NationalAverageP8PupilsIncluded)
                            }
                        }
                    });

                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.Two);
                        hBuilder.AddText("School progress score");
                    });

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "", Bold = true},
                            new TextElement {Value = ks4Results[0].Year, Bold = true},
                            new TextElement {Value = ks4Results[1].Year, Bold = true},
                            new TextElement {Value = ks4Results[2].Year, Bold = true}
                        },
                        new[]
                        {
                            new TextElement {Value = htbDocument.SchoolName, Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0].SipProgress8Score)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1].SipProgress8Score)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2].SipProgress8Score)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = "School confidence interval", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedConfidenceInterval(
                                    ks4Results[0].SipProgress8lowerconfidence,
                                    ks4Results[0].SipProgress8upperconfidence)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedConfidenceInterval(
                                    ks4Results[1].SipProgress8lowerconfidence,
                                    ks4Results[1].SipProgress8upperconfidence)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedConfidenceInterval(
                                    ks4Results[2].SipProgress8lowerconfidence,
                                    ks4Results[2].SipProgress8upperconfidence)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = $"{htbDocument.LocalAuthorityName} LA Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0].LAAverageP8Score)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1].LAAverageP8Score)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2].LAAverageP8Score)
                            }
                        },
                        new[]
                        {
                            new TextElement
                                {Value = $"{htbDocument.LocalAuthorityName} LA confidence interval", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedConfidenceInterval(
                                    ks4Results[0].LAAverageP8LowerConfidence, ks4Results[0].LAAverageP8UpperConfidence)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedConfidenceInterval(
                                    ks4Results[1].LAAverageP8LowerConfidence, ks4Results[1].LAAverageP8UpperConfidence)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedConfidenceInterval(
                                    ks4Results[2].LAAverageP8LowerConfidence, ks4Results[2].LAAverageP8UpperConfidence)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = "National Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[0].NationalAverageP8Score)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[1].NationalAverageP8Score)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[2].NationalAverageP8Score)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = "National confidence interval", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedConfidenceInterval(
                                    ks4Results[0].NationalAverageP8LowerConfidence,
                                    ks4Results[0].NationalAverageP8UpperConfidence)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedConfidenceInterval(
                                    ks4Results[1].NationalAverageP8LowerConfidence,
                                    ks4Results[1].NationalAverageP8UpperConfidence)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedConfidenceInterval(
                                    ks4Results[2].NationalAverageP8LowerConfidence,
                                    ks4Results[2].NationalAverageP8UpperConfidence)
                            }
                        }
                    });

                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.Two);
                        hBuilder.AddText("Progress 8 English");
                    });

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "", Bold = true},
                            new TextElement {Value = ks4Results[0].Year, Bold = true},
                            new TextElement {Value = ks4Results[1].Year, Bold = true},
                            new TextElement {Value = ks4Results[2].Year, Bold = true}
                        },
                        new[]
                        {
                            new TextElement {Value = htbDocument.SchoolName, Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0]
                                    .SipProgress8english)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1]
                                    .SipProgress8english)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2]
                                    .SipProgress8english)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = $"{htbDocument.LocalAuthorityName} LA Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[0].LAAverageP8English)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[1].LAAverageP8English)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[2].LAAverageP8English)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = "National Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0]
                                    .NationalAverageP8English)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1]
                                    .NationalAverageP8English)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2]
                                    .NationalAverageP8English)
                            }
                        }
                    });

                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.Two);
                        hBuilder.AddText("Progress 8 Maths");
                    });

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "", Bold = true},
                            new TextElement {Value = ks4Results[0].Year, Bold = true},
                            new TextElement {Value = ks4Results[1].Year, Bold = true},
                            new TextElement {Value = ks4Results[2].Year, Bold = true}
                        },
                        new[]
                        {
                            new TextElement {Value = htbDocument.SchoolName, Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0].SipProgress8maths)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1].SipProgress8maths)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2].SipProgress8maths)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = $"{htbDocument.LocalAuthorityName} LA Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0].LAAverageP8Maths)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1].LAAverageP8Maths)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2].LAAverageP8Maths)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = "National Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[0].NationalAverageP8Maths)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[1].NationalAverageP8Maths)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[2].NationalAverageP8Maths)
                            }
                        }
                    });

                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.Two);
                        hBuilder.AddText("Progress 8 EBacc");
                    });

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "", Bold = true},
                            new TextElement {Value = ks4Results[0].Year, Bold = true},
                            new TextElement {Value = ks4Results[1].Year, Bold = true},
                            new TextElement {Value = ks4Results[2].Year, Bold = true}
                        },
                        new[]
                        {
                            new TextElement {Value = htbDocument.SchoolName, Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0].SipProgress8ebacc)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1].SipProgress8ebacc)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2].SipProgress8ebacc)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = $"{htbDocument.LocalAuthorityName} LA Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[0].LAAverageP8Ebacc)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[1].LAAverageP8Ebacc)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(ks4Results[2].LAAverageP8Ebacc)
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = "National Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[0].NationalAverageP8Ebacc)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[1].NationalAverageP8Ebacc)
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedStringResult(
                                    ks4Results[2].NationalAverageP8Ebacc)
                            }
                        }
                    });

                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.Two);
                        hBuilder.AddText("Percentage of pupils entering EBacc");
                    });

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "", Bold = true},
                            new TextElement {Value = ks4Results[0].Year, Bold = true},
                            new TextElement {Value = ks4Results[1].Year, Bold = true},
                            new TextElement {Value = ks4Results[2].Year, Bold = true}
                        },
                        new[]
                        {
                            new TextElement {Value = htbDocument.SchoolName, Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedResult(ks4Results[0].Enteringebacc.ToString())
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedResult(ks4Results[1].Enteringebacc.ToString())
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedResult(ks4Results[2].Enteringebacc.ToString())
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = $"{htbDocument.LocalAuthorityName} LA Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedResult(ks4Results[0].LAEnteringEbacc.ToString())
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedResult(ks4Results[1].LAEnteringEbacc.ToString())
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedResult(ks4Results[2].LAEnteringEbacc.ToString())
                            }
                        },
                        new[]
                        {
                            new TextElement {Value = "National Average", Bold = true},
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedResult(ks4Results[0].NationalEnteringEbacc.ToString())
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedResult(ks4Results[1].NationalEnteringEbacc.ToString())
                            },
                            new TextElement
                            {
                                Value = PerformanceDataHelpers.GetFormattedResult(ks4Results[2].NationalEnteringEbacc.ToString())
                            }
                        }
                    });

                    builder.AddParagraph(pBuilder => pBuilder.AddText(""));

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "Additional information", Bold = true},
                            new TextElement {Value = htbDocument.KeyStage4AdditionalInformation}
                        }
                    });
                });
            }
        }

        private static void BuildKeyStage5PerformanceInformation(IDocumentBuilder documentBuilder,
            HtbDocument htbDocument)
        {
            if (!PerformanceDataHelpers.HasKeyStage5PerformanceInformation(htbDocument.KeyStage5Performance))
            {
                documentBuilder.ReplacePlaceholderWithContent("KeyStage5PerformanceSection",
                    b => { b.AddParagraph(string.Empty); });
            }
            else
            {
                documentBuilder.ReplacePlaceholderWithContent("KeyStage5PerformanceSection", builder =>
                {
                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.One);
                        hBuilder.AddText(new TextElement {Value = "Key stage 5 performance tables (KS5)", Bold = true});
                    });

                    foreach (var ks5Result in htbDocument.KeyStage5Performance
                        .OrderByDescending(k => k.Year))
                    {
                        builder.AddHeading(hBuilder =>
                        {
                            hBuilder.SetHeadingLevel(HeadingLevel.Two);
                            hBuilder.AddText($"{PerformanceDataHelpers.GetFormattedYear(ks5Result.Year)} Key stage 5");
                        });

                        builder.AddTable(new[]
                        {
                            new[]
                            {
                                new TextElement {Value = "", Bold = true},
                                new TextElement {Value = "Academic progress", Bold = true},
                                new TextElement {Value = "Academic average", Bold = true},
                                new TextElement {Value = "Applied general progress", Bold = true},
                                new TextElement {Value = "Applied general average", Bold = true}
                            },
                            new[]
                            {
                                new TextElement {Value = htbDocument.SchoolName, Bold = true},
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedResult(ks5Result.Academy.AcademicProgress)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedResult(ks5Result.Academy.AcademicAverage)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedResult(ks5Result.Academy.AppliedGeneralProgress)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedResult(ks5Result.Academy.AppliedGeneralAverage)}"
                                }
                            },
                            new[]
                            {
                                new TextElement {Value = "National average", Bold = true},
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedResult(ks5Result.National.AcademicProgress)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedResult(ks5Result.National.AcademicAverage)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedResult(ks5Result.National.AppliedGeneralProgress)}"
                                },
                                new TextElement
                                {
                                    Value =
                                        $"{PerformanceDataHelpers.GetFormattedResult(ks5Result.National.AppliedGeneralAverage)}"
                                }
                            }
                        });
                    }

                    builder.AddParagraph(pBuilder => pBuilder.AddText(""));

                    builder.AddTable(new[]
                    {
                        new[]
                        {
                            new TextElement {Value = "Additional information", Bold = true},
                            new TextElement {Value = htbDocument.KeyStage5AdditionalInformation}
                        }
                    });
                });
            }
        }

        private static CreateHtbDocumentResponse CreateErrorResponse(
            ServiceResponseError serviceResponseError)
        {
            if (serviceResponseError.ErrorCode == ErrorCode.NotFound)
            {
                return new CreateHtbDocumentResponse
                {
                    ResponseError = new ServiceResponseError
                    {
                        ErrorCode = ErrorCode.NotFound,
                        ErrorMessage = "Not found"
                    }
                };
            }

            return new CreateHtbDocumentResponse
            {
                ResponseError = new ServiceResponseError
                {
                    ErrorCode = ErrorCode.ApiError,
                    ErrorMessage = "API has encountered an error"
                }
            };
        }
    }
}