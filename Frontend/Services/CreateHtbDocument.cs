using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;
using DocumentGeneration;
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
                var generator = new XDocumentBuilder(ms);

                generator.AddHeading(project.Name, XDocumentHeadingBuilder.HeadingLevelOptions.Heading1);
                generator.AddHeading(academy.Name, XDocumentHeadingBuilder.HeadingLevelOptions.Heading2);

                AddAcademyPerformanceTable(generator, academyResult);
                AddPupilNumbersTable(generator, academyResult);
                AddOfstedJudgementTable(generator, academyResult);

                generator.Build();
            }

            var successResponse = new CreateHtbDocumentResponse
            {
                Document = ms.ToArray()
            };
            return successResponse;
        }

        private static void AddOfstedJudgementTable(IXDocumentBuilder generator, RepositoryResult<Academy> academyResult)
        {
            generator.AddHeading("Latest Ofsted judgement", XDocumentHeadingBuilder.HeadingLevelOptions.Heading3);

            var data = academyResult.Result.LatestOfstedJudgement.FieldsToDisplay()
                .Select(field => new List<string> {field.Title, field.Value}).ToList();

            generator.AddTable(data);
        }

        private static void AddPupilNumbersTable(IXDocumentBuilder generator, RepositoryResult<Academy> academyResult)
        {
            generator.AddHeading("Pupil numbers", XDocumentHeadingBuilder.HeadingLevelOptions.Heading3);

            var data = academyResult.Result.PupilNumbers.FieldsToDisplay()
                .Select(field => new List<string> {field.Title, field.Value}).ToList();

            generator.AddTable(data);
        }

        private static void AddAcademyPerformanceTable(IXDocumentBuilder generator,
            RepositoryResult<Academy> academyResult)
        {
            generator.AddHeading("Academy performance", XDocumentHeadingBuilder.HeadingLevelOptions.Heading3);

            var academyPerformanceData = academyResult.Result.Performance.FieldsToDisplay()
                .Select(field => new List<string> {field.Title, field.Value}).ToList();

            generator.AddTable(academyPerformanceData);
        }

        private static CreateHtbDocumentResponse CreateErrorResponse(RepositoryResultBase.RepositoryError repositoryError)
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