using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;
using DocumentGeneration;
using DocumentGeneration.Elements;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;

namespace Frontend.Services
{
    public class CreateHtbDocument : ICreateHtbDocument
    {
        private readonly IProjects _projectsRepository;
        private readonly IAcademies _academiesRepository;

        public CreateHtbDocument(IProjects projectsRepository, IAcademies academiesRepository)
        {
            _projectsRepository = projectsRepository;
            _academiesRepository = academiesRepository;
        }

        public async Task<CreateHtbDocumentResponse> Execute(string projectUrn)
        {
            var projectResult = await _projectsRepository.GetByUrn(projectUrn);
            if (!projectResult.IsValid)
            {
                return CreateErrorResponse(projectResult.Error);
            }

            var projectAcademy = projectResult.Result.TransferringAcademies.First();
            var academyResult = await _academiesRepository.GetAcademyByUkprn(projectAcademy.OutgoingAcademyUkprn);
            if (!academyResult.IsValid)
            {
                return CreateErrorResponse(academyResult.Error);
            }

            var project = projectResult.Result;
            var academy = academyResult.Result;

            MemoryStream ms;

            await using (ms = new MemoryStream())
            {
                var builder = new DocumentBuilder(ms);

                HeaderAndFooter(builder);
                Title(builder, academy);
                IntroductorySection(builder, academy, project);
                GeneralInformation(builder, academy);
                Rationale(builder, project);
                KeyStagePerformanceInformation(builder, academy);

                builder.Build();
            }

            var successResponse = new CreateHtbDocumentResponse
            {
                Document = ms.ToArray()
            };
            return successResponse;
        }

