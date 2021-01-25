using API.Models.Downstream.D365;
using API.Models.Upstream.Request;
using System.Collections.Generic;
using System.Linq;

namespace API.Mapping.Request
{
    public class PostProjectAcademiesRequestMapper : IMapper<PostProjectsAcademiesModel, PostAcademyTransfersProjectAcademyD365Model>
    {
        public PostAcademyTransfersProjectAcademyD365Model Map(PostProjectsAcademiesModel input)
        {
            return new PostAcademyTransfersProjectAcademyD365Model
            {
                AcademyId = $"/accounts({input.AcademyId})",
                EsfaInterventionReasons = input.EsfaInterventionReasons != null && input.EsfaInterventionReasons.Any()
                                          ? string.Join(',', input.EsfaInterventionReasons.Select(r => ((int)MappingDictionaries.EsfaInterventionReasonEnumMap.GetValueOrDefault(r)).ToString())
                                                                                          .ToList())
                                          : null,
                EsfaInterventionReasonsExplained = input.EsfaInterventionReasonsExplained,
                RddOrRscInterventionReasons = input.RddOrRscInterventionReasons != null && input.RddOrRscInterventionReasons.Any()
                                              ? string.Join(',', input.RddOrRscInterventionReasons.Select(r => ((int)MappingDictionaries.RddOrRscInterventionReasonEnumMap.GetValueOrDefault(r)).ToString())
                                                                                                  .ToList())
                                              : null,
                RddOrRscInterventionReasonsExplained = input.RddOrRscInterventionReasonsExplained,
                Trusts = input.Trusts != null
                         ? input.Trusts.Select(t => new PostAcademyTransfersProjectAcademyTrustD365Model { TrustId = $"/accounts({t.TrustId})" })
                                       .ToList()
                         : new List<PostAcademyTransfersProjectAcademyTrustD365Model>()
            };
        }
    }
}
