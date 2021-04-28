using API.Models.Downstream.D365;
using API.Models.Upstream.Request;
using System.Collections.Generic;
using System.Linq;

namespace API.Mapping.Request
{
    public class PostProjectsRequestDynamicsMapper : IDynamicsMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model>
    {
        private readonly IDynamicsMapper<PostProjectsAcademiesModel, PostAcademyTransfersProjectAcademyD365Model> _projectAcademiesDynamicsMapper;

        public PostProjectsRequestDynamicsMapper(IDynamicsMapper<PostProjectsAcademiesModel, PostAcademyTransfersProjectAcademyD365Model> projectAcademiesDynamicsMapper)
        {
            _projectAcademiesDynamicsMapper = projectAcademiesDynamicsMapper;
        }

        public PostAcademyTransfersProjectsD365Model Map(PostProjectsRequestModel input)
        {
            if (input == null)
            {
                return null;
            }

            var academies = input.ProjectAcademies?.Select(p => _projectAcademiesDynamicsMapper.Map(p)).ToList();


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