        private static void Title(DocumentBuilder builder, Academy academy)
        {
            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.One);
                hBuilder.AddText("Headteacher board (HTB) template for:");
            });

            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.Two);
                hBuilder.AddText($"{academy.Name} - URN {academy.Urn}");
            });

            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.Two);
                hBuilder.AddText($"Outgoing trust - SomeID");
            });
        }

        private static void HeaderAndFooter(DocumentBuilder builder)
        {
            builder.AddHeader(hBuilder =>
            {
                hBuilder.AddParagraph(pBuilder =>
                {
                    pBuilder.Justify(ParagraphJustification.Center);
                    pBuilder.AddText(new TextElement {Bold = true, Value = "OFFICIAL"});
                });
            });

            builder.AddFooter(fBuilder =>
            {
                fBuilder.AddTable(tBuilder =>
                {
                    tBuilder.SetBorderStyle(TableBorderStyle.None);
                    tBuilder.AddRow(rBuilder =>
                    {
                        rBuilder.AddCells(new[]
                        {
                            "Author: Meow Meowington", "Cleared by: Woofs Barkington", "Version: 01/01/2021"
                        });
                    });
                });
            });
        }

        private static void KeyStagePerformanceInformation(DocumentBuilder builder, Academy academy)
        {
            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.One);
                hBuilder.AddText("EXAMPLE Key stage performance tables (KS2)");
            });

            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.Two);
                hBuilder.AddText("2019 Key stage 2");
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
                    new TextElement {Value = academy.Name, Bold = true},
                    new TextElement {Value = "47\n(disadvantaged 20)"},
                    new TextElement {Value = "31\n(disadvantaged 14)"},
                    new TextElement {Value = "Suppressed"},
                    new TextElement {Value = "Suppressed"},
                    new TextElement {Value = "Suppressed"}
                },
                new[]
                {
                    new TextElement {Value = "Local authority average", Bold = true},
                    new TextElement {Value = "63"},
                    new TextElement {Value = "9"},
                    new TextElement {Value = "0.2\n(disadvantaged -0.1)"},
                    new TextElement {Value = "-0.8\n(disadvantaged 0.2)"},
                    new TextElement {Value = "-0.3\n(disadvantaged 0.2)"}
                },
                new[]
                {
                    new TextElement {Value = "National average", Bold = true},
                    new TextElement {Value = "65\n(disadvantaged 51)"},
                    new TextElement {Value = "11\n(disadvantaged 5)"},
                    new TextElement {Value = "0.00"},
                    new TextElement {Value = "0.00"},
                    new TextElement {Value = "0.00"}
                }
            });
        }

        private static void Rationale(DocumentBuilder builder, Project project)
        {
            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.One);
                hBuilder.AddText("Rationale");
            });

            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.Two);
                hBuilder.AddText("Rationale for the project");
            });

            builder.AddParagraph(pBuilder => pBuilder.AddText(project.Rationale.Project));

            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.One);
                hBuilder.AddText("Rationale for the trust or sponsor");
            });

            builder.AddParagraph(pBuilder => pBuilder.AddText(project.Rationale.Trust));
        }

        private static void GeneralInformation(DocumentBuilder builder, Academy academy)
        {
            builder.AddHeading(hBuilder =>
            {
                hBuilder.SetHeadingLevel(HeadingLevel.One);
                hBuilder.AddText("General information");
            });

            builder.AddTable(new[]
            {
                new[]
                {
                    new TextElement {Value = "School type", Bold = true},
                    new TextElement {Value = academy.EstablishmentType}
                },
                new[]
                {
                    new TextElement {Value = "School phase", Bold = true},
                    new TextElement {Value = academy.Performance.SchoolPhase}
                },
                new[]
                {
                    new TextElement {Value = "Age range", Bold = true},
                    new TextElement {Value = academy.Performance.AgeRange}
                }
            });

            builder.AddParagraph(pBuilder => pBuilder.AddText(""));

            builder.AddTable(new[]
            {
                new[]
                {
                    new TextElement {Value = "Capacity", Bold = true},
                    new TextElement {Value = academy.Performance.Capacity}
                },
                new[]
                {
                    new TextElement {Value = "Published admission number (PAN)", Bold = true},
                    new TextElement {Value = academy.Performance.Pan}
                },
                new[]
                {
                    new TextElement {Value = "Percentage the school is full", Bold = true},
                    new TextElement {Value = academy.Performance.PercentageFull}
                },
                new[]
                {
                    new TextElement {Value = "Percentage of free school meals at the school (%FSM)", Bold = true},
                    new TextElement {Value = ""}
                }
            });

            builder.AddParagraph(pBuilder => pBuilder.AddText(""));

            builder.AddTable(new[]
            {
                new[]
                {
                    new TextElement {Value = "Viability issues", Bold = true},
                    new TextElement {Value = ""}
                },
                new[]
                {
                    new TextElement {Value = "Financial surplus or deficit", Bold = true},
                    new TextElement {Value = ""}
                },
                new[]
                {
                    new TextElement {Value = "Private finance initiative (PFI) scheme", Bold = true},
                    new TextElement {Value = ""}
                }
            });

            builder.AddParagraph(pBuilder => pBuilder.AddText(""));

            builder.AddTable(new[]
            {
                new[]
                {
                    new TextElement {Value = "Is this a diocesan multi-academy trust (MAT)", Bold = true},
                    new TextElement {Value = ""}
                },
                new[]
                {
                    new TextElement
                    {
                        Value = "Percentage of good or outstanding schools in the diocesan trust", Bold = true
                    },
                    new TextElement {Value = ""}
                },
                new[]
                {
                    new TextElement {Value = "Distance from the school to the trust headquarters", Bold = true},
                    new TextElement {Value = ""}
                },
                new[]
                {
                    new TextElement {Value = "MP details", Bold = true},
                    new TextElement {Value = ""}
                }
            });
        }

        private static void IntroductorySection(DocumentBuilder builder, Academy academy, Project project)
        {
            builder.AddTable(new[]
            {
                new[]
                {
                    new TextElement {Value = "Recommendation", Bold = true},
                    new TextElement {Value = "Project recommendation"}
                },
                new[]
                {
                    new TextElement {Value = "Academy type and route", Bold = true},
                    new TextElement {Value = academy.EstablishmentType}
                },
                new[]
                {
                    new TextElement {Value = "Academy type and route", Bold = true},
                    new TextElement {Value = academy.EstablishmentType}
                }
            });

            builder.AddParagraph(pBuilder => pBuilder.AddText(""));

            builder.AddTable(new[]
            {
                new[]
                {
                    new TextElement {Value = "Date of HTB", Bold = true},
                    new TextElement {Value = "Date"}
                },
                new[]
                {
                    new TextElement {Value = "Proposed academy opening date", Bold = true},
                    new TextElement {Value = "N/A"}
                },
                new[]
                {
                    new TextElement {Value = "Previous HTB date", Bold = true},
                    new TextElement {Value = "adwad"}
                }
            });

            builder.AddParagraph(pBuilder => pBuilder.AddText(""));

            builder.AddTable(new[]
            {
                new[]
                {
                    new TextElement {Value = "Local authority", Bold = true},
                    new TextElement {Value = "LA"}
                },
                new[]
                {
                    new TextElement {Value = "Sponsor name", Bold = true},
                    new TextElement {Value = project.TransferringAcademies[0].IncomingTrustName}
                },
                new[]
                {
                    new TextElement {Value = "Sponsor reference number", Bold = true},
                    new TextElement {Value = "SomeNumber"}
                }
            });
        }

        private static CreateHtbDocumentResponse CreateErrorResponse(
            RepositoryResultBase.RepositoryError repositoryError)
        {
            if (repositoryError.StatusCode == System.Net.HttpStatusCode.NotFound)
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