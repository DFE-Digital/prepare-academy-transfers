using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Data.Models.Projects;
using DocumentGeneration;
using DocumentGeneration.Elements;
using Frontend.Helpers;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Helpers;

namespace Frontend.Services
{
    public class CreateHtbDocument : ICreateHtbDocument
    {
        private readonly IProjects _projectsRepository;
        private readonly IAcademies _academiesRepository;
        private readonly IGetInformationForProject _getInformationForProject;
        
        public CreateHtbDocument(IProjects projectsRepository, IAcademies academiesRepository, IGetInformationForProject getInformationForProject )
        {
            _projectsRepository = projectsRepository;
            _academiesRepository = academiesRepository;
            _getInformationForProject = getInformationForProject;
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

            var informationForProjectResult = await _getInformationForProject.Execute(projectUrn);
            if (!informationForProjectResult.IsValid)
            {
                return CreateErrorResponse(informationForProjectResult.ResponseError);
            }

            var project = projectResult.Result;
            var academy = academyResult.Result;
            var informationForProject = informationForProjectResult;
            
            var htbDocument = new HtbDocument
            {
                Recommendation = EnumHelpers<TransferAcademyAndTrustInformation.RecommendationResult>.GetDisplayValue(project.AcademyAndTrustInformation.Recommendation),
                Author = project.AcademyAndTrustInformation.Author,
                ProjectName = project.Name,
                SponsorName = project.IncomingTrustName,
                AcademyTypeAndRoute = academy.EstablishmentType,
                SchoolName = academy.Name,
                SchoolUrn =  academy.Urn,
                TrustName = project.OutgoingTrustName,
                TrustReferenceNumber = project.OutgoingTrustUkprn,
                SchoolType = academy.EstablishmentType,
                SchoolPhase = academy.GeneralInformation.SchoolPhase,
                AgeRange = academy.GeneralInformation.AgeRange,
                SchoolCapacity = academy.GeneralInformation.Capacity,
                PublishedAdmissionNumber = academy.GeneralInformation.Pan,
                NumberOnRoll = $"{academy.GeneralInformation.NumberOnRoll} ({academy.GeneralInformation.PercentageFull}%) ",
                PercentageSchoolFull = academy.GeneralInformation.PercentageFull,
                PercentageFreeSchoolMeals = academy.PupilNumbers.EligibleForFreeSchoolMeals,
                OfstedLastInspection = academy.LatestOfstedJudgement.InspectionDate,
                OverallEffectiveness = academy.LatestOfstedJudgement.OverallEffectiveness,
                RationaleForProject = project.Rationale.Project,
                RationaleForTrust = project.Rationale.Trust,
                ClearedBy = "Cleared by",
                Version = "Version",
                DateOfHtb = DatesHelper.DateStringToGovUkDate(project.Dates.Htb),
                DateOfProposedTransfer = DatesHelper.DateStringToGovUkDate(project.Dates.Target),
                DateTransferWasFirstDiscussed = DatesHelper.DateStringToGovUkDate(project.Dates.FirstDiscussed),
                ViabilityIssues = academy.GeneralInformation.ViabilityIssue,
                FinancialDeficit = academy.GeneralInformation.Pfi,
                Pfi = academy.GeneralInformation.Pfi,
                PercentageGoodOrOutstandingInDiocesanTrust = academy.GeneralInformation.DiocesesPercent,
                DistanceFromTheAcademyToTheTrustHeadquarters = academy.GeneralInformation.DistanceToSponsorHq,
                MpAndParty = academy.GeneralInformation.MpAndParty,
                WhoInitiatedTheTransfer = EnumHelpers<TransferFeatures.ProjectInitiators>.GetDisplayValue(project.Features.WhoInitiatedTheTransfer),
                ReasonForTransfer = project.Features.IsTransferSubjectToIntervention ? "Subject to Intervention" : "Not subject to intervention",
                MoreDetailsAboutTheTransfer = project.Features.ReasonForTransfer.InterventionDetails,
                TypeOfTransfer = project.Features.TypeOfTransfer == TransferFeatures.TransferTypes.Empty ? project.Features.OtherTypeOfTransfer : 
                    EnumHelpers<TransferFeatures.TransferTypes>.GetDisplayValue(project.Features.TypeOfTransfer),
                TransferBenefits = GetTransferBenefits(project.Benefits),
                OtherFactors = GetOtherFactors(project.Benefits),
                GirlsOnRoll = academy.PupilNumbers.GirlsOnRoll,
                BoysOnRoll = academy.PupilNumbers.BoysOnRoll,
                PupilsWithSen = academy.PupilNumbers.WithStatementOfSen,
                PupilsWithFirstLanguageNotEnglish = academy.PupilNumbers.WhoseFirstLanguageIsNotEnglish,
                PupilsFsm6Years = academy.PupilNumbers.EligibleForFreeSchoolMeals,
                PupilNumbersAdditionalInformation = project.PupilNumbersAdditionalInformation,
                OfstedReport = academy.LatestOfstedJudgement.OfstedReport,
                OfstedAdditionalInformation = project.LatestOfstedJudgementAdditionalInformation
            };

            var ms = CreateMemoryStream("htb-template");
            var builder = DocumentBuilder.CreateFromTemplate(ms, htbDocument);
            
            BuildKeyStage2PerformanceInformation(builder, informationForProject);

            return new CreateHtbDocumentResponse
            {
                Document = builder.Build()
            };
        }

