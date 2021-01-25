using API.Models.Downstream.D365;
using API.Models.Upstream.Enums;
using API.Models.Upstream.Response;
using System.Collections.Generic;
using System.Linq;

namespace API.Mapping.Response
{
    public class GetProjectsResponseMapper : IMapper<GetProjectsD365Model, GetProjectsResponseModel>
    {
        private readonly IMapper<AcademyTransfersProjectAcademy,
                                 GetProjectsAcademyResponseModel> _academyMapper;
        private readonly IEstablishmentNameFormatter _establishmentNameFormatter;

        public GetProjectsResponseMapper(IMapper<AcademyTransfersProjectAcademy,
                                                 GetProjectsAcademyResponseModel> academyMapper,
                                         IEstablishmentNameFormatter establishmentNameFormatter)
        {
            _academyMapper = academyMapper;
            _establishmentNameFormatter = establishmentNameFormatter;
        }

        public GetProjectsResponseModel Map(GetProjectsD365Model input)
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

        private static ProjectStatusEnum ExtractProjectStatus(GetProjectsD365Model input)
        {
            return input.ProjectStatus != default
                   ? MapProjectStatus(input)
                   : default;
        }

        private static ProjectStatusEnum MapProjectStatus(GetProjectsD365Model input)
        {
            var mappedStatus = MappingDictionaries.ProjecStatusEnumMap
                                                   .Where(v => v.Value == input.ProjectStatus);

            return mappedStatus.Any() ? mappedStatus.First().Key : default;
        }

        private List<GetProjectsTrustResponseModel> ExtractTrusts(GetProjectsD365Model input)
        {
            return input.Trusts == null || input.Trusts.Count == 0
                   ? new List<GetProjectsTrustResponseModel>()
                   : input.Trusts.Select(t => new GetProjectsTrustResponseModel
                   {
                       ProjectTrustId = t.ProjectTrustId,
                       TrustId = t.TrustId,
                       TrustName = _establishmentNameFormatter.Format(t.TrustName)
                   })
                                 .ToList();
        }

        private List<GetProjectsAcademyResponseModel> ExtractAcademies(GetProjectsD365Model input)
        {
            return input.Academies == null || input.Academies.Count == 0
                   ? new List<GetProjectsAcademyResponseModel>()
                   : input.Academies?.Select(a => _academyMapper.Map(a))
                                     .ToList();
        }
    }
}