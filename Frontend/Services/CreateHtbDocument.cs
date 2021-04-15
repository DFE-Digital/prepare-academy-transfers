using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using API.Repositories.Interfaces;
using DocumentGeneration;

namespace Frontend.Services
{
    public class CreateHtbDocument : ICreateHtbDocument
    {
        private readonly IProjectsRepository _projectsRepository;
        private readonly IAcademiesRepository _academiesRepository;

        public CreateHtbDocument(IProjectsRepository projectsRepository, IAcademiesRepository academiesRepository)
        {
            _projectsRepository = projectsRepository;
            _academiesRepository = academiesRepository;
        }

        public async Task<byte[]> Execute(Guid projectId)
        {
            var projectResult = await _projectsRepository.GetProjectById(projectId);
            var projectAcademy = projectResult.Result.ProjectAcademies.First().AcademyId;
            var academyResult = await _academiesRepository.GetAcademyById(projectAcademy);
            var academy = academyResult.Result;

            MemoryStream ms;

            await using (ms = new MemoryStream())
            {
                var generator = new DocumentBuilder(ms);

                generator.AddHeading($"General and performance information - {academy.AcademyName}",
                    DocumentHeadingBuilder.HeadingLevelOptions.Heading3);

                var tableData = new List<List<string>>
                {
                    new List<string> {"School phase", "Primary"},
                    new List<string> {"Age range", "4 to 11"},
                    new List<string> {"School type", academy.EstablishmentType},
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