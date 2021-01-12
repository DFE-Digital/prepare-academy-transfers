using API.Models.Downstream.D365;
using API.Models.Upstream.Enums;
using API.Models.Upstream.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Mapping
{
    public class GetProjectAcademiesResponseMapper : IMapper<AcademyTransfersProjectAcademy,
                                                             Models.Upstream.Response.GetProjectsAcademyResponseModel>
    {
        public GetProjectsAcademyResponseModel Map(AcademyTransfersProjectAcademy academy)
        {
            return new GetProjectsAcademyResponseModel
            {
                ProjectAcademyId = academy.AcademyTransfersProjectAcademyId,
                AcademyId = academy.AcademyId,
                AcademyName = academy.AcademyName,
                ProjectId = academy.ProjectId,
                EsfaInterventionReasons = ExtractEsfaInterventionReasons(academy),
                EsfaInterventionReasonsExplained = academy.EsfaInterventionReasonsExplained,
                RddOrRscInterventionReasons = ExtractRddorRscInterventionReason(academy),
                RddOrRscInterventionReasonsExplained = academy.RddOrRscInterventionReasonsExplained,
                AcademyTrusts = academy.ProjectAcademyTrusts == null || academy.ProjectAcademyTrusts.Count == 0
                                ? new List<GetAcademyTrustsResponseModel>()
                                : academy.ProjectAcademyTrusts.Select(t => new GetAcademyTrustsResponseModel
                                {
                                    ProjectTrustId = t.ProjectAcademyTrustId,
                                    TrustId = t.TrustId,
                                    TrustName = t.TrustName
                                })
                                                        .ToList()
            };
        }

        private static List<RddOrRscInterventionReasonEnum> ExtractRddorRscInterventionReason(AcademyTransfersProjectAcademy a)
        {
            return string.IsNullOrEmpty(a.RddOrRscInterventionReasons)
                   ? new List<RddOrRscInterventionReasonEnum>()
                   : a.RddOrRscInterventionReasons?
                      .Split(',')
                      .Select(v => Enum.Parse<Models.D365.Enums.RddOrRscInterventionReasonEnum>(v))
                      .Select(v => MappingDictionaries.RddOrRscInterventionReasonEnumMap.Where(d => d.Value == v).FirstOrDefault().Key)
                      .ToList();
        }

        private static List<EsfaInterventionReasonEnum> ExtractEsfaInterventionReasons(AcademyTransfersProjectAcademy a)
        {
            return string.IsNullOrEmpty(a.EsfaInterventionReasons)
                   ? new List<EsfaInterventionReasonEnum>()
                   : a.EsfaInterventionReasons?
                   .Split(',')
                   .Select(v => Enum.Parse<Models.D365.Enums.EsfaInterventionReasonEnum>(v))
                   .Select(v => MappingDictionaries.EsfaInterventionReasonEnumMap.Where(d => d.Value == v).FirstOrDefault().Key)
                   .ToList();
        }
    }
}
