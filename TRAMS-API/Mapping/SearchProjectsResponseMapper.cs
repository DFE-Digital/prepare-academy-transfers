using API.Models.Downstream.D365;
using API.Models.Upstream.Enums;
using API.Models.Upstream.Response;
using System.Linq;

namespace API.Mapping
{
    public class SearchProjectsResponseMapper : IMapper<SearchProjectsD365Model, SearchProjectsModel>
    {
        public SearchProjectsModel Map(SearchProjectsD365Model input)
        {
            if (input == null)
            {
                return null;
            }

            return new SearchProjectsModel
            {
                ProjectId = input.ProjectId,
                ProjectName = input.ProjectName,
                ProjectInitiatorFullName = input.ProjectInitiatorFullName,
                ProjectInitiatorUid = input.ProjectInitiatorUid,
                ProjectStatus = input.ProjectStatus != default
                                ? MapProjectStatus(input)
                                : default
            };
        }

        private static ProjectStatusEnum MapProjectStatus(SearchProjectsD365Model input)
        {
            var mappedStatus = MappingDictionaries.ProjecStatusEnumMap
                                                  .Where(v => v.Value == input.ProjectStatus);

            return mappedStatus.Any() ? mappedStatus.First().Key : default;
        }
    }
}
