using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Data;
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

                builder.AddTable(tBuilder =>
                {
                    tBuilder.AddRow(rBuilder =>
                    {
                        rBuilder.AddCells(new[]
                        {
                            new TextElement {Value = "Recommendation", Bold = true},
                            new TextElement {Value = "Project recommendation"}
                        });
                    });

                    tBuilder.AddRow(rBuilder =>
                    {
                        rBuilder.AddCells(new[]
                        {
                            new TextElement {Value = "Is an academy order (AO) required?", Bold = true},
                            new TextElement {Value = "N/A"}
                        });
                    });

                    tBuilder.AddRow(rBuilder =>
                    {
                        rBuilder.AddCells(new[]
                        {
                            new TextElement {Value = "Academy type and route", Bold = true},
                            new TextElement {Value = academy.EstablishmentType}
                        });
                    });
                });

                builder.AddParagraph(pBuilder => pBuilder.AddText(""));

                builder.AddTable(tBuilder =>
                {
                    tBuilder.AddRow(rBuilder =>
                    {
                        rBuilder.AddCells(new[]
                        {
                            new TextElement {Value = "Date of HTB", Bold = true},
                            new TextElement {Value = "Date"}
                        });
                    });

                    tBuilder.AddRow(rBuilder =>
                    {
                        rBuilder.AddCells(new[]
                        {
                            new TextElement {Value = "Proposed academy opening date", Bold = true},
                            new TextElement {Value = "N/A"}
                        });
                    });

                    tBuilder.AddRow(rBuilder =>
                    {
                        rBuilder.AddCells(new[]
                        {
                            new TextElement {Value = "Previous HTB date", Bold = true},
                            new TextElement {Value = "adwad"}
                        });
                    });
                });

                builder.AddHeading(hBuilder =>
                {
                    hBuilder.SetHeadingLevel(HeadingLevel.One);
                    hBuilder.AddText("KS2 Perf tables");
                });

                builder.AddTable(tBuilder =>
                {
                    tBuilder.AddRow(rBuilder =>
                    {
                        rBuilder.AddCells(new[]
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
                        });
                    });

                    tBuilder.AddRow(rBuilder =>
                    {
                        rBuilder.AddCell(new TextElement {Value = academy.Name, Bold = true});
                        rBuilder.AddCells(new[]
                        {
                            "47\n(disadvantaged 20)",
                            "31\n(disadvantaged 14)",
                            "Suppressed",
                            "Suppressed",
                            "Suppressed"
                        });
                    });
                });

                builder.Build();
            }

            var successResponse = new CreateHtbDocumentResponse
            {
                Document = ms.ToArray()
            };
            return successResponse;
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