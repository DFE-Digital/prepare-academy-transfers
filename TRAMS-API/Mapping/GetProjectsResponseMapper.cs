using API.Models.D365;
using API.Models.Upstream.Response;
using System;
using System.Linq;

namespace API.Mapping
{
    public class GetProjectsResponseMapper : IMapper<GetAcademyTransfersProjectsD365Model, GetProjectsResponseModel>
    {
        public GetProjectsResponseModel Map(GetAcademyTransfersProjectsD365Model input)
        {
            if (input == null)
            {
                return null;
            }

            return new GetProjectsResponseModel
            {
                ProjectId = input.ProjectId,
                ProjectName = input.ProjectName,
                ProjectStatus = MappingDictionaries.ProjecStatusEnumMap
                                                   .Where(v => v.Value == input.ProjectStatus)
                                                   .First()
                                                   .Key,
                ProjectInitiatorFullName = input.ProjectInitiatorFullName,
                ProjectInitiatorUid = input.ProjectInitiatorUid,
                ProjectAcademies = input.Academies?.Select(a => new GetProjectsAcademyResponseModel
                {
                    ProjectAcademyId = a.AcademyTransfersProjectAcademyId,
                    AcademyId = a.AcademyId,
                    AcademyName = a.AcademyName,
                    ProjectId = a.ProjectId,
                    EsfaInterventionReasons = a.EsfaInterventionReasons?
                                               .Split(',')
                                               .Select(v => Enum.Parse<Models.D365.Enums.EsfaInterventionReasonEnum>(v))
                                               .Select(v => MappingDictionaries.EsfaInterventionReasonEnumMap.Where(d => d.Value == v).FirstOrDefault().Key)
                                               .ToList(),
                    EsfaInterventionReasonsExplained = a.EsfaInterventionReasonsExplained,
                    RddOrRscInterventionReasons = a.RddOrRscInterventionReasons?
                                                   .Split(',')
                                                   .Select(v => Enum.Parse<Models.D365.Enums.RddOrRscInterventionReasonEnum>(v))
                                                   .Select(v => MappingDictionaries.RddOrRscInterventionReasonEnumMap.Where(d => d.Value == v).FirstOrDefault().Key)
                                                   .ToList()
                }).ToList(),
                ProjectTrusts = input.Trusts?.Select(t => new GetProjectsTrustResponseModel
                {
                    ProjectTrustId = t.ProjectTrustId,
                    TrustId = t.TrustId,
                    TrustName = t.TrustName
                }).ToList()
            };
        }
    }
}