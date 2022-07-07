using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration;
using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces;
using Frontend.Helpers;
using Frontend.Models.ProjectTemplate;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;

namespace Frontend.Services
{
    public class CreateProjectTemplate : ICreateProjectTemplate
    {
        private readonly IGetProjectTemplateModel _getProjectTemplateModel;
        private const string LocalAuthority = "local authority";
        
        public CreateProjectTemplate(IGetProjectTemplateModel getProjectTemplateModel)
        {
            _getProjectTemplateModel = getProjectTemplateModel;
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

        public async Task<CreateProjectTemplateResponse> Execute(string projectUrn)
        {
            var getHtbDocumentForProject = await _getProjectTemplateModel.Execute(projectUrn);
            if (!getHtbDocumentForProject.IsValid)
            {
                return CreateErrorResponse(getHtbDocumentForProject.ResponseError);
            }

            var projectTemplateModel = getHtbDocumentForProject.ProjectTemplateModel;

            var ms = CreateMemoryStream("htb-template");
            var builder = DocumentBuilder.CreateFromTemplate(ms, projectTemplateModel);

            BuildTitle(builder, projectTemplateModel);
            BuildOtherFactors(builder, projectTemplateModel);
            BuildAcademyData(builder, projectTemplateModel.Academies);

            return new CreateProjectTemplateResponse
            {
                Document = builder.Build()
            };
        }

        private void BuildOtherFactors(DocumentBuilder documentBuilder, ProjectTemplateModel projectTemplateModel)
        {
            documentBuilder.ReplacePlaceholderWithContent("Risks", builder =>
            {
                builder.AddTable(tableBuilder =>
                {
                    foreach (var otherFactor in projectTemplateModel.OtherFactors)
                    {
                        //Full Table Width = dxa 9740
                        tableBuilder.AddRow(rowBuilder =>
                        {
                            rowBuilder.AddCell(new TextElement(otherFactor.Item1)
                                    {Bold = true},
                                new TableCellProperties
                                {
                                    TableCellWidth = new TableCellWidth
                                        {Width = "5235", Type = TableWidthUnitValues.Dxa},
                                    TableCellBorders = new TableCellBorders
                                    {
                                        TopBorder = new TopBorder {Size = 0, Color = "ffffff"}
                                    }
                                });
                            rowBuilder.AddCell(new TextElement(otherFactor.Item2),
                                new TableCellProperties
                                {
                                    TableCellWidth = new TableCellWidth
                                        {Width = "4505", Type = TableWidthUnitValues.Dxa},
                                    TableCellBorders = new TableCellBorders
                                    {
                                        TopBorder = new TopBorder {Size = 0, Color = "ffffff"}
                                    }
                                });
                        });
                    }
                });
            });
        }

        private void BuildAcademyData(DocumentBuilder documentBuilder, List<ProjectTemplateAcademyModel> academies)
        {
            documentBuilder.ReplacePlaceholderWithContent("AcademySection", builder =>
            {
                foreach (var academy in academies)
                {
                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(HeadingLevel.One);
                        hBuilder.AddText(new TextElement {Value = academy.SchoolName, Bold = true});
                    });

                    BuildGeneralInformation(builder, academy);
                    BuildPupilNumbers(builder, academy);
                    BuildLatestOfsted(builder, academy);
                    BuildKeyStage2PerformanceInformation(builder, academy);
                    BuildKeyStage4PerformanceInformation(builder, academy);
                    BuildKeyStage5PerformanceInformation(builder, academy);
                    builder.AddParagraph(pBuilder => pBuilder.AddPageBreak());
                }
            });
        }

        private static void BuildTitle(IDocumentBuilder documentBuilder, ProjectTemplateModel projectTemplateModel)
        {
            documentBuilder.ReplacePlaceholderWithContent("HtbTemplateTitle", builder =>
            {
                builder.AddHeading(hBuilder =>
                {
                    hBuilder.SetHeadingLevel(HeadingLevel.One);
                    hBuilder.AddText(new TextElement
                    {
                        Value = "Project template for:\n" +
                                $"Incoming trust: {projectTemplateModel.IncomingTrustName} (UKPRN: {projectTemplateModel.IncomingTrustUkprn})\n \n" +
                                $"Project reference: {projectTemplateModel.ProjectReference}",
                        Bold = true
                    });
                });
            });
        }

