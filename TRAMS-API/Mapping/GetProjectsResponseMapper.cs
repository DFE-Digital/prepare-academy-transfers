using API.Models.D365;
using API.Models.Upstream;
using API.Models.Upstream.Enums;
using API.Models.Upstream.Response;
using System;
using System.Collections.Generic;
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
                ProjectStatus = ExtractProjectStatus(input),
                ProjectInitiatorFullName = input.ProjectInitiatorFullName,
                ProjectInitiatorUid = input.ProjectInitiatorUid,
                ProjectAcademies = ExtractAcademies(input),
                ProjectTrusts = ExtractTrusts(input)
            };
        }

        private static ProjectStatusEnum ExtractProjectStatus(GetAcademyTransfersProjectsD365Model input)
        {
            return input.ProjectStatus != 0
                   ? MappingDictionaries.ProjecStatusEnumMap
                                           .Where(v => v.Value == input.ProjectStatus)
                                           .FirstOrDefault()
                                           .Key
                   : 0;
        }

        private static List<GetProjectsTrustResponseModel> ExtractTrusts(GetAcademyTransfersProjectsD365Model input)
        {
            return input.Trusts == null || input.Trusts.Count == 0
                   ? new List<GetProjectsTrustResponseModel>()
                   : input.Trusts.Select(t => new GetProjectsTrustResponseModel
                   {
                       ProjectTrustId = t.ProjectTrustId,
                       TrustId = t.TrustId,
                       TrustName = t.TrustName
                   }).ToList();
        }

        private static List<GetProjectsAcademyResponseModel> ExtractAcademies(GetAcademyTransfersProjectsD365Model input)
        {
            return input.Academies == null || input.Academies.Count == 0
                   ? new List<GetProjectsAcademyResponseModel>()
                   : input.Academies?.Select(a => new GetProjectsAcademyResponseModel
                   {
                       ProjectAcademyId = a.AcademyTransfersProjectAcademyId,
                       AcademyId = a.AcademyId,
                       AcademyName = a.AcademyName,
                       ProjectId = a.ProjectId,
                       EsfaInterventionReasons = ExtractEsfaInterventionReasons(a),
                       EsfaInterventionReasonsExplained = a.EsfaInterventionReasonsExplained,
                       RddOrRscInterventionReasons = ExtractRddorRscInterventionReason(a),
                       RddOrRscInterventionReasonsExplained = a.RddOrRscInterventionReasonsExplained
                   }).ToList();
        }

        private static List<RddOrRscInterventionReasonEnum> ExtractRddorRscInterventionReason(AcademyTransfersProjectAcademy a)
        {
            return string.IsNullOrEmpty(a.RddOrRscInterventionReasons)
                   ? new List<Models.Upstream.RddOrRscInterventionReasonEnum>()
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