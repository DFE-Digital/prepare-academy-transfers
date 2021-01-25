using API.Models.Downstream.D365;
using API.Models.Upstream.Request;
using System.Collections.Generic;
using System.Linq;

namespace API.Mapping.Request
{
    public class PutProjectAcademiesRequestMapper : IMapper<PutProjectAcademiesRequestModel, PatchProjectAcademiesD365Model>
    {
        public PatchProjectAcademiesD365Model Map(PutProjectAcademiesRequestModel input)
        {
            return new PatchProjectAcademiesD365Model
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
                RddOrRscInterventionReasonsExplained = input.RddOrRscInterventionReasonsExplained
            };
        }
    }
}