        private static void BuildGeneralInformation(IDocumentBodyBuilder builder, ProjectTemplateAcademyModel academy)
        {
            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.Two);
                hBuilder.AddText(new TextElement {Value = "General information", Bold = true});
            });

            builder.AddTable(new[]
            {
                new[]
                {
                    new TextElement {Value = "School type", Bold = true},
                    new TextElement {Value = academy.SchoolType},
                },
                new[]
                {
                    new TextElement {Value = "School phase", Bold = true},
                    new TextElement {Value = academy.SchoolPhase},
                },
                new[]
                {
                    new TextElement {Value = "Age range", Bold = true},
                    new TextElement {Value = academy.AgeRange},
                },
                new[]
                {
                    new TextElement {Value = "Capacity", Bold = true},
                    new TextElement {Value = academy.SchoolCapacity},
                },
                new[]
                {
                    new TextElement {Value = "Published admission number (PAN)", Bold = true},
                    new TextElement {Value = academy.PublishedAdmissionNumber},
                },
                new[]
                {
                    new TextElement {Value = "Number on roll (percentage the school is full)", Bold = true},
                    new TextElement {Value = $"{academy.NumberOnRoll}"},
                },
                new[]
                {
                    new TextElement {Value = "Percentage of free school meals (%FSM)", Bold = true},
                    new TextElement {Value = $"{academy.PercentageFreeSchoolMeals}"},
                },
                new[]
                {
                    new TextElement {Value = "Viability issues", Bold = true},
                    new TextElement {Value = academy.ViabilityIssues},
                },
                new[]
                {
                    new TextElement {Value = "Financial deficit", Bold = true},
                    new TextElement {Value = academy.FinancialDeficit},
                },
                new[]
                {
                    new TextElement {Value = "Private finance initiative (PFI) scheme", Bold = true},
                    new TextElement {Value = $"{academy.Pfi}"},
                },
                new[]
                {
                    new TextElement
                        {Value = "Percentage of good or outstanding schools in the diocesan trust", Bold = true},
                    new TextElement {Value = academy.PercentageGoodOrOutstandingInDiocesanTrust},
                },
                new[]
                {
                    new TextElement {Value = "Distance from the academy to the trust headquarters", Bold = true},
                    new TextElement {Value = academy.DistanceFromTheAcademyToTheTrustHeadquarters},
                },
                new[]
                {
                    new TextElement {Value = "MP (party)", Bold = true},
                    new TextElement {Value = $"{academy.MpAndParty}"},
                }
            });
        }

        private static void BuildPupilNumbers(IDocumentBodyBuilder builder, ProjectTemplateAcademyModel academy)
        {
            builder.AddParagraph(pBuilder => pBuilder.AddPageBreak());

            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.Two);
                hBuilder.AddText(new TextElement {Value = "Pupil numbers", Bold = true});
            });

            builder.AddTable(new[]
            {
                new[]
                {
                    new TextElement {Value = "Girls on roll", Bold = true},
                    new TextElement {Value = academy.GirlsOnRoll},
                },
                new[]
                {
                    new TextElement {Value = "Boys on roll", Bold = true},
                    new TextElement {Value = academy.BoysOnRoll},
                },
                new[]
                {
                    new TextElement
                        {Value = "Pupils with a statement of special educational needs (SEN)", Bold = true},
                    new TextElement {Value = academy.PupilsWithSen},
                },
                new[]
                {
                    new TextElement {Value = "Pupils whose first language is not English", Bold = true},
                    new TextElement {Value = academy.PupilsWithFirstLanguageNotEnglish},
                },
                new[]
                {
                    new TextElement
                        {Value = "Pupils eligible for free school meals during the past 6 years", Bold = true},
                    new TextElement {Value = academy.PupilsFsm6Years},
                },
                new[]
                {
                    new TextElement {Value = "Additional information", Bold = true},
                    new TextElement {Value = academy.PupilNumbersAdditionalInformation},
                }
            });
        }

        private static void BuildLatestOfsted(IDocumentBodyBuilder builder, ProjectTemplateAcademyModel academy)
        {
            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.Two);
                hBuilder.AddText(new TextElement {Value = "Latest Ofsted judgement", Bold = true});
            });

            var ofstedInformation = new List<TextElement[]>
            {
                new[]
                {
                    new TextElement {Value = "School name", Bold = true},
                    new TextElement {Value = academy.SchoolName},
                },
                new[]
                {
                    new TextElement {Value = "Latest full inspection date", Bold = true},
                    new TextElement {Value = academy.InspectionEndDate},
                },
                new[]
                {
                    new TextElement {Value = "Overall effectiveness", Bold = true},
                    new TextElement {Value = academy.OverallEffectiveness},
                },
                new[]
                {
                    new TextElement {Value = "Quality of education", Bold = true},
                    new TextElement {Value = academy.QualityOfEducation},
                },
                new[]
                {
                    new TextElement {Value = "Behaviour and attitudes", Bold = true},
                    new TextElement {Value = academy.BehaviourAndAttitudes},
                },
                new[]
                {
                    new TextElement {Value = "Personal development", Bold = true},
                    new TextElement {Value = academy.PersonalDevelopment},
                },
                new[]
                {
                    new TextElement {Value = "Effectiveness of leadership and management", Bold = true},
                    new TextElement {Value = academy.EffectivenessOfLeadershipAndManagement},
                },
                new[]
                {
                    new TextElement {Value = "Additional information", Bold = true},
                    new TextElement {Value = academy.OfstedAdditionalInformation},
                }
            };

            if (academy.LatestInspectionIsSection8)
            {
                ofstedInformation.Insert(1, new[]
                {
                    new TextElement {Value = "Latest short inspection date", Bold = true},
                    new TextElement {Value = academy.DateOfLatestSection8Inspection},
                });
            }

            if (academy.EarlyYearsProvisionApplicable)
            {
                ofstedInformation.Add(new[]
                {
                    new TextElement {Value = "Early years provision", Bold = true},
                    new TextElement {Value = academy.EarlyYearsProvision},
                });
            }

            if (academy.SixthFormProvisionApplicable)
            {
                ofstedInformation.Add(new[]
                {
                    new TextElement {Value = "Sixth form provision", Bold = true},
                    new TextElement {Value = academy.SixthFormProvision},
                });
            }

            builder.AddTable(ofstedInformation);
        }

        private static void BuildKeyStage2PerformanceInformation(IDocumentBodyBuilder builder,
            ProjectTemplateAcademyModel academy)
        {
            if (!PerformanceDataHelpers.HasKeyStage2PerformanceInformation(academy.KeyStage2Performance))
            {
                return;
            }

            builder.AddParagraph(pBuilder => pBuilder.AddPageBreak());

            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.Two);
                hBuilder.AddText(new TextElement {Value = "Key stage 2 performance tables (KS2)", Bold = true});
            });

            foreach (var ks2Result in academy.KeyStage2Performance.OrderByDescending(k => k.Year))
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
                        new TextElement {Value = academy.SchoolName, Bold = true},
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
                        new TextElement {Value = $"{academy.LocalAuthorityName} {LocalAuthority} average", Bold = true},
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
                    new TextElement {Value = academy.KeyStage2AdditionalInformation}
                }
            });
        }

        private static void BuildKeyStage4PerformanceInformation(IDocumentBodyBuilder builder,
            ProjectTemplateAcademyModel academy)
        {
            if (!PerformanceDataHelpers.HasKeyStage4PerformanceInformation(academy.KeyStage4Performance))
            {
                return;
            }

            builder.AddParagraph(pBuilder => pBuilder.AddPageBreak());

            var ks4Results = academy.KeyStage4Performance.Select(c =>
            {
                if (c.Year != null)
                {
                    c.Year = PerformanceDataHelpers.GetFormattedYear(c.Year);
                }

                return c;
            }).ToList();

            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.Two);
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
                    new TextElement {Value = academy.SchoolName, Bold = true},
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
                    new TextElement {Value = $"{academy.LocalAuthorityName} {LocalAuthority} average", Bold = true},
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
                    new TextElement {Value = "National average", Bold = true},
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
                    new TextElement {Value = academy.SchoolName, Bold = true},
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
                    new TextElement {Value = $"{academy.LocalAuthorityName} {LocalAuthority} average", Bold = true},
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
                    new TextElement {Value = "National average", Bold = true},
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
                    new TextElement {Value = academy.SchoolName, Bold = true},
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
                    new TextElement {Value = $"{academy.LocalAuthorityName} {LocalAuthority} average", Bold = true},
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
                    new TextElement {Value = "National average", Bold = true},
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
                    new TextElement {Value = academy.SchoolName, Bold = true},
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
                    new TextElement {Value = $"{academy.LocalAuthorityName} {LocalAuthority} average", Bold = true},
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
                    new TextElement {Value = "National average", Bold = true},
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
                    new TextElement {Value = academy.SchoolName, Bold = true},
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
                    new TextElement {Value = $"{academy.LocalAuthorityName} {LocalAuthority} average", Bold = true},
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
                    new TextElement {Value = "National average", Bold = true},
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
                    new TextElement {Value = academy.SchoolName, Bold = true},
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
                    new TextElement {Value = $"{academy.LocalAuthorityName} {LocalAuthority} average", Bold = true},
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
                        {Value = $"{academy.LocalAuthorityName} {LocalAuthority} confidence interval", Bold = true},
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
                    new TextElement {Value = "National average", Bold = true},
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
                    new TextElement {Value = academy.SchoolName, Bold = true},
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
                    new TextElement {Value = $"{academy.LocalAuthorityName} {LocalAuthority} average", Bold = true},
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
                    new TextElement {Value = "National average", Bold = true},
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
                    new TextElement {Value = academy.SchoolName, Bold = true},
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
                    new TextElement {Value = $"{academy.LocalAuthorityName} {LocalAuthority} average", Bold = true},
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
                    new TextElement {Value = "National average", Bold = true},
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
                    new TextElement {Value = academy.SchoolName, Bold = true},
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
                    new TextElement {Value = $"{academy.LocalAuthorityName} {LocalAuthority} average", Bold = true},
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
                    new TextElement {Value = "National average", Bold = true},
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
                    new TextElement {Value = academy.SchoolName, Bold = true},
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
                    new TextElement {Value = $"{academy.LocalAuthorityName} {LocalAuthority} average", Bold = true},
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
                    new TextElement {Value = "National average", Bold = true},
                    new TextElement
                    {
                        Value = PerformanceDataHelpers.GetFormattedResult(
                            ks4Results[0].NationalEnteringEbacc.ToString())
                    },
                    new TextElement
                    {
                        Value = PerformanceDataHelpers.GetFormattedResult(
                            ks4Results[1].NationalEnteringEbacc.ToString())
                    },
                    new TextElement
                    {
                        Value = PerformanceDataHelpers.GetFormattedResult(
                            ks4Results[2].NationalEnteringEbacc.ToString())
                    }
                }
            });

            builder.AddParagraph(pBuilder => pBuilder.AddText(""));

            builder.AddTable(new[]
            {
                new[]
                {
                    new TextElement {Value = "Additional information", Bold = true},
                    new TextElement {Value = academy.KeyStage4AdditionalInformation}
                }
            });
        }

        private static void BuildKeyStage5PerformanceInformation(IDocumentBodyBuilder builder,
            ProjectTemplateAcademyModel academy)
        {
            if (!PerformanceDataHelpers.HasKeyStage5PerformanceInformation(academy.KeyStage5Performance))
            {
                return;
            }

            builder.AddParagraph(pBuilder => pBuilder.AddPageBreak());

            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.One);
                hBuilder.AddText(new TextElement {Value = "Key stage 5 performance tables (KS5)", Bold = true});
            });

            foreach (var ks5Result in academy.KeyStage5Performance
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
                        new TextElement {Value = academy.SchoolName, Bold = true},
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
                    new TextElement {Value = academy.KeyStage5AdditionalInformation}
                }
            });
        }

        private static CreateProjectTemplateResponse CreateErrorResponse(
            ServiceResponseError serviceResponseError)
        {
            if (serviceResponseError.ErrorCode == ErrorCode.NotFound)
            {
                return new CreateProjectTemplateResponse
                {
                    ResponseError = new ServiceResponseError
                    {
                        ErrorCode = ErrorCode.NotFound,
                        ErrorMessage = "Not found"
                    }
                };
            }

            return new CreateProjectTemplateResponse
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