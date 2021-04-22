using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using API.Repositories.Interfaces;
using Data;
using DocumentGeneration;

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

                generator.AddHeading($"General and performance information - {dynamicsAcademy.AcademyName}",
                    DocumentHeadingBuilder.HeadingLevelOptions.Heading3);

                var tableData = new List<List<string>>
                {
                    new List<string> {"School phase", "Primary"},
                    new List<string> {"Age range", "4 to 11"},
                    new List<string> {"School type", dynamicsAcademy.EstablishmentType},
                    new List<string> {"NOR (%full)", "113 (100%)"},
                    new List<string> {"Capacity", "113"},
                    new List<string> {"PAN", "17"},
                    new List<string> {"PFI", "No"},
                    new List<string> {"Viability issues?", "No"}
                };
                generator.AddTable(tableData);

                generator.Build();
            }

            return ms.ToArray();
        }
    }
}