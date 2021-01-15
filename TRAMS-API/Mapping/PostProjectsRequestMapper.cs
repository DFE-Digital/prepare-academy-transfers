using API.Models.D365;
using API.Models.Request;
using System.Collections.Generic;
using System.Linq;

namespace API.Mapping
{
    public class PostProjectsRequestMapper : IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model>
    {
        public PostAcademyTransfersProjectsD365Model Map(PostProjectsRequestModel input)
        {
            if (input == null)
            {
                return null;
            }

            var academies = input.ProjectAcademies?.Select(p => new PostAcademyTransfersProjectAcademyD365Model
            {
                AcademyId = $"/accounts({p.AcademyId})",
                EsfaInterventionReasons = p.EsfaInterventionReasons != null 
                                          ? string.Join(',', p.EsfaInterventionReasons.Select(r => ((int)MappingDictionaries.EsfaInterventionReasonEnumMap.GetValueOrDefault(r)).ToString())
                                                                                      .ToList()) 
                                          : null,
                EsfaInterventionReasonsExplained = p.EsfaInterventionReasonsExplained,
                RddOrRscInterventionReasons = p.RddOrRscInterventionReasons != null 
                                              ? string.Join(',', p.RddOrRscInterventionReasons.Select(r => ((int)MappingDictionaries.RddOrRscInterventionReasonEnumMap.GetValueOrDefault(r)).ToString())
                                                                                              .ToList())
                                              : null,
                RddOrRscInterventionReasonsExplained = p.RddOrRscInterventionReasonsExplained,
                Trusts = p.Trusts != null 
                         ? p.Trusts.Select(t => new PostAcademyTransfersProjectAcademyTrustD365Model { TrustId = $"/accounts({t.TrustId})" })
                                   .ToList() 
                         : new List<PostAcademyTransfersProjectAcademyTrustD365Model>()
            }).ToList();


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