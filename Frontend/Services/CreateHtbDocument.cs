using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using API.Repositories.Interfaces;
using Data;
using Data.Models;
using DocumentGeneration;
using Frontend.Services.Interfaces;

namespace Frontend.Services
{
    public class CreateHtbDocument : ICreateHtbDocument
    {
        private readonly IProjectsRepository _dynamicsProjectsRepository;
        private readonly IAcademiesRepository _dynamicsAcademiesRepository;
        private readonly IAcademies _academiesRepository;

        public CreateHtbDocument(IProjectsRepository dynamicsProjectsRepository,
            IAcademiesRepository dynamicsAcademiesRepository, IAcademies academiesRepository)
        {
            _dynamicsProjectsRepository = dynamicsProjectsRepository;
            _dynamicsAcademiesRepository = dynamicsAcademiesRepository;
            _academiesRepository = academiesRepository;
        }

        public async Task<byte[]> Execute(Guid projectId)
        {
            var projectResult = await _dynamicsProjectsRepository.GetProjectById(projectId);
            var projectAcademy = projectResult.Result.ProjectAcademies.First().AcademyId;
            var dynamicsAcademyResult = await _dynamicsAcademiesRepository.GetAcademyById(projectAcademy);
            var dynamicsAcademy = dynamicsAcademyResult.Result;
            var academyResult = await _academiesRepository.GetAcademyByUkprn(dynamicsAcademy.Ukprn);

            MemoryStream ms;

            await using (ms = new MemoryStream())
            {
                var generator = new DocumentBuilder(ms);

                generator.AddHeading(projectResult.Result.ProjectName,
                    DocumentHeadingBuilder.HeadingLevelOptions.Heading1);

                generator.AddHeading(dynamicsAcademy.AcademyName, DocumentHeadingBuilder.HeadingLevelOptions.Heading2);

                AddAcademyPerformanceTable(generator, academyResult);
                AddPupilNumbersTable(generator, academyResult);
                AddOfstedJudgementTable(generator, academyResult);

                generator.Build();
            }

            return ms.ToArray();
        }

        private static void AddOfstedJudgementTable(IDocumentBuilder generator, RepositoryResult<Academy> academyResult)
        {
            generator.AddHeading("Latest Ofsted judgement", DocumentHeadingBuilder.HeadingLevelOptions.Heading3);

            var data = academyResult.Result.LatestOfstedJudgement.FieldsToDisplay()
                .Select(field => new List<string> {field.Title, field.Value}).ToList();

            generator.AddTable(data);
        }

        private static void AddPupilNumbersTable(IDocumentBuilder generator, RepositoryResult<Academy> academyResult)
        {
            generator.AddHeading("Pupil numbers", DocumentHeadingBuilder.HeadingLevelOptions.Heading3);

            var data = academyResult.Result.PupilNumbers.FieldsToDisplay()
                .Select(field => new List<string> {field.Title, field.Value}).ToList();

            generator.AddTable(data);
        }

        private static void AddAcademyPerformanceTable(IDocumentBuilder generator,
            RepositoryResult<Academy> academyResult)
        {
            generator.AddHeading("Academy performance", DocumentHeadingBuilder.HeadingLevelOptions.Heading3);

            var academyPerformanceData = academyResult.Result.Performance.FieldsToDisplay()
                .Select(field => new List<string> {field.Title, field.Value}).ToList();

            generator.AddTable(academyPerformanceData);
        }
    }
}