        private static void BuildKeyStage2PerformanceInformation(DocumentBuilder documentBuilder, GetInformationForProjectResponse informationForProject)
        {
            const string newLineCharacter = "\n";
            documentBuilder.ReplacePlaceholderWithContent("KeyStage2PerformanceSection", builder =>
            {
                builder.AddHeading(hBuilder =>
                {
                    hBuilder.SetHeadingLevel(HeadingLevel.One);
                    hBuilder.AddText("Key stage performance tables (KS2)");
                });

                var academy = informationForProject.OutgoingAcademy;
                foreach (var ks2Result in informationForProject.EducationPerformance.KeyStage2Performance.OrderByDescending(k => k.Year))
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
                            new TextElement {Value = academy.Name, Bold = true},
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.PercentageMeetingExpectedStdInRWM, newLineCharacter)}"
                            },
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.PercentageAchievingHigherStdInRWM, newLineCharacter)}"
                            },
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.ReadingProgressScore, newLineCharacter)}"
                            },
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.WritingProgressScore, newLineCharacter)}"
                            },
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.MathsProgressScore, newLineCharacter)}"
                            },
                        },
                        new[]
                        {
                            new TextElement {Value = $"{academy.LocalAuthorityName} LA average", Bold = true},
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.LAAveragePercentageMeetingExpectedStdInRWM, newLineCharacter)}"
                            },
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.LAAveragePercentageAchievingHigherStdInRWM, newLineCharacter)}"
                            },
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.LAAverageReadingProgressScore, newLineCharacter)}"
                            },
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.LAAverageWritingProgressScore, newLineCharacter)}"
                            },
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.LAAverageMathsProgressScore, newLineCharacter)}"
                            },
                        },
                        new[]
                        {
                            new TextElement {Value = "National average", Bold = true},
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.NationalAveragePercentageMeetingExpectedStdInRWM, newLineCharacter)}"
                            },
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.NationalAveragePercentageAchievingHigherStdInRWM, newLineCharacter)}"
                            },
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.NationalAverageReadingProgressScore, newLineCharacter)}"
                            },
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.NationalAverageWritingProgressScore, newLineCharacter)}"
                            },
                            new TextElement
                            {
                                Value =
                                    $"{PerformanceDataHelpers.GetFormattedResult(ks2Result.NationalAverageMathsProgressScore, newLineCharacter)}"
                            },
                        }
                    });
                }
            });
        }

        public async Task<CreateHtbDocumentResponse> XExecute(string projectUrn)
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

            var builder = new DocumentBuilder();

            HeaderAndFooter(builder);
            Title(builder, academy);
            IntroductorySection(builder, academy, project);
            GeneralInformation(builder, academy);
            Rationale(builder, project);
            KeyStagePerformanceInformation(builder, academy);

            var successResponse = new CreateHtbDocumentResponse
            {
                Document = builder.Build()
            };
            return successResponse;
        }
        
        private static string GetOtherFactors(TransferBenefits transferBenefits)
        {
            var otherFactorsSummary = transferBenefits.OtherFactors.Select(otherFactor => new[]
            {
                EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(otherFactor.Key),
                otherFactor.Value
            }).ToList();

            var sb = new StringBuilder();
            foreach (var otherFactor in otherFactorsSummary)
            {
                sb.Append($"{otherFactor[0]}\n");
                if (!string.IsNullOrEmpty(otherFactor[1]))
                    sb.Append($"{otherFactor[1]}\n");
            }

            return sb.ToString();
        }

        private static string GetTransferBenefits(TransferBenefits transferBenefits)
        {
            var benefitSummary = transferBenefits.IntendedBenefits
                .FindAll(EnumHelpers<TransferBenefits.IntendedBenefit>.HasDisplayValue)
                .Select(EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue)
                .ToList();

            if (transferBenefits.IntendedBenefits.Contains(TransferBenefits.IntendedBenefit.Other))
            {
                benefitSummary.Add($"Other: {transferBenefits.OtherIntendedBenefit}");
            }

            var sb = new StringBuilder();
            foreach (var benefit in benefitSummary)
            {
                sb.Append($"{benefit}\n");
            }

            return sb.ToString();
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
                    new TextElement {Value = academy.GeneralInformation.SchoolPhase}
                },
                new[]
                {
                    new TextElement {Value = "Age range", Bold = true},
                    new TextElement {Value = academy.GeneralInformation.AgeRange}
                }
            });

            builder.AddParagraph(pBuilder => pBuilder.AddText(""));

            builder.AddTable(new[]
            {
                new[]
                {
                    new TextElement {Value = "Capacity", Bold = true},
                    new TextElement {Value = academy.GeneralInformation.Capacity}
                },
                new[]
                {
                    new TextElement {Value = "Published admission number (PAN)", Bold = true},
                    new TextElement {Value = academy.GeneralInformation.Pan}
                },
                new[]
                {
                    new TextElement {Value = "Percentage the school is full", Bold = true},
                    new TextElement {Value = academy.GeneralInformation.PercentageFull}
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