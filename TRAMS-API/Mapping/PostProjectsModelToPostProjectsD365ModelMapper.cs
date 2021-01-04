using API.Models.D365;
using API.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Mapping
{
    public class PostProjectsModelToPostProjectsD365ModelMapper : IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model>
    {
        public PostAcademyTransfersProjectsD365Model Map(PostProjectsRequestModel input)
        {
            var output = new PostAcademyTransfersProjectsD365Model
            {
                ProjectInitiatorFullName = input.ProjectInitiatorFullName,
                ProjectInitiatorUid = input.ProjectInitiatorUid,
                ProjectStatus = MappingDictionaries.ProjectStatusMap.GetValueOrDefault(input.ProjectStatus),
                Academies = input.ProjectAcademies.Select(p => new PostAcademyTransfersProjectAcademyD365Model
                {
                    AcademyId = p.AcademyId.ToString(),
                    EsfaInterventionReasons = string.Join(',', p.EsfaInterventionReasons.Select(r => MappingDictionaries.EsfaInterventionReasonMap.GetValueOrDefault(r).ToString())),
                    EsfaInterventionReasonsExplained = p.EsfaInterventionReasonsExplained,
                    RddOrRscInterventionReasons = string.Join(',', p.RddOrRscInterventionReasons.Select(r => MappingDictionaries.RddOrRscInterventionReasonMap.GetValueOrDefault(r).ToString())),
                    RddOrRscInterventionReasonsExplained = p.RddOrRscInterventionReasonsExplained,
                    Trusts = p.Trusts.Select(t => new PostAcademyTransfersProjectAcademyTrustD365Model { TrustId = t.TrustId.ToString() }).ToList()
                }).ToList(),
                Trusts = input.ProjectTrusts.Select(t => new PostAcademyTransfersProjectTrustD365Model { TrustId = t.TrustId.ToString() }).ToList()
            };

            return output;
        }
    }
}
