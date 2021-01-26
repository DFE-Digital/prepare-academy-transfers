using API.Models.Downstream.D365;
using API.Models.Upstream.Request;
using System.Collections.Generic;
using System.Linq;

namespace API.Mapping.Request
{
    public class PostProjectsRequestMapper : IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model>
    {
        private readonly IMapper<PostProjectsAcademiesModel, PostAcademyTransfersProjectAcademyD365Model> _projectAcademiesMapper;

        public PostProjectsRequestMapper(IMapper<PostProjectsAcademiesModel, PostAcademyTransfersProjectAcademyD365Model> projectAcademiesMapper)
        {
            _projectAcademiesMapper = projectAcademiesMapper;
        }

        public PostAcademyTransfersProjectsD365Model Map(PostProjectsRequestModel input)
        {
            if (input == null)
            {
                return null;
            }

            var academies = input.ProjectAcademies?.Select(p => _projectAcademiesMapper.Map(p)).ToList();


            var output = new PostAcademyTransfersProjectsD365Model
            {
                ProjectInitiatorFullName = input.ProjectInitiatorFullName,
                ProjectInitiatorUid = input.ProjectInitiatorUid,
                ProjectStatus = MappingDictionaries.ProjecStatusEnumMap.GetValueOrDefault(input.ProjectStatus),
                Academies = academies,
                Trusts = input.ProjectTrusts != null
                         ? input.ProjectTrusts.Select(t => new PostAcademyTransfersProjectTrustD365Model
                         {
                             TrustId = $"/accounts({t.TrustId})"
                         })
                                              .ToList()
                         : new List<PostAcademyTransfersProjectTrustD365Model>()
            };

            return output;
        }
    }